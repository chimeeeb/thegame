using System;
using System.IO;
using System.Threading;
using System.Linq;
using GameLibrary.Configuration;
using GameLibrary.Enum;
using GameLibrary.Interface;
using NUnit.Framework;

namespace TCPTests
{
    [TestFixture]
    public class GameEndTests
    {
        static GameEndTests()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void GM_Should_EndGame_When_RedTeamWins(
            [Values(1, 2, 3, 4)] int playersPerTeam)
        {
            GameSettings settings = GameSettings.GetDefault();
            settings.NumberOfGoalsPerTeam = 1;
            settings.NumberOfPieces = 0; // To avoid confusion with dummy pieces
            using (var environment =
                Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                using (CountdownEvent cde = new CountdownEvent(2 * playersPerTeam + 1))
                {
                    environment.Players.ForEach((p) => p.ServerDisconnected += () => cde.Signal());
                    environment.GameMaster.ServerDisconnected += () => cde.Signal();

                    var player = environment.Players.Where((p) => p.Team == Team.Red)
                                                    .FirstOrDefault();
                    ITile agentTile = player.Tile as Player.Tile;
                    Game.Map map = environment.GameMaster.Map;
                    Game.Tile gmTile = map[agentTile.X, agentTile.Y];
                    Game.Tile goalTile = map[0, 0];

                    // Find generated goal tile
                    for(int i = 0; i < map.Width; i++)
                    {
                        for(int j = 0; j < map.GoalAreaHeight; j++)
                        {
                            if(map[i,j].Type == TileType.Goal)
                            {
                                goalTile = map[i, j];
                                break;
                            }
                        }
                    }

                    // Pick up a piece
                    player.Map[agentTile.X, agentTile.Y].UpdateTile(DateTime.Now, -1);
                    player.Tile = player.Map[goalTile.X, goalTile.Y];
                    environment.GameMaster.Agents[player.Id].Piece = Piece.Real;
                    environment.GameMaster.NumberOfPieces = 1;
                    environment.GameMaster.Agents[player.Id].Tile = map[goalTile.X, goalTile.Y];
                    map[agentTile.X, agentTile.Y].UpdateTile(DateTime.Now);
                    map[goalTile.X, goalTile.Y].UpdateTile(DateTime.Now, player.Id, null, goalTile.Type);

                    // Put piece and win
                    player.PiecePut();
                        
                    Assert.IsTrue(cde.Wait(2000), $"Game end operation timed out, so far {cde.CurrentCount} modules remain connected.");

                    environment.CheckGmDisconnected(settings);
                    environment.CheckAllAgentsDisconnected();
                    environment.CheckServerDisconnected();
                }
            }
        }
    }
}
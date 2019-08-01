using System;
using System.IO;
using GameLibrary.Configuration;
using GameLibrary.Enum;
using GameLibrary.Messages;
using NUnit.Framework;

namespace TCPTests
{
    [TestFixture]
    public class EnvironmentSetupTests
    {
        static EnvironmentSetupTests()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test, Order(1)]
        public void Should_NotThrow_When_CreatingEnvironmentWithConstructor()
        {
            Assert.DoesNotThrow(() => {
                Environment environment = new Environment();
                environment.Dispose();
            });
        }

        [Test, Order(2)]
        public void Should_CreateEnvironmentAndConnectGm_When_CreatingEnvironmentFromGivenSettings()
        {
            GameSettings gameSettings = GameSettings.GetDefault();
            using (Environment environment = new Environment(gameSettings))
            {
                // Correct GM
                Assert.AreEqual(0, environment.GameMaster.BlueTeamIds.Count);
                Assert.AreEqual(0, environment.GameMaster.RedTeamIds.Count);
                Assert.AreEqual(0, environment.GameMaster.BlueTeamPoints);
                Assert.AreEqual(0, environment.GameMaster.RedTeamPoints);
                Assert.AreEqual(0, environment.GameMaster.Agents.Count);
                Assert.AreEqual(0, environment.GameMaster.Exchanges.Count);
                Assert.AreEqual(GameMasterState.Connected, environment.GameMaster.State);
                Assert.AreEqual(gameSettings.MapHeight, environment.GameMaster.Map.Height);
                Assert.AreEqual(gameSettings.MapWidth, environment.GameMaster.Map.Width);
                Assert.AreEqual(gameSettings.GoalAreaHeight, environment.GameMaster.Map.GoalAreaHeight);
                Assert.AreEqual(0, environment.GameMaster.NumberOfPieces);
                Assert.AreEqual(0, environment.GameMaster.NumberOfPlayers);
                Assert.AreEqual(0, environment.GameMaster.NumberOfTeamBluePlayers);
                Assert.AreEqual(0, environment.GameMaster.NumberOfTeamRedPlayers);
                Assert.AreEqual(Team.None, environment.GameMaster.WinningTeam);
                Assert.AreEqual(gameSettings, environment.GameMaster.Settings);

                // Correct Server
                Assert.AreEqual(0, environment.Server.AgentIdsToIpPort.Count);
                Assert.IsFalse(environment.Server.Debug);
                Assert.AreEqual(ServerState.GmConnected, environment.Server.State);
                Assert.IsNotEmpty(environment.Server.GmIpPort);

                // Correct Players
                Assert.AreEqual(0, environment.Players.Count);
            }
        }

        [Test, Order(3)]
        public void Should_NotThrow_When_ConnectingNewPlayerToServer(
            [Values(Team.Red, Team.Blue)] Team team,
            [Values(StrategyType.Normal, StrategyType.Random)] StrategyType strategy,
            [Values(true, false)] bool wantBeALeader,
            [Values(true, false)] bool isUsingStrategy)
        {
            using (Environment environment = new Environment())
            {
                AgentSettings agentSettings = new AgentSettings
                {
                    ServerPort = environment.GameMaster.Settings.ServerPort,
                    ServerIp = environment.GameMaster.Settings.ServerIp,
                    Strategy = strategy,
                    Team = team,
                    WantBeALeader = wantBeALeader
                };

                Assert.DoesNotThrow(() => environment.AddNewPlayer(team, strategy, wantBeALeader, isUsingStrategy));
            }
        }

        [Test, Order(4)]
        public void Should_AddAndConnectNewPlayer_With_GivenSettings(
            [Values(Team.Red, Team.Blue)] Team team,
            [Values(StrategyType.Normal, StrategyType.Random)] StrategyType strategy,
            [Values(true, false)] bool wantBeALeader,
            [Values(true, false)] bool isUsingStrategy)
        {
            using (Environment environment = new Environment())
            {
                AgentSettings agentSettings = new AgentSettings
                {
                    ServerPort = environment.GameMaster.Settings.ServerPort,
                    ServerIp = environment.GameMaster.Settings.ServerIp,
                    Strategy = strategy,
                    Team = team,
                    WantBeALeader = wantBeALeader
                };

                environment.AddNewPlayer(team, strategy, wantBeALeader, isUsingStrategy);

                Assert.AreEqual(1, environment.Players.Count);
                var player = environment.Players[0];
                Assert.AreEqual(agentSettings, player.Settings);
                Assert.AreEqual(isUsingStrategy, player.IsUsingStrategy);
                Assert.AreEqual(AgentState.Disconnected, player.State);
                Assert.IsNull(player.Map);
                Assert.IsNull(player.ReceivedMessage);
                Assert.IsNull(player.Tile);
                Assert.IsNull(player.Strategy);

                // Values set when connecting to the server
                Assert.AreEqual(wantBeALeader, player.IsLeader);
                Assert.AreEqual(team, player.Team);
            }
        }

        [Test, Order(5)]
        public void Should_NotThrow_When_ConnectingPlayersToGame(
            [Values (1, 2, 3, 4)] int playersPerTeam,
            [Values(true, false)] bool areAgentsIdle)
        {
            GameSettings settings = GameSettings.GetDefault();
            settings.NumberOfPlayers = 2 * playersPerTeam;
            using (Environment environment = new Environment(settings))
            {
                for (int i = 0; i < playersPerTeam; i++)
                {
                    environment.AddNewPlayer(team: Team.Blue, isUsingStrategy: !areAgentsIdle);
                    environment.AddNewPlayer(team: Team.Red, isUsingStrategy: !areAgentsIdle);
                }

                Assert.DoesNotThrow(() => environment.ConnectPlayers());
            }
        }

        [Test]
        public void Should_DisconnectModules_When_DisposingEnvironment(
            [Values(1, 2, 3, 4)] int playersPerTeam,
            [Values(true, false)] bool areAgentsIdle,
            [Values(true, false)] bool isGameRunning)
        {
            GameSettings settings = GameSettings.GetDefault();
            Environment environment;
            using (environment = Environment.CreateEnvironment(playersPerTeam, settings, areAgentsIdle, isGameRunning))
            { }
            // Correct GM
            environment.CheckGmDisconnected(settings);

            // Correct Server
            environment.CheckServerDisconnected();

            // Correct players
            Assert.AreEqual(playersPerTeam * 2, environment.Players.Count, "Environment players count invalid.");
            environment.CheckAllAgentsDisconnected();
        }

        [Test]
        public void Should_CreateCorrectEnvironment_With_StaticMethodAndIdleAgents(
            [Values(1, 2, 3, 4)] int playersPerTeam,
            [Values(true, false)] bool isGameRunning)
        {
            GameSettings settings = GameSettings.GetDefault();
            using (Environment environment = Environment.CreateEnvironment(playersPerTeam, settings, true, isGameRunning))
            {
                // Correct GM
                Assert.AreEqual(isGameRunning ? GameMasterState.GameRunning : GameMasterState.ReadyForGame, environment.GameMaster.State);
                Assert.AreEqual(playersPerTeam, environment.GameMaster.BlueTeamIds.Count, "GM Blue team ids count invalid.");
                Assert.AreEqual(playersPerTeam, environment.GameMaster.RedTeamIds.Count, "GM Red team ids count invalid.");
                Assert.AreEqual(playersPerTeam * 2, environment.GameMaster.Agents.Count, "GM Agents count invalid.");
                Assert.AreEqual(0, environment.GameMaster.BlueTeamPoints, "GM Blue team points number invalid.");
                Assert.AreEqual(0, environment.GameMaster.RedTeamPoints, "GM Red team points number invalid.");
                Assert.AreEqual(playersPerTeam * 2, environment.GameMaster.NumberOfPlayers, "GM number of players invalid.");
                Assert.AreEqual(playersPerTeam, environment.GameMaster.NumberOfTeamBluePlayers, "GM number of blue players invalid.");
                Assert.AreEqual(playersPerTeam, environment.GameMaster.NumberOfTeamRedPlayers, "GM number of red players invalid.");

                // Correct Server
                Assert.AreEqual(playersPerTeam * 2, environment.Server.AgentIdsToIpPort.Count, "Server agents ids count invalid.");
                Assert.AreEqual(isGameRunning ? ServerState.GameRunning : ServerState.GmConnected, environment.Server.State, "Server state invalid.");
                Assert.IsNotEmpty(environment.Server.GmIpPort, "Server GM IpPort string invalid.");
                Assert.IsNull(environment.Server.GameEndedMessage, "Game ended message invalid.");

                // Correct players
                Assert.AreEqual(playersPerTeam * 2, environment.Players.Count, "Environment players count invalid.");
                Assert.AreEqual(1, environment.Players.FindAll((player) => player.Id == 0).Count);
                foreach (var player in environment.Players)
                {
                    Assert.AreEqual(isGameRunning ? AgentState.IsPlaying : AgentState.Connected, player.State, "Player state invalid.");
                    if (isGameRunning)
                    {
                        Assert.IsInstanceOf(typeof(GameInfoMessage), player.ReceivedMessage, "Player received message invalid.");
                        Assert.AreEqual(environment.GameMaster.Map.Height, player.Map.Height, "Player map height invalid.");
                        Assert.AreEqual(environment.GameMaster.Map.GoalAreaHeight, player.Map.GoalAreaHeight, "Player goal area height invalid.");
                        Assert.AreEqual(environment.GameMaster.Map.Width, player.Map.Width, "Player map width invalid.");
                        Assert.IsNotNull(player.Tile, "Player tile invalid.");
                        Assert.IsNotNull(player.Strategy, "Player strategy invalid.");
                    }
                    else
                    {
                        Assert.IsInstanceOf(typeof(AcceptedToGameMessage), player.ReceivedMessage, "Player received message invalid.");
                    }
                }
            }
        }
    }
}
 
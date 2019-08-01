using System;
using System.IO;
using System.Threading;
using GameLibrary.Configuration;
using GameLibrary.Enum;
using GameLibrary.Messages;
using GameLibrary.Serialization;
using NUnit.Framework;

namespace TCPTests
{
    [TestFixture]
    public class AgentActionsTests
    {
        static AgentActionsTests()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void Agents_Should_ConnectToGame(
            [Values(1, 2, 3, 4)] int playersPerTeam)
        {
            var settings = GameSettings.GetDefault();
            settings.NumberOfPlayers = 2 * playersPerTeam;
            using (var env = new Environment(settings))
            {
                for (int i = 0; i < playersPerTeam; i++)
                {
                    env.AddNewPlayer(team: Team.Blue);
                    env.AddNewPlayer(team: Team.Red);
                }
                Assert.DoesNotThrow(() => env.ConnectPlayers());
                Assert.AreEqual(2 * playersPerTeam, env.GameMaster.NumberOfPlayers);
                Assert.AreEqual(playersPerTeam, env.GameMaster.NumberOfTeamBluePlayers);
                Assert.AreEqual(playersPerTeam, env.GameMaster.NumberOfTeamRedPlayers);
            }
        }

        [Test]
        public void Agents_Should_ReceiveGameInfoMessage_When_TheGameStarts(
            [Values(1, 2, 3, 4)] int playersPerTeam)
        {
            GameSettings settings = GameSettings.GetDefault();
            using (var env =
                Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                foreach (var player in env.Players)
                {
                    //agent used information in game info to update map
                    Assert.IsTrue(player.ReceivedMessage is GameInfoMessage);
                }
            }
        }

        [Test]
        public void Agent_Should_ReceiveDiscoveryResultMessage_When_PerformingDiscoveryAction(
            [Values(1, 2, 3, 4)] int playersPerTeam)
        {
            GameSettings settings = GameSettings.GetDefault();
            using (var env =
                Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                var playerReceivedInGameMessageEventRaised = new ManualResetEvent(false);
                foreach (var pl in env.Players)
                {
                    pl.ReceivedMessageDuringGame += () => playerReceivedInGameMessageEventRaised.Set();
                }
                Random rnd = new Random();
                var player = env.Players[rnd.Next(env.Players.Count)];
                player.Discovery();
                Assert.IsTrue(playerReceivedInGameMessageEventRaised.WaitOne(1000), "Player didnt receive response in less than second after discovery action");
                Assert.IsTrue(player.ReceivedMessage is DiscoveryResultMessage);
                playerReceivedInGameMessageEventRaised.Dispose();
            }
        }

        [Test]
        public void Agent_Should_ReceivePickUpResultMessage_When_PerformingPiecePickUp_And_StandingOnPiece(
            [Values(1, 2, 3, 4)] int playersPerTeam)
        {
            GameSettings settings = GameSettings.GetDefault();
            settings.MapHeight = 4;
            settings.MapWidth = 4;
            settings.GoalAreaHeight = 1;
            // To avoid confusion with randomly generated pieces
            settings.NumberOfPieces = 0;
            using (var env =
                Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                foreach (var player in env.Players)
                {
                    using (var playerReceivedInGameMessageEventRaised = new ManualResetEvent(false))
                    {
                        player.ReceivedMessageDuringGame += () => playerReceivedInGameMessageEventRaised.Set();
                        // Move player to the task area
                        if (player.Team == Team.Blue)
                        {
                            player.Move(Direction.Down);
                            Assert.IsTrue(playerReceivedInGameMessageEventRaised.WaitOne(1000), "Player didnt receive response in less than second after move action");
                        }
                        else
                        {
                            player.Move(Direction.Up);
                            Assert.IsTrue(playerReceivedInGameMessageEventRaised.WaitOne(1000), "Player didnt receive response in less than second after move action");

                        }
                        Assert.IsInstanceOf(typeof(MoveResultMessage), player.ReceivedMessage);
                        Assert.AreEqual(TileType.Task, env.GameMaster.Map[player.Tile.X, player.Tile.Y].Type);
                        Assert.AreEqual(PieceStatus.None, player.Strategy.Piece);

                        env.GameMaster.Map[player.Tile.X, player.Tile.Y].UpdateTile(DateTime.Now, player.Id, Piece.Real);
                        env.GameMaster.NumberOfPieces++;

                        playerReceivedInGameMessageEventRaised.Reset();
                        player.PiecePickUp();
                        Assert.IsTrue(playerReceivedInGameMessageEventRaised.WaitOne(1000), "Player didnt receive response in less than second after pick up action");
                        Assert.IsInstanceOf(typeof(PickUpResultMessage), player.ReceivedMessage);
                        Assert.AreEqual(PieceStatus.Unidentified, player.Strategy.Piece); 
                    }
                }
            }
        }

        [Test]
        public void Agent_Should_ReceiveInvalidActionMessage_When_PerformingPiecePickUp_And_NOTStandingOnPiece(
           [Values(1, 2, 3, 4)] int playersPerTeam)
        {
            GameSettings settings = GameSettings.GetDefault();
            settings.NumberOfPieces = 0;
            using (var env =
                Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                using (var playerReceivedInGameMessageEventRaised = new ManualResetEvent(false))
                {
                    Random rnd = new Random();
                    var player = env.Players[rnd.Next(env.Players.Count)];
                    player.ReceivedMessageDuringGame += () => playerReceivedInGameMessageEventRaised.Set();
                    player.PiecePickUp();
                    Assert.IsTrue(playerReceivedInGameMessageEventRaised.WaitOne(1000), "Player didnt receive response in less than second after pick up action");
                    Assert.IsTrue(player.ReceivedMessage is InvalidActionMessage); 
                }
            }
        }

        [Test]
        public void Agent_Should_ReceiveInvalidActionMessage_When_PerformingDestroyPiece_And_NOTHoldingPiece(
           [Values(1, 2, 3, 4)] int playersPerTeam)
        {
            GameSettings settings = GameSettings.GetDefault();
            using (var env =
                Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                using (var playerReceivedInGameMessageEventRaised = new ManualResetEvent(false))
                {
                    Random rnd = new Random();
                    var player = env.Players[rnd.Next(env.Players.Count)];
                    player.ReceivedMessageDuringGame += () => playerReceivedInGameMessageEventRaised.Set();
                    player.PieceDestroy();
                    Assert.IsTrue(playerReceivedInGameMessageEventRaised.WaitOne(1000), "Player didnt receive response in less than second after destroy piece action");
                    Assert.IsTrue(player.ReceivedMessage is InvalidActionMessage);
                }
            }
        }

        [Test]
        public void Agent_Should_ReceiveInvalidActionMessage_When_PerformingTestPiece_And_NOTHoldingPiece(
           [Values(1, 2, 3, 4)] int playersPerTeam)
        {
            GameSettings settings = GameSettings.GetDefault();
            using (var env =
                Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                using (var playerReceivedInGameMessageEventRaised = new ManualResetEvent(false))
                {

                    Random rnd = new Random();
                    var player = env.Players[rnd.Next(env.Players.Count)];
                    player.ReceivedMessageDuringGame += () => playerReceivedInGameMessageEventRaised.Set();
                    player.PieceTest();
                    Assert.IsTrue(playerReceivedInGameMessageEventRaised.WaitOne(1000), "Player didnt receive response in less than second after test piece action");
                    Assert.IsTrue(player.ReceivedMessage is InvalidActionMessage);
                }
            }
        }

        [Test]
        public void Agent_Should_ReceiveCannotMoveThere_When_MovingOutOfMapLeftRight(
           [Values(1)] int playersPerTeam,
           [Values(Direction.Right,Direction.Left)] Direction direction)
        {
            GameSettings settings = GameSettings.GetDefault();
            settings.MapHeight = 4;
            settings.MapWidth = 1;
            settings.GoalAreaHeight = 1;
            using (var env =
                Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                foreach (var player in env.Players)
                {
                    using (var playerReceivedInGameMessageEventRaised = new ManualResetEvent(false))
                    {
                        player.ReceivedMessageDuringGame += () => playerReceivedInGameMessageEventRaised.Set();
                        player.Move(direction);
                        Assert.IsTrue(playerReceivedInGameMessageEventRaised.WaitOne(1000), "Player didnt receive response in less than second after move action");
                        Assert.IsTrue(player.ReceivedMessage is CannotMoveThereMessage); 
                    }
                }
            }
        }

        [Test]
        public void Agent_Should_ReceiveTestPieceResultMessage_When_PerformingTestPiece_And_HoldingPiece(
            [Values(1, 2, 3, 4)] int playersPerTeam,
            [Values(Piece.Fake,Piece.Real)] Piece pieceType)
        {
            GameSettings settings = GameSettings.GetDefault();
            settings.NumberOfPieces = 0;
            using (var env =
             Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                foreach (var player in env.Players)
                {
                    using (var pReceivedMsg = new ManualResetEvent(false))
                    {
                        player.ReceivedMessageDuringGame += () => pReceivedMsg.Set();
                        // Move player to the task area
                        if (player.Team == Team.Blue)
                        {
                            player.Move(Direction.Down);
                            Assert.IsTrue(pReceivedMsg.WaitOne(1000));
                        }
                        else
                        {
                            player.Move(Direction.Up);
                            Assert.IsTrue(pReceivedMsg.WaitOne(1000));
                        }
                        pReceivedMsg.Reset();

                        //create piece under agent
                        env.GameMaster.Map[player.Tile.X, player.Tile.Y].UpdateTile(DateTime.Now, player.Id, pieceType);
                        env.GameMaster.NumberOfPieces++;
                        player.PiecePickUp();
                        Assert.IsTrue(pReceivedMsg.WaitOne(1000));
                        pReceivedMsg.Reset();

                        player.PieceTest();
                        Assert.IsTrue(pReceivedMsg.WaitOne(1000), "Player didnt receive response in less than second after test piece action");
                        Assert.IsInstanceOf(typeof(TestPieceResultMessage), player.ReceivedMessage);
                        Assert.AreEqual(pieceType==Piece.Fake?PieceStatus.Sham:PieceStatus.Real, player.Strategy.Piece);
                    }
                }
            }
        }

        [Test]
        public void Agent_Should_ReceiveDestroyPieceResultMessage_When_PerformingDestroyPiece_And_HoldingPiece(
            [Values(1, 2, 3, 4)] int playersPerTeam,
            [Values(Piece.Fake, Piece.Real)] Piece pieceType)
        {
            GameSettings settings = GameSettings.GetDefault();
            settings.NumberOfPieces = 0;
            using (var env =
             Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                foreach (var player in env.Players)
                {
                    using (var pReceivedMsg = new ManualResetEvent(false))
                    {
                        player.ReceivedMessageDuringGame += () => pReceivedMsg.Set();
                        // Move player to the task area
                        if (player.Team == Team.Blue)
                        {
                            player.Move(Direction.Down);
                            Assert.IsTrue(pReceivedMsg.WaitOne(1000));
                        }
                        else
                        {
                            player.Move(Direction.Up);
                            Assert.IsTrue(pReceivedMsg.WaitOne(1000));
                        }
                        pReceivedMsg.Reset();

                        //create piece under agent
                        env.GameMaster.Map[player.Tile.X, player.Tile.Y].UpdateTile(DateTime.Now, player.Id, pieceType);
                        env.GameMaster.NumberOfPieces++;
                        player.PiecePickUp();
                        Assert.IsTrue(pReceivedMsg.WaitOne(1000));
                        pReceivedMsg.Reset();

                        player.PieceDestroy();
                        Assert.IsTrue(pReceivedMsg.WaitOne(1000), "Player didnt receive response in less than second after destroy piece action");
                        Assert.IsInstanceOf(typeof(DestroyPieceResultMessage), player.ReceivedMessage);
                        Assert.AreEqual(PieceStatus.None, player.Strategy.Piece);
                        Assert.AreEqual(0, env.GameMaster.NumberOfPieces);

                    }
                }
            }
        }


        [Test]
        public void Agent_Should_ReceivePutPieceResultMessage_When_PerformingPutPiece_And_HoldingPieceInTaskArea(
            [Values(1, 2, 3, 4)] int playersPerTeam,
            [Values(Piece.Fake, Piece.Real)] Piece pieceType)
        {
            GameSettings settings = GameSettings.GetDefault();
            settings.GoalAreaHeight = 1;
            settings.NumberOfPieces = 0;
            using (var env =
             Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                int numOfPieces = 0;
                foreach (var player in env.Players)
                {
                    using (var pReceivedMsg = new ManualResetEvent(false))
                    {
                        using (CountdownEvent cde = new CountdownEvent(3))
                        {
                            player.ReceivedMessageDuringGame += () => pReceivedMsg.Set();
                            env.GameMaster.ReceivedRequestInGame += (int id) =>
                            {
                                if (id == player.Id)
                                    cde.Signal();
                            };

                            // Move player to the task area, important that goal area height is one (randomizing starting positions of agents inside goal area)
                            if (player.Team == Team.Blue)
                            {
                                player.Move(Direction.Down);
                                Assert.IsTrue(pReceivedMsg.WaitOne(1000));
                            }
                            else
                            {
                                player.Move(Direction.Up);
                                Assert.IsTrue(pReceivedMsg.WaitOne(1000));
                            }
                            pReceivedMsg.Reset();

                            //create piece under agent
                            env.GameMaster.Map[player.Tile.X, player.Tile.Y].UpdateTile(DateTime.Now, player.Id, pieceType);
                            env.GameMaster.NumberOfPieces++;
                            numOfPieces++;

                            player.PiecePickUp();
                            Assert.IsTrue(pReceivedMsg.WaitOne(1000));
                            pReceivedMsg.Reset();
                            
                            player.PiecePut();
                            Assert.IsTrue(cde.Wait(1000), "GM didnt receive request in less than second after put piece request");
                            Assert.AreEqual(pieceType, env.GameMaster.Map[player.Tile.X, player.Tile.Y].Piece);


                            Assert.IsTrue(pReceivedMsg.WaitOne(1000), "Player didnt receive response in less than second after test action");
                            Assert.IsInstanceOf(typeof(PutPieceResultMessage), player.ReceivedMessage);
                            Assert.AreEqual(PieceStatus.None, player.Strategy.Piece);             
                            Assert.AreEqual(numOfPieces, env.GameMaster.NumberOfPieces);

                        }
                    }
                }
            }
        }


        [Test]
        public void Agent_Should_ReceivePutPieceResultMessage_When_PerformingPutPiece_And_HoldingPieceInGoalArea(
           [Values(1, 2, 3, 4)] int playersPerTeam,
           [Values(Piece.Fake, Piece.Real)] Piece pieceType)
        {
            GameSettings settings = GameSettings.GetDefault();
            settings.NumberOfPieces = 0;
            using (var env =
             Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                foreach (var player in env.Players)
                {
                    using (var pReceivedMsg = new ManualResetEvent(false))
                    {
                        using (CountdownEvent cde = new CountdownEvent(2))
                        {
                            player.ReceivedMessageDuringGame += () => pReceivedMsg.Set();
                            env.GameMaster.ReceivedRequestInGame += (int id) =>
                            {
                                if (id == player.Id)
                                    cde.Signal();
                            };

                            //create piece under agent
                            env.GameMaster.Map[player.Tile.X, player.Tile.Y].UpdateTile(DateTime.Now, player.Id, pieceType);
                            var tileTypeBefore = env.GameMaster.Map[player.Tile.X, player.Tile.Y].Type;
                            env.GameMaster.NumberOfPieces++;

                            player.PiecePickUp();
                            Assert.IsTrue(pReceivedMsg.WaitOne(1000));
                            pReceivedMsg.Reset();

                            player.PiecePut();
                            Assert.IsTrue(cde.Wait(1000), "GM didnt receive request in less than second after put piece request");
                            if (tileTypeBefore == TileType.Goal)
                            {
                                Assert.AreEqual(pieceType == Piece.Real ? TileType.DiscoveredGoal : TileType.Goal, env.GameMaster.Map[player.Tile.X, player.Tile.Y].Type);
                            }
                            else
                            {
                                Assert.AreEqual(tileTypeBefore, env.GameMaster.Map[player.Tile.X, player.Tile.Y].Type);
                            }

                            Assert.IsTrue(pReceivedMsg.WaitOne(1000), "Player didnt receive response in less than second after test action");
                            Assert.IsInstanceOf(typeof(PutPieceResultMessage), player.ReceivedMessage);
                            Assert.AreEqual(PieceStatus.None, player.Strategy.Piece);
                            Assert.AreEqual(0, env.GameMaster.NumberOfPieces);

                        }
                    }
                }
            }
        }
    }
}
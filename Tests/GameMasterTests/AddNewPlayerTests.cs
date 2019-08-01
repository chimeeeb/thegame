//using System;
//using System.Drawing;
//using NUnit.Framework;
//using ProjectGame;
//using ProjectGame.Items;
//using ProjectGame.Logic;
//using Tests.Repositories;
//using Tests.TestClasses;

//namespace Tests.GameMasterTests
//{
//    class AddNewPlayerTests: GameMasterTestsBase
//    {
//        private IGameMasterRepository _gmRepository;
//        [Test]
//        public void Should_AddNewAgent_When_HavingLessPlayersThanMax()
//        {
//            var maxNumberOfPlayers = 6;
//            var gm = new GameMaster();


//            //int maxNumberOfPlayers = 6;
//            //GameMasterMock gm = new GameMasterMock(5, 5, 1, 1, maxNumberOfPlayers);
//            //for (int i = 0; i < maxNumberOfPlayers / 2; i++)
//            //{
//            //    Assert.That(() => gm.AddNewPlayer(Team.Red), Throws.Nothing);
//            //    Assert.That(() => gm.AddNewPlayer(Team.Blue), Throws.Nothing);
//            //}
//            //Assert.AreEqual(Parameters.NumberOfPlayers, gm.NumberOfPlayers);
//            //Assert.AreEqual(Parameters.NumberOfPlayers / 2, gm.NumberOfTeamBluePlayers);
//            //Assert.AreEqual(Parameters.NumberOfPlayers / 2, gm.NumberOfTeamRedPlayers);
//        }
        
//        [Test]
//        public void Should_RejectConnectToGameRequest_When_TeamsAreFull()
//        {
//            int maxNumberOfPlayers = 2;
//            GameMasterMock gm = new GameMasterMock(5, 5, 1, 1, maxNumberOfPlayers);
//            Assert.That(() => gm.AddNewPlayer(Team.Red), Throws.Nothing);
//            Assert.That(() => gm.AddNewPlayer(Team.Blue), Throws.Nothing);
//            Assert.Throws<ArgumentOutOfRangeException>(() => gm.AddNewPlayer(Team.Red));
//            Assert.Throws<ArgumentOutOfRangeException>(() => gm.AddNewPlayer(Team.Blue));
//        }

//        [Test]
//        public void Should_RejectConnectToGameRequest_When_MapIsFull()
//        {
//            int maxNumberOfPlayers = 26;
//            GameMasterMock gm = new GameMasterMock(5, 5, 1, 1, maxNumberOfPlayers);
//            Team team = Team.Red;
//            for (int i = 0; i < maxNumberOfPlayers - 1; i++)
//            {
//                if (i >= maxNumberOfPlayers / 2) team = Team.Blue;
//                Assert.That(() => gm.AddNewPlayer(team), Throws.Nothing);
//            }
//            Assert.Throws<ArgumentOutOfRangeException>(() => gm.AddNewPlayer(team));
//        }

//        [Test]
//        public void Should_FindCorrectPlaceForAgent_WhenAddingNewPlayer()
//        {
//            int maxNumberOfPlayers = 6;
//            GameMasterMock gm = new GameMasterMock(5, 5, 1, 1, maxNumberOfPlayers);
//            Team team = Team.Red;
//            for (int i = 0; i < maxNumberOfPlayers; i++)
//            {
//                if (i >= maxNumberOfPlayers / 2) team = Team.Blue;
//                gm.AddNewPlayer(team);

//                Assert.Less(gm.AgentsPositions[i].X, gm.Map.Width);
//                Assert.Less(gm.AgentsPositions[i].Y, gm.Map.Height);
//                Assert.GreaterOrEqual(gm.AgentsPositions[i].X, 0);
//                Assert.GreaterOrEqual(gm.AgentsPositions[i].Y, 0);

//                GameMasterTile t = gm.Map.GetTile(gm.AgentsPositions[i].X, gm.AgentsPositions[i].Y);
//                Assert.AreEqual(i, t.AgentId);
//                Assert.AreEqual(null, t.Piece);
//            }
//        }

//        [Test, Timeout(3000)]
//        public void Should_FindFastPlaceForAgent_When_AddingNewPlayers()
//        {
//            int height = 100;
//            int width = 100;
//            int maxNumberOfPlayers = height * width;
//            GameMasterMock gm = new GameMasterMock(width, height, 1, 1, maxNumberOfPlayers);
//            Team team = Team.Red;
//            for (int i = 0; i < maxNumberOfPlayers; i++)
//            {
//                if (i >= maxNumberOfPlayers / 2) team = Team.Blue;
//                gm.AddNewPlayer(team);
//            }
//            for(int i = 0; i < width; i++)
//            {
//                for(int j = 0; j < height; j++)
//                {
//                    Assert.AreNotEqual(-1, gm.Map.GetTile(i, j).AgentId);
//                }
//            }
//        }

//        [Test]
//        public void Should_UpdateMap_When_AddingNewPlayer()
//        {
//            int maxNumberOfPlayers = 2;
//            GameMasterMock gm = new GameMasterMock(5, 5, 1, 1, maxNumberOfPlayers);
//            gm.AddNewPlayer(Team.Red);
//            gm.AddNewPlayer(Team.Blue);

//            Point redAgentPosition = gm.AgentsPositions[0];
//            Point blueAgentPosition = gm.AgentsPositions[1];

//            Assert.AreEqual(0, gm.Map.GetTile(redAgentPosition.X, redAgentPosition.Y).AgentId);
//            Assert.AreEqual(1, gm.Map.GetTile(blueAgentPosition.X, blueAgentPosition.Y).AgentId);

//            for(int i = 0; i < gm.Map.Width; i++)
//            {
//                for(int j = 0; j < gm.Map.Height; j++)
//                {
//                    if(i != redAgentPosition.X && i != blueAgentPosition.X && j != redAgentPosition.Y && j != blueAgentPosition.Y)
//                    {
//                        Assert.AreEqual(-1, gm.Map.GetTile(i, j).AgentId);
//                    }
//                }
//            }
//        }
//    }
//}
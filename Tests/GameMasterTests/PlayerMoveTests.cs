//using NUnit.Framework;
//using ProjectGame;
//using ProjectGame.Logic;

//namespace Tests.GameMasterTests
//{
//    class PlayerMoveTests: GameMasterTestsBase
//    {
//        [Test]
//        public void Should_MoveAgent_When_RequestedMoveIsAllowed()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);
            
//            Assert.AreEqual(true, gm.PlayerMove(Direction.Down, agentId));
//        }

//        [Test]
//        public void Should_CorrectlyUpdateMap_When_MovingAgent()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            Assert.AreEqual(true, gm.PlayerMove(Direction.Down, agentId));
//            Assert.AreEqual(-1, gm.Map.GetTile(x, y).AgentId);
//            y += 1;
//            Assert.AreEqual(x, gm.AgentsPositions[agentId].X);
//            Assert.AreEqual(y, gm.AgentsPositions[agentId].Y);
//            Assert.AreEqual(agentId, gm.Map.GetTile(x, y).AgentId);
            
//            Assert.AreEqual(true, gm.PlayerMove(Direction.Down, agentId));
//            Assert.AreEqual(-1, gm.Map.GetTile(x, y).AgentId);
//            y += 1;
//            Assert.AreEqual(x, gm.AgentsPositions[agentId].X);
//            Assert.AreEqual(y, gm.AgentsPositions[agentId].Y);
//            Assert.AreEqual(agentId, gm.Map.GetTile(x, y).AgentId);
            
//            Assert.AreEqual(true, gm.PlayerMove(Direction.Right, agentId));
//            Assert.AreEqual(-1, gm.Map.GetTile(x, y).AgentId);
//            x += 1;
//            Assert.AreEqual(x, gm.AgentsPositions[agentId].X);
//            Assert.AreEqual(y, gm.AgentsPositions[agentId].Y);
//            Assert.AreEqual(agentId, gm.Map.GetTile(x, y).AgentId);
            
//            Assert.AreEqual(true, gm.PlayerMove(Direction.Up, agentId));
//            Assert.AreEqual(-1, gm.Map.GetTile(x, y).AgentId);
//            y -= 1;
//            Assert.AreEqual(x, gm.AgentsPositions[agentId].X);
//            Assert.AreEqual(y, gm.AgentsPositions[agentId].Y);
//            Assert.AreEqual(agentId, gm.Map.GetTile(x, y).AgentId);
            
//            Assert.AreEqual(true, gm.PlayerMove(Direction.Left, agentId));
//            Assert.AreEqual(-1, gm.Map.GetTile(x, y).AgentId);
//            x -= 1;
//            Assert.AreEqual(x, gm.AgentsPositions[agentId].X);
//            Assert.AreEqual(y, gm.AgentsPositions[agentId].Y);
//            Assert.AreEqual(agentId, gm.Map.GetTile(x, y).AgentId);
//        }

//        [Test]
//        public void Should_RejectMoveAgentRequest_When_GoingOutsideBoard()
//        {
//            int maxNumberOfPlayers = 2;
//            int[] x = new int[2] { 0, gm.Map.Width - 1 };
//            int[] y = new int[2] { 0, gm.Map.Height - 1 };
//            int agentUpLeftId = 0;
//            int agentDownRightId = 1;
//            AddAgentsToGame(maxNumberOfPlayers, x, y);

//            Assert.AreEqual(false, gm.PlayerMove(Direction.Up, agentUpLeftId));
//            Assert.AreEqual(false, gm.PlayerMove(Direction.Left, agentUpLeftId));

//            Assert.AreEqual(false, gm.PlayerMove(Direction.Down, agentDownRightId));
//            Assert.AreEqual(false, gm.PlayerMove(Direction.Right, agentDownRightId));

//            for(int i = 0; i < maxNumberOfPlayers; i++)
//            {
//                Assert.AreEqual(x[i], gm.AgentsPositions[i].X);
//                Assert.AreEqual(y[i], gm.AgentsPositions[i].Y);
//                Assert.AreEqual(i, gm.Map.GetTile(x[i], y[i]).AgentId);
//            }
//        }

//        [Test]
//        public void Should_NotRejectMoveAgentRequest_When_CollidingWithPiece()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);
//            gm.Map.PutPieceOnMap(x + 1, y);

//            Assert.AreEqual(true, gm.PlayerMove(Direction.Right, agentId));
//        }

//        [Test]
//        public void Should_RejectMoveAgentRequest_When_CollidingWithOtherAgent()
//        {
//            int maxNumberOfPlayers = 2;
//            int[] x = new int[2] { 0, 1 };
//            int[] y = new int[2] { 0, 0 };
//            int agentLeftId = 0;
//            int agentRightId = 1;
//            AddAgentsToGame(maxNumberOfPlayers, x, y);

//            Assert.AreEqual(false, gm.PlayerMove(Direction.Right, agentLeftId));
//            Assert.AreEqual(false, gm.PlayerMove(Direction.Left, agentRightId));

//            for (int i = 0; i < maxNumberOfPlayers; i++)
//            {
//                Assert.AreEqual(x[i], gm.AgentsPositions[i].X);
//                Assert.AreEqual(y[i], gm.AgentsPositions[i].Y);
//                Assert.AreEqual(i, gm.Map.GetTile(x[i], y[i]).AgentId);
//            }
//        }
//    }
//}
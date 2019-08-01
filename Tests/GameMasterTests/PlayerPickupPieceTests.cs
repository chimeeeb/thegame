//using NUnit.Framework;
//using ProjectGame;
//using ProjectGame.Items;
//using ProjectGame.Logic;

//namespace Tests.GameMasterTests
//{
//    class PlayerPickupPieceTests: GameMasterTestsBase
//    {
//        [Test]
//        public void Should_AllowPickupPieceAgentRequest_When_AgentHasNoPiece_And_TileContainsPiece()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(x, y);
            
//            Assert.AreEqual(true, gm.PlayerPickUpPiece(agentId));
//        }

//        [Test]
//        public void Should_CorrectlyPickupPiece_When_Allowed()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(x, y);
//            Piece piece = gm.Map.GetTile(x, y).Piece;
//            Assert.AreNotEqual(piece, null);
            
//            Assert.AreEqual(true, gm.PlayerPickUpPiece(agentId));
//            Assert.AreEqual(null, gm.Map.GetTile(x, y).Piece);
//            Assert.AreEqual(true, gm.AgentIdsToPieces.ContainsKey(agentId));
//            Assert.AreEqual(piece, gm.AgentIdsToPieces[agentId]);
//        }

//        [Test]
//        public void Should_RejectPickupPieceAgentRequest_When_AgentHasPiece()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(x, y);
//            Piece piece1 = gm.Map.GetTile(x, y).Piece;
//            Assert.AreNotEqual(null, piece1);

//            gm.Map.PutPieceOnMap(x + 1, y);
//            Piece piece2 = gm.Map.GetTile(x + 1, y).Piece;
//            Assert.AreNotEqual(null, piece2);

//            Assert.AreEqual(true, gm.PlayerPickUpPiece(agentId));
//            Assert.AreEqual(null, gm.Map.GetTile(x, y).Piece);
//            Assert.AreEqual(true, gm.AgentIdsToPieces.ContainsKey(agentId));
//            Assert.AreEqual(piece1, gm.AgentIdsToPieces[agentId]);

//            Assert.AreEqual(true, gm.PlayerMove(Direction.Right, agentId));
            
//            Assert.AreEqual(false, gm.PlayerPickUpPiece(agentId));
//        }

//        [Test]
//        public void Should_RejectPickupPieceAgentRequest_When_TileContainsNoPiece()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            Assert.AreEqual(false, gm.PlayerPickUpPiece(agentId));
//        }
//    }
//}
//using NUnit.Framework;
//using ProjectGame;
//using ProjectGame.Items;

//namespace Tests.GameMasterTests
//{
//    class PlayerDestroyPieceTests: GameMasterTestsBase
//    {
//        [Test]
//        public void Should_DestroyPiece_WhenAgentHasPiece()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(x, y, true);

//            gm.PlayerPickUpPiece(agentId);
//            Assert.AreEqual(true, gm.PlayerDestroyPiece(agentId));
//            Assert.AreEqual(null, gm.Map.GetTile(x, y).Piece);
//            Assert.AreEqual(false, gm.AgentIdsToPieces.ContainsKey(agentId));
//        }

//        [Test]
//        public void Should_RejectDestroyPieceAgentRequest_When_AgentHasNoPiece()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(x, y, true);
//            Piece piece = gm.Map.GetTile(x, y).Piece;

//            Assert.AreEqual(false, gm.PlayerDestroyPiece(agentId));
//            Assert.AreEqual(piece, gm.Map.GetTile(x, y).Piece);
//        }
//    }
//}
//using NUnit.Framework;
//using ProjectGame;
//using ProjectGame.Communication;
//using ProjectGame.Items;

//namespace Tests.GameMasterTests.PlayerPutPieceTests
//{
//    partial class PlayerPutPieceTests : GameMasterTestsBase
//    {
//        [Test]
//        public void Should_RejectPutPieceAgentRequest_When_AgentHasNoPiece()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            (bool isAllowed, Result result) = gm.PlayerPutPiece(agentId);

//            Assert.AreEqual(false, isAllowed);
//            Assert.AreEqual(Result.NoInformation, result);
//            Assert.AreEqual(null, gm.Map.GetTile(x, y).Piece);
//        }

//        [Test]
//        public void Should_RejectPutPieceAgentRequest_When_TileIsTaskTile_And_TileContainsPiece()
//        {
//            int x = 2;
//            int y = 2;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(x, y);
//            gm.PlayerPickUpPiece(agentId);
//            gm.Map.PutPieceOnMap(x, y);
//            Piece piece = gm.Map.GetTile(x, y).Piece;

//            (bool isAllowed, Result result) = gm.PlayerPutPiece(agentId);

//            Assert.AreEqual(false, isAllowed);
//            Assert.AreEqual(Result.NoInformation, result);
//            Assert.AreEqual(true, gm.AgentIdsToPieces.ContainsKey(agentId));
//            Assert.AreEqual(Tile.TileType.Task, gm.Map.GetTile(x, y).Type);
//            Assert.AreEqual(piece, gm.Map.GetTile(x, y).Piece);
//        }
//    }
//}
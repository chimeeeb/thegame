//using NUnit.Framework;
//using ProjectGame;
//using ProjectGame.Communication;
//using ProjectGame.Items;

//namespace Tests.GameMasterTests.PlayerPutPieceTests
//{
//    partial class PlayerPutPieceTests : GameMasterTestsBase
//    {

//        [Test]
//        public void Should_CorrectlyPutPieceOnGoal_When_AgentHasShamPiece()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(x, y, false);
//            gm.PlayerPickUpPiece(agentId);

//            (bool isAllowed, Result result) = gm.PlayerPutPiece(agentId);

//            Assert.AreEqual(true, isAllowed);
//            Assert.AreEqual(false, gm.AgentIdsToPieces.ContainsKey(agentId));
//            Assert.AreEqual(Result.NoInformation, result);
//            Assert.AreEqual(Tile.TileType.Goal, gm.Map.GetTile(x, y).Type);
//            Assert.AreEqual(null, gm.Map.GetTile(x, y).Piece);
//        }

//        [Test]
//        public void Should_CorrectlyPutPieceOnNonGoal_When_AgentHasShamPiece()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.SetGoalTile(x, y, Tile.TileType.NoGoal);
//            gm.Map.PutPieceOnMap(x, y, false);
//            gm.PlayerPickUpPiece(agentId);

//            (bool isAllowed, Result result) = gm.PlayerPutPiece(agentId);

//            Assert.AreEqual(true, isAllowed);
//            Assert.AreEqual(false, gm.AgentIdsToPieces.ContainsKey(agentId));
//            Assert.AreEqual(Result.NoInformation, result);
//            Assert.AreEqual(Tile.TileType.NoGoal, gm.Map.GetTile(x, y).Type);
//            Assert.AreEqual(null, gm.Map.GetTile(x, y).Piece);
//        }


//        [Test]
//        public void Should_CorrectlyPutPieceOnTaskTile_When_AgentHasShamPiece()
//        {
//            int x = 2;
//            int y = 2;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(x, y, false);
//            Piece piece = gm.Map.GetTile(x, y).Piece;
//            gm.PlayerPickUpPiece(agentId);

//            (bool isAllowed, Result result) = gm.PlayerPutPiece(agentId);

//            Assert.AreEqual(true, isAllowed);
//            Assert.AreEqual(false, gm.AgentIdsToPieces.ContainsKey(agentId));
//            Assert.AreEqual(Result.NoInformation, result);
//            Assert.AreEqual(Tile.TileType.Task, gm.Map.GetTile(x, y).Type);
//            Assert.AreEqual(piece, gm.Map.GetTile(x, y).Piece);
//        }

//    }
//}
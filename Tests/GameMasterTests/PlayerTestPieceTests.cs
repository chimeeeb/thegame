//using NUnit.Framework;
//using ProjectGame;
//using ProjectGame.Logic;

//namespace Tests.GameMasterTests
//{
//    class PlayerTestPieceTests: GameMasterTestsBase
//    {
//        [Test]
//        public void Should_ReturnReal_When_PieceIsReal()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(x, y, true);
//            gm.PlayerPickUpPiece(agentId);

//            Assert.AreEqual(PieceStatus.Real, gm.PlayerTestPiece(agentId));
//        }

//        [Test]
//        public void Should_ReturnSham_When_PieceIsSham()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(x, y, false);
//            gm.PlayerPickUpPiece(agentId);

//            Assert.AreEqual(PieceStatus.Sham, gm.PlayerTestPiece(agentId));
//        }

//        [Test]
//        public void Should_ReturnUndefined_When_AgentHasNoPiece()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            Assert.AreEqual(PieceStatus.Unidentified, gm.PlayerTestPiece(agentId));
//        }
//    }
//}
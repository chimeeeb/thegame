//using NUnit.Framework;
//using ProjectGame;

//namespace Tests.GameMasterTests.PlayerDiscoveryTests
//{
//    partial class PlayerDiscoveryTests: GameMasterTestsBase
//    {
//        [Test]
//        public void Should_HaveCorrectDistances_When_AtBoardLeftEdge()
//        {
//            int x = 0;
//            int y = 2;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(2, 2);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                0, 3, 2,
//                0, 2, 1,
//                0, 3, 2
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }

//        [Test]
//        public void Should_HaveCorrectDistances_When_AtBoardRightEdge()
//        {
//            int x = 4;
//            int y = 3;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(2, 3);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                2, 3, 0,
//                1, 2, 0,
//                2, 3, 0
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }

//        [Test]
//        public void Should_HaveCorrectDistances_When_AtBoardUpperEdge()
//        {
//            int x = 3;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(3, 3);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                0, 0, 0,
//                4, 3, 4,
//                3, 2, 3
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }

//        [Test]
//        public void Should_HaveCorrectDistances_When_AtBoardLowerEdge()
//        {
//            int x = 3;
//            int y = 4;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(3, 2);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                2, 1, 2,
//                3, 2, 3,
//                0, 0, 0
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }
//    }
//}
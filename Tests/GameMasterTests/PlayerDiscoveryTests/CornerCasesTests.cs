//using NUnit.Framework;
//using ProjectGame;

//namespace Tests.GameMasterTests.PlayerDiscoveryTests
//{
//    partial class PlayerDiscoveryTests: GameMasterTestsBase
//    {
//        [Test]
//        public void Should_HaveCorrectDistances_When_AtBoardLeftUpperCorner()
//        {
//            int x = 0;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(3, 3);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                0, 0, 0,
//                0, 6, 5,
//                0, 5, 4
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }

//        [Test]
//        public void Should_HaveCorrectDistances_When_AtBoardRightUpperCorner()
//        {
//            int x = 4;
//            int y = 0;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(2, 2);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                0, 0, 0,
//                3, 4, 0,
//                2, 3, 0
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }

//        [Test]
//        public void Should_HaveCorrectDistances_When_AtBoardLeftLowerCorner()
//        {
//            int x = 0;
//            int y = 4;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(2, 2);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                0, 3, 2,
//                0, 4, 3,
//                0, 0, 0
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }

//        [Test]
//        public void Should_HaveCorrectDistances_When_AtBoardRightLowerCorner()
//        {
//            int x = 4;
//            int y = 4;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(2, 2);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                2, 3, 0,
//                3, 4, 0,
//                0, 0, 0
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }
//    }
//}
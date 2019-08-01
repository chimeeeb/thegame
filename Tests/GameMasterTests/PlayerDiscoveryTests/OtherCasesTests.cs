//using NUnit.Framework;
//using ProjectGame;

//namespace Tests.GameMasterTests.PlayerDiscoveryTests
//{
//    partial class PlayerDiscoveryTests: GameMasterTestsBase
//    {
//        [Test]
//        public void Should_HaveCorrectDistances_When_NotAtBoardEdge()
//        {
//            int x = 3;
//            int y = 3;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(1, 1);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                2, 3, 4,
//                3, 4, 5,
//                4, 5, 6
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }

//        [Test]
//        public void Should_HaveCorrectDistances_When_PieceWithinSquare()
//        {
//            int x = 1;
//            int y = 1;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(2, 2);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                4, 3, 2,
//                3, 2, 1,
//                2, 1, 0
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }

//        [Test]
//        public void Should_HaveCorrectDistances_When_OnPiece()
//        {
//            int x = 1;
//            int y = 1;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(1, 1);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                2, 1, 2,
//                1, 0, 1,
//                2, 1, 2
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }

//        [Test]
//        public void Should_HaveCorrectDistances_When_NoPiece()
//        {
//            int x = 1;
//            int y = 1;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                int.MaxValue, int.MaxValue, int.MaxValue,
//                int.MaxValue, int.MaxValue, int.MaxValue,
//                int.MaxValue, int.MaxValue, int.MaxValue
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }

//        [Test]
//        public void Should_HaveCorrectDistances_When_MultiplePieces()
//        {
//            int x = 2;
//            int y = 2;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);

//            gm.Map.PutPieceOnMap(1, 1);
//            gm.Map.PutPieceOnMap(3, 3);

//            int[] distances = gm.PlayerDiscovery(agentId);
//            int[] expectedDistances = new int[9]
//            {
//                0, 1, 2,
//                1, 2, 1,
//                2, 1, 0
//            };

//            for (int i = 0; i < distances.Length; i++)
//            {
//                Assert.AreEqual(expectedDistances[i], distances[i]);
//            }
//        }
//    }
//}
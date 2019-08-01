//using System;
//using System.Collections.Generic;
//using System.Linq;
//using NUnit.Framework;
//using ProjectGame;
//using ProjectGame.Items;

//namespace Tests
//{
//    public class MapTests
//    {
//        [Test]
//        public void UpdateAfterDiscoveryTests([Values(0, 4, 8)] int discoveryTileX, [Values(0, 4, 8)] int discoveryTileY)
//        {
//            var width = 8;
//            var height = 8;
//            var goalArea = 2;

//            var agentMap = new AgentMap(width, height, goalArea);
            
//            var timeStamp = DateTime.Now;

//            agentMap.UpdateAfterDiscovery(discoveryTileX, discoveryTileY, Enumerable.Range(1, 9).ToArray(), timeStamp);

//            void AssertDiscoveredTile((int X, int Y) tile, int expectedDistanceToPiece, DateTime expectedTimeStamp)
//            {
//                Assert.AreEqual(agentMap[tile.X, tile.Y].DistanceToPiece, expectedDistanceToPiece);
//                Assert.AreEqual(agentMap[tile.X, tile.Y].Timestamp, expectedTimeStamp);
//            }

//            AssertDiscoveredTile((discoveryTileX - 1, discoveryTileY - 1), 0, timeStamp);
//            AssertDiscoveredTile((discoveryTileX, discoveryTileY - 1), 0, timeStamp);
//            AssertDiscoveredTile((discoveryTileX + 1, discoveryTileY - 1), 0, timeStamp);
//            AssertDiscoveredTile((discoveryTileX - 1, discoveryTileY), 0, timeStamp);
//            AssertDiscoveredTile((discoveryTileX, discoveryTileY), 0, timeStamp);
//            AssertDiscoveredTile((discoveryTileX + 1, discoveryTileY), 0, timeStamp);
//            AssertDiscoveredTile((discoveryTileX - 1, discoveryTileY + 1), 0, timeStamp);
//            AssertDiscoveredTile((discoveryTileX, discoveryTileY + 1), 0, timeStamp);
//            AssertDiscoveredTile((discoveryTileX + 1, discoveryTileY + 1), 0, timeStamp);
//        }

//        [Test]
//        public void GenerateGoalTilesTests()
//        {
//            var width = 8;
//            var height = 8;
//            var goalArea = 2;

//            var gmMap = new GameMasterMap(width, height, goalArea);
//            gmMap.GenerateGoalTiles();

//            Assert.AreEqual(SearchTilesFor(gmMap, tile => tile.Type == TileType.Goal),
//                Parameters.NumberOfGoalsPerTeam * 2);
//            Assert.AreEqual(SearchTilesFor(gmMap, tile => tile.Type == TileType.NoGoal),
//                (width * goalArea - Parameters.NumberOfGoalsPerTeam) * 2);
//        }

//        [Test]
//        public void GeneratePieceTests()
//        {
//            var width = 8;
//            var height = 8;
//            var goalArea = 2;

//            var gmMap = new GameMasterMap(width, height, goalArea);
//            gmMap.GeneratePiece();
//            Assert.True(CheckIfTileExists(gmMap, tile => tile.Piece != null));

//            int maxNumbersOfTiles = width * (height - goalArea * 2);

//            for (int i = 2; i <= maxNumbersOfTiles; i++)
//            {
//                gmMap.GeneratePiece();
//                Assert.AreEqual(SearchTilesFor(gmMap, tile => tile.Piece != null).Count(), i);
//            }

//            gmMap.GeneratePiece();
//            Assert.AreEqual(SearchTilesFor(gmMap, tile => tile.Piece != null).Count(), maxNumbersOfTiles);
//        }

//        [Test]
//        public void DistanceToPieceTests()
//        {
//            var width = 8;
//            var height = 8;
//            var goalArea = 2;

//            var gmMap = new GameMasterMap(width, height, goalArea);
//            gmMap.GeneratePiece();
//            gmMap.GeneratePiece();
//            gmMap.GeneratePiece();

//            var referenceX = 3;
//            var referenceY = 3;

//            var pieces = SearchTilesFor(gmMap, tile => tile.Piece != null).ToList();

//            void AssertDistanceToPiece((int X, int Y) tile)
//            {
//                var nearestDistance = int.MaxValue;
//                var dist1 = Math.Abs(tile.X - pieces[0].x) + Math.Abs(tile.Y - pieces[0].y);
//                if (nearestDistance < dist1) nearestDistance = dist1;
//                var dist2 = Math.Abs(tile.X - pieces[1].x) + Math.Abs(tile.Y - pieces[1].y);
//                if (nearestDistance < dist2) nearestDistance = dist2;
//                var dist3 = Math.Abs(tile.X - pieces[2].x) + Math.Abs(tile.Y - pieces[2].y);
//                if (nearestDistance < dist3) nearestDistance = dist3;

//                Assert.AreEqual(nearestDistance, gmMap.DistanceToPiece(tile.X, tile.Y));
//            }

//            AssertDistanceToPiece((referenceX - 1, referenceY - 1));
//            AssertDistanceToPiece((referenceX, referenceY - 1));
//            AssertDistanceToPiece((referenceX + 1, referenceY - 1));
//            AssertDistanceToPiece((referenceX - 1, referenceY));
//            AssertDistanceToPiece((referenceX, referenceY));
//            AssertDistanceToPiece((referenceX + 1, referenceY));
//            AssertDistanceToPiece((referenceX - 1, referenceY + 1));
//            AssertDistanceToPiece((referenceX, referenceY + 1));
//            AssertDistanceToPiece((referenceX + 1, referenceY + 1));
//        }

//        private IEnumerable<(int x, int y)> SearchTilesFor<T>(Map<T> map, Func<T, bool> condition) where T : Tile, new()
//        {
//            for (int i = 0; i < map.Width; i++)
//            for (int j = 0; j < map.Height; j++)
//                if (condition(map[i, j]))
//                    yield return (i, j);
//        }

//        private bool CheckIfTileExists<T>(Map<T> map, Func<T, bool> condition) where T : Tile, new() =>
//            SearchTilesFor(map, condition).Any();
//    }
//}
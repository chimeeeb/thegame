//using NUnit.Framework;
//using ProjectGame;
//using ProjectGame.Communication;
//using ProjectGame.Items;
//using ProjectGame.Logic;

//namespace Tests.GameMasterTests
//{
//    class EndGameTests : GameMasterTestsBase
//    {
//        [Test]
//        public void Should_EndGame_When_TeamUnlockedLastGoal()
//        {
//            int x = 2;
//            int y = 2;
//            int agentId = 0;
//            AddSingleAgentToGame(x, y);
//            // By default all fields in goal area contain goals
//            UnlockGoals(gm.Map, Team.Red, 2, 0);
//            gm.Map.PutPieceOnMap(x, y, true);

//            gm.PlayerPickUpPiece(agentId);
//            gm.PlayerMove(Direction.Up, agentId);
//            gm.PlayerMove(Direction.Up, agentId);
//            (bool isAllowed, Result result) = gm.PlayWithPutPiece(agentId);

//            Assert.AreEqual(true, isAllowed);
//            Assert.AreEqual(Result.GoalCompleted, result);
//            Assert.AreEqual(Team.Red, gm.WinningTeam);
//            Assert.AreEqual(Parameters.NumberOfGoalsPerTeam, gm.Map.NumberOfTeamRedGoals);
//        }

//        /// <summary>
//        /// A helper function to mock unlocking almost all goals of given team, except for one.
//        /// </summary>
//        /// <param name="map">Game Master map to modify</param>
//        /// <param name="xOfGoalToRemain">Row of the goal field to contain non-discovered goal</param>
//        /// <param name="yOfGoalToRemain">Column of the goal field to contain non-discovered goal</param>
//        /// <remarks>Assumes that red team has upper goal area.</remarks>
//        private void UnlockGoals(GameMasterMap map, Team team, int xOfGoalToRemain, int yOfGoalToRemain)
//        {
//            for (int j = 0; j < map.GoalAreaHeight; j++)
//            {
//                int row = team == Team.Red ? j : map.Height - j - 1;
//                for (int i = 0; i < map.Width; i++)
//                {
//                    if (i != xOfGoalToRemain || row != yOfGoalToRemain)
//                    {
//                        map.SetGoalTile(i, row, Tile.TileType.DiscoveredGoal);
//                        map.UpdateTeamResult(row);
//                    }
//                }
//            }
//        }
//    }
//}
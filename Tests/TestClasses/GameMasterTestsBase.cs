//using NUnit.Framework;
//using ProjectGame;
//using ProjectGame.Logic;
//using Tests.TestClasses;

//namespace Tests.GameMasterTests
//{
//    /// <summary>
//    /// Base class for all Game Master tests.
//    /// Contains common methods used by different GM test classes.
//    /// </summary>
//    class GameMasterTestsBase
//    {
//        protected GameMasterMock gm;

//        /// <summary>
//        /// A wrapper function that prepares GM and Agents for testing.
//        /// </summary>
//        /// <param name="maxNumberOfPlayers">Number of Agents to generate and connect to the game.</param>
//        /// <param name="x">X- positions of Agents on the board.</param>
//        /// <param name="y">Y - positions of Agents on the board.</param>
//        /// <param name="goalsPerTeam">Number of goal tiles per team, by default all tiles in goal area are goal tiles.</param>
//        /// <returns>Array of Agents connected to the game.</returns>
//        public void AddAgentsToGame(int maxNumberOfPlayers, int[] x, int[] y, int goalsPerTeam = 5)
//        {
//            gm = new GameMasterMock(5, 5, 1, 1, maxNumberOfPlayers, goalsPerTeam);
//            Team team = Team.Red;
//            for (int i = 0; i < maxNumberOfPlayers; i++)
//            {
//                if (i >= maxNumberOfPlayers / 2) team = Team.Blue;
//                gm.AddNewPlayer(team, x[i], y[i]);
//            }
//        }

//        /// <summary>
//        /// A wrapper function that prepares GM and Agent for testing.
//        /// </summary>
//        /// <param name="x">X- position of the Agent on the board.</param>
//        /// <param name="y">Y - position of the Agent on the board.</param>
//        /// <param name="goalsPerTeam">Number of goal tiles per team, by default all tiles in goal area are goal tiles.</param>
//        /// <returns>Agent connected to the game.</returns>
//        public void AddSingleAgentToGame(int x, int y, int goalsPerTeam = 5)
//        {
//            gm = new GameMasterMock(5, 5, 1, 1, 2, goalsPerTeam);
//            gm.AddNewPlayer(Team.Red, x, y);
//        }
//    }
//}

using GameLibrary.Enum;
using GameLibrary.Interface;
using System;

namespace GameLibrary.Strategies
{
    public class TotallyRandomStrategy : StrategyBase
    {
        public TotallyRandomStrategy(Team team, int mapHeight, int mapWidth, int mapGoalAreaHeight, int[] agentsIdFromTeam, int leaderId) : base(team, mapHeight, mapWidth, mapGoalAreaHeight, agentsIdFromTeam, leaderId) { }

        public override Actions UseStrategy(ITile position, int distanceToPiece)
        {
            Random rand = new Random();
            switch(rand.Next(10))
            {
                case 0:
                    return Actions.MoveUp;
                case 1:
                    return Actions.MoveDown;
                case 2:
                    return Actions.MoveLeft;
                case 3:
                    return Actions.MoveRight;
                case 4:
                    return Actions.DestroyPiece;
                case 5:
                    return Actions.PickPiece;
                case 6:
                    return Actions.PutPiece;
                case 7:
                    return Actions.TestPiece;
                case 8:
                    if (AgentsIdFromTeam.Length > 0 && !WaitingForExchangeAnswer)
                        return Actions.InfoExchange;
                    return Actions.Discovery;
                default:
                    return Actions.Discovery;
            }
        }
    }
}

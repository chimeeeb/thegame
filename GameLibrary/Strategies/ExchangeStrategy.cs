using GameLibrary.Enum;
using GameLibrary.Interface;

namespace GameLibrary.Strategies
{
    public class ExchangeStrategy : StrategyBase
    {
        public ExchangeStrategy(Team team, int mapHeight, int mapWidth, int mapGoalAreaHeight, int[] agentsIdFromTeam, int leaderId) : base(team, mapHeight, mapWidth, mapGoalAreaHeight, agentsIdFromTeam, leaderId) { }

        public override Actions UseStrategy(ITile position, int distanceToPiece)
        {
            if (AgentsIdFromTeam.Length > 0 && !WaitingForExchangeAnswer)
                return Actions.InfoExchange;
            return Actions.Discovery;
        }
    }
}

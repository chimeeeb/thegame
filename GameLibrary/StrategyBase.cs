using GameLibrary.Enum;
using GameLibrary.Interface;
using GameLibrary.Strategies;
using System;

namespace GameLibrary
{
    /// <summary>
    /// Base class for all game strategies.
    /// </summary>
    public abstract class StrategyBase : IStrategy
    {        
        /// <summary>
        /// Represents information about the piece hold.
        /// </summary>
        public PieceStatus Piece { get; set; }
        /// <summary>
        /// Is updated after each Agent action to contain the amount of time the Agent should wait before requesting the next action.
        /// </summary>
        public TimeSpan WaitingTime { get; set; }
        /// <summary>
        /// Agent stores number of times he couldn't move in given direction.
        /// </summary>
        public int CouldntMove { get; set; }
        /// <summary>
        /// Agent remembers if his last move action made him get closer to one of the pieces.
        /// </summary>
        public bool IsGettingCloser { get; set; }
        /// <summary>
        /// Team the Agent belongs to.
        /// </summary>
        public Team Team { get; set; }
        /// <summary>
        /// ID of the leader of this Agent team
        /// </summary>
        public int LeaderId;
        /// <summary>
        /// IDs of teammates
        /// </summary>
        public int[] AgentsIdFromTeam;
        /// <summary>
        /// Map dimensions
        /// </summary>
        public int MapHeight { get; set; }
        public int MapWidth { get; set; }
        public int MapGoalAreaHeight { get; set; }
        /// <summary>
        /// Whether the Agent is currently involved in an information exchange.
        /// </summary>
        public bool WaitingForExchangeAnswer { get; set; }

        /// <summary>
        /// Creates a basic abstract strategy.
        /// </summary>
        public StrategyBase(Team team, int mapHeight, int mapWidth, int mapGoalAreaHeight, int[] agentsIdFromTeam, int leaderId)
        {
            Team = team;
            Piece = PieceStatus.None;
            WaitingTime = TimeSpan.Zero;
            CouldntMove = 0;
            IsGettingCloser = false;
            MapHeight = mapHeight;
            MapGoalAreaHeight = mapGoalAreaHeight;
            MapWidth = mapWidth;
            AgentsIdFromTeam = agentsIdFromTeam;
            LeaderId = leaderId;
            WaitingForExchangeAnswer = false;
        }

        /// <summary>
        /// Abstract function that finds the best action for the Agent.
        /// </summary>
        /// <param name="position">Tile which the Agent currently occupies.</param>
        /// <param name="distanceToPiece">Current distance to the closest piece.</param>
        /// <returns></returns>
        public abstract Actions UseStrategy(ITile position, int distanceToPiece);

        /// <summary>
        /// Function that returns ID of the Agent with whom the information should be exchanged.
        /// </summary>
        /// <returns></returns>
        public virtual int ExchangeInfoTarget()
        {
            Random rand = new Random();
            return AgentsIdFromTeam[rand.Next(0, AgentsIdFromTeam.Length)];
        }

        /// <summary>
        /// A method to get a concrete strategy object based on enum type choice (coming from Agent settings).
        /// <returns>Strategy object of appropriate type.</returns>
        public static StrategyBase GetStrategy(StrategyType strategyType, Team team, int mapHeight, int mapWidth, int mapGoalAreaHeight, int[] agentsIdFromTeam, int leaderId)
        {
            switch(strategyType)
            {
                case StrategyType.Normal:
                    return new NormalStrategy(team, mapHeight, mapWidth, mapGoalAreaHeight, agentsIdFromTeam, leaderId);
                case StrategyType.Random:
                    return new TotallyRandomStrategy(team, mapHeight, mapWidth, mapGoalAreaHeight, agentsIdFromTeam, leaderId);
                case StrategyType.Discoverer:
                    return new DiscovererStrategy(team, mapHeight, mapWidth, mapGoalAreaHeight, agentsIdFromTeam, leaderId);
                case StrategyType.Exchange:
                    return new ExchangeStrategy(team, mapHeight, mapWidth, mapGoalAreaHeight, agentsIdFromTeam, leaderId);
                case StrategyType.Superior:
                    return new SuperiorStrategy(team, mapHeight, mapWidth, mapGoalAreaHeight, agentsIdFromTeam, leaderId);
            }
            return new NormalStrategy(team, mapHeight, mapWidth, mapGoalAreaHeight, agentsIdFromTeam, leaderId);
        }
    }
}

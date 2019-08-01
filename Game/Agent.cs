using System;
using GameLibrary;
using GameLibrary.Enum;

namespace Game
{
    public class Agent: AgentBase
    {
        /// <summary>
        /// Point in game time to which agent must wait before being permitted to commit next action.
        /// </summary>
        public TimeSpan WaitingUntilTime { get; set; } = new TimeSpan(0);

        /// <summary>
        /// Piece hold by the Agent.
        /// </summary>
        public Piece Piece { get; set; }
        /// <summary>
        /// Whether the Agent was asked for information exchange by team leader.
        /// </summary>
        public bool CalledByLeader { get; set; }
        /// <summary>
        /// Whether the Agent got disconnected from the game.
        /// </summary>
        public bool Disconnected { get; set; }
        
        public Agent(int id, bool isLeader, Team team, Piece piece = Piece.Null)
        {
            Id = id;
            IsLeader = isLeader;
            Team = team;
            Piece = piece;
            CalledByLeader = false;
            Disconnected = false;
        }
    }
}
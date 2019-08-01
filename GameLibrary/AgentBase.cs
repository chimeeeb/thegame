using GameLibrary.Interface;
using GameLibrary.Enum;

namespace GameLibrary
{
    /// <summary>
    /// Base class to represent common core of GM and Player Agents.
    /// </summary>
    public abstract class AgentBase
    {
        /// <summary>
        /// Unique value identifying each Agent.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// True if the Agent is a Leader.
        /// </summary>
        public bool IsLeader { get; set; }

        /// <summary>
        /// Team the Agent belongs to.
        /// </summary>
        public Team Team { get; set; }

        /// <summary>
        /// The tile the Agent is standing on
        /// </summary>
        public ITile Tile { get; set; }
    }
}
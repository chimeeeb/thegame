using GameLibrary.Enum;
using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// A class to represent the broadcast message sent to all Agents to inform them about the end of the game
    /// </summary>
    public class GameEndedMessage : Message
    {
        /// <summary>
        /// ID of the Agent in a game
        /// </summary>
        [JsonProperty("agentId")]
        public int AgentId { get; set; }
        /// <summary>
        /// Timestamp of the message
        /// </summary>
        [JsonProperty("timestamp")]
        public int GameTimeStamp { get; set; }
        /// <summary>
        /// Tells which team won
        /// </summary>
        [JsonProperty("winningTeam")]
        public Team WinningTeam { get; set; }
    }
}

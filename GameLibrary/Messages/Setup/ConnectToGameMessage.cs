using GameLibrary.Enum;
using Newtonsoft.Json;

namespace GameLibrary.Messages
{

    /// <summary>
    /// A class that represents the message an Agent sends to GameMaster to connect to the game
    /// </summary>
    public class ConnectToGameMessage : Message
    {
        /// <summary>
        /// Indicates whether agent trying to connect to the game wants to be the leader of his team
        /// </summary>
        [JsonProperty("wantToBeLeader")]
        public bool WantToBeLeader { get; set; }

        /// <summary>
        /// The team to which the Agent wants to be assigned, 0 or 1
        /// </summary>
        [JsonProperty("teamId")]
        public Team TeamId { get; set; }

        /// <summary>
        /// Agent id, set by the Communication Server here for the first time
        /// </summary>
        [JsonProperty("agentId")]
        public int AgentId { get; set; }
    }
}

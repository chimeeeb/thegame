using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// A class that represents a message received by an Agent when accepted into the game by GameMaster
    /// </summary>
    public class AcceptedToGameMessage : Message
    {
        /// <summary>
        /// ID of the Agent in a game, assigned by the Communication Server, here passed to agent for the first time.
        /// </summary>
        [JsonProperty("agentId")]
        public int AgentId { get; set; }
  
        /// <summary>
        /// Whether the agent connected successfully.
        /// </summary>
        [JsonProperty("isConnected")]
        public bool IsConnected { get; set; }
    }


}

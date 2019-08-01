using Newtonsoft.Json;

namespace GameLibrary.Messages
{

    /// <summary>
    /// An abstract class to represent GameMaster answer to an Agent.
    /// </summary>
    public abstract class ResultMessage : Message
    {
        /// <summary>
        /// ID of the Agent
        /// </summary>
        [JsonProperty("agentId")]
        public int AgentId { get; set; }

        /// <summary>
        /// Informs agent to what point in GameTime he should wait before requesting new action.
        /// </summary>
        [JsonProperty("waitUntilTime")]
        public int WaitUntilTime { get; set; }

        /// <summary>
        /// Timestamp of the message
        /// </summary>
        [JsonProperty("timestamp")]
        public int GameTimeStamp { get; set; }
    }
}

using Newtonsoft.Json;

namespace GameLibrary.Messages
{

    /// <summary>
    /// An abstract class to represent Agent request sent to GameMaster.
    /// </summary>
    public abstract class RequestMessage : Message
    {
        /// <summary>
        /// ID of the Agent
        /// </summary>
        [JsonProperty("agentId")]
        public int AgentId { get; set; }
    }

}

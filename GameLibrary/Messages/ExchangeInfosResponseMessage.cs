using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Message send by the Agent that had been asked for information exchange.
    /// </summary>
    public class ExchangeInfosResponseMessage : Message
    {
        /// <summary>
        /// ID of the Agent
        /// </summary>
        [JsonProperty("agentId")]
        public int AgentId { get; set; }

        /// <summary>
        /// Agent's id with whom you want to communicate 
        /// </summary>
        [JsonProperty("withAgentId")]
        public int WithAgentId { get; set; }

        /// <summary>
        /// Tells if agent wants to communicate
        /// </summary>
        [JsonProperty("agreement")]
        public bool Agreement { get; set; }

        /// <summary>
        /// Data you want to pass to other agent
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}

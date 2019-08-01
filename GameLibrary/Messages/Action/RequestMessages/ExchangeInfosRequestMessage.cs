using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Message sent by an Agent to initiate information exchange.
    /// </summary>
    public class ExchangeInfosRequestMessage : RequestMessage
    {
        /// <summary>
        /// Agent's id with whom you want to communicate.
        /// </summary>
        [JsonProperty("withAgentId")]
        public int WithAgentId { get; set; }
        /// <summary>
        /// The data you want to pass to other agent.
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}

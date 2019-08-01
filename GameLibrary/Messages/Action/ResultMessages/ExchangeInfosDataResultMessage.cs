using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Message with information exchange data sent by Game Master at the end of exchange info action.
    /// </summary>
    public class ExchangeInfosDataResultMessage : ResultMessage
    {
        /// <summary>
        /// Agent's id with whom you want to communicate.
        /// </summary>
        [JsonProperty("withAgentId")]
        public int WithAgentId { get; set; }

        /// <summary>
        /// Tells if agent wants to communicate.
        /// </summary>
        [JsonProperty("agreement")]
        public bool Agreement { get; set; }

        /// <summary>
        /// Data you want to pass to other agent, empty if agreement is false.
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}

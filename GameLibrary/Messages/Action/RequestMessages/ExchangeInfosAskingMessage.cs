using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Message sent by Game Master to inform chosen Agent about communication request.
    /// </summary>
    public class ExchangeInfosAskingMessage : RequestMessage
    {
        /// <summary>
        /// Id of Agent who wants to communicate with you.
        /// </summary>
        [JsonProperty("withAgentId")]
        public int WithAgentId { get; set; }

        /// <summary>
        /// Timestamp of the message.
        /// </summary>
        [JsonProperty("timestamp")]
        public int GameTimeStamp { get; set; }
    }
}

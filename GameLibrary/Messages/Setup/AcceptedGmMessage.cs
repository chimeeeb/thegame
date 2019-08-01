using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Message indicating wheter Game Master connection attempt was successful.
    /// </summary>
    public class AcceptedGmMessage : Message
    {
        /// <summary>
        /// Informs GM whether the connection attempt was successful
        /// </summary>
        [JsonProperty("isConnected")]
        public bool IsConnected { get; set; }
    }
}

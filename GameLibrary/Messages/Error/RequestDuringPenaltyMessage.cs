using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Message sent by Game Master to an Agent who had sent a request during his penalty time.
    /// </summary>
    public class RequestDuringPenaltyMessage : ErrorMessage
    {

        /// <summary>
        /// Informs agent to what point in GameTime he should wait before requesting new action
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

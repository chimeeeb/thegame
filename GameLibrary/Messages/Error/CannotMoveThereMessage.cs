using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Message sent by GM to an Agent who requested a forbidden move.
    /// </summary>
    public class CannotMoveThereMessage : ErrorMessage
    {
        /// <summary>
        /// Timestamp of the message.
        /// </summary>
        [JsonProperty("timestamp")]
        public int GameTimeStamp { get; set; }
    }
}

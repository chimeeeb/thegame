using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// A class that represents a message received by an Agent after Game Master processed his test piece request.
    /// </summary>
    public class TestPieceResultMessage : ResultMessage
    {
        /// <summary>
        /// Tells whether piece is real or sham.
        /// </summary>
        [JsonProperty("isCorrect")]
        public bool IsReal { get; set; }
    }
}

using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// A class that represents a message received by an Agent after he is moved on the board by GameMaster
    /// </summary>
    public class MoveResultMessage : ResultMessage
    {
        /// <summary>
        /// Information about a distance to a piece
        /// </summary>
        [JsonProperty("closestPiece")]
        public int DistanceToPiece { get; set; }
    }
}

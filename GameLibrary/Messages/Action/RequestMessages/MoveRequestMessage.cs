using GameLibrary.Enum;
using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Message an Agent sends to GameMaster to move on the board.
    /// </summary>
    public class MoveRequestMessage : RequestMessage
    {
        /// <summary>
        /// Direction of movement.
        /// </summary>
        [JsonProperty("moveDirection")]
        public Direction Direction;
    }
}

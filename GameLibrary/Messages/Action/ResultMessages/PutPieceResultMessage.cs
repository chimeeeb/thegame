using GameLibrary.Enum;
using Newtonsoft.Json;

namespace GameLibrary.Messages
{

    /// <summary>
    /// A class that represents a message received by an Agent after GameMaster processed his put piece request.
    /// </summary>
    public class PutPieceResultMessage : ResultMessage
    {
        /// <summary>
        /// Effect of putting the piece
        /// </summary>
        [JsonProperty("result")]
        public Result Effect { get; set; }
    }
}

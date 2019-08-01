using Newtonsoft.Json;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Auxiliary class to represent tile information in DiscoveryResultMessage.
    /// </summary>
    public class JMapTile
    {
        /// <summary>
        /// Map coordinate x
        /// </summary>
        [JsonProperty("x")]
        public int X;
        /// <summary>
        /// Map coordinate y
        /// </summary>
        [JsonProperty("y")]
        public int Y;
        /// <summary>
        /// Distance to closest piece
        /// </summary>
        [JsonProperty("dist")]
        public int Distance;
    }
}

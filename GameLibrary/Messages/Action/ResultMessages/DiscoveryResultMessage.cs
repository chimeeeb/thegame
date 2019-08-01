using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameLibrary.Messages
{
    /// <summary>
    /// A class that represents a message received by an Agent after GameMaster processed his discovery request.
    /// </summary>
    public class DiscoveryResultMessage : ResultMessage
    {
        /// <summary>
        /// Information about discovered distances
        /// </summary>
        [JsonProperty("closestPieces")]
        public List<JMapTile> DiscoveryResults { get; set; }
    }
}

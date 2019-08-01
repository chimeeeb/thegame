using System;
using GameLibrary;
using GameLibrary.Enum;
using Newtonsoft.Json;

namespace Player
{
    /// <summary>
    /// Tile that represents Agent knowledge about a particular board field.
    /// </summary>
    public class Tile : TileBase
    {
        private const int UnknownDistanceToPiece = -1;

        /// <summary>
        /// Stores most recent Agent's information about the Manhattan distance from the AgentTile to the nearest piece. 
        /// Unknown is -1.
        /// </summary>
        /// 
        [JsonIgnore]
        public int DistanceToPiece { get; private set; }

        public Tile()
        {
            DistanceToPiece = UnknownDistanceToPiece;
        }

        public void UpdateTile(DateTime timestamp, int distanceToPiece, TileType? type = null)
        {
            if (type == null)
                base.UpdateTile(timestamp, Type);
            else
                base.UpdateTile(timestamp, type);
            DistanceToPiece = distanceToPiece;
        }
    }
}
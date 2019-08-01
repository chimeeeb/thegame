using System;
using GameLibrary.Enum;
using GameLibrary.Interface;
using Newtonsoft.Json;

namespace GameLibrary
{
    /// <summary>
    /// Base class of a single tile.
    /// </summary>
    public abstract class TileBase : ITile
    {
        protected static readonly DateTime UnknownTimeStamp = DateTime.Now;

        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Stores most recent modification time of the tile.
        /// </summary>
        [JsonIgnore]
        public DateTime Timestamp { get; protected set; }

        /// <summary>
        /// Type of the tile
        /// </summary>
        public TileType Type { get; protected set; }

        protected TileBase()
        {
            Timestamp = UnknownTimeStamp;
            Type = TileType.Unknown;
        }

        protected virtual void UpdateTile(DateTime timestamp, TileType? type = null)
        {
            Timestamp = timestamp;
            if (type.HasValue) Type = type.Value;
        }
    }
}
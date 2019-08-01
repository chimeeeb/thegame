using System;
using GameLibrary.Enum;

namespace GameLibrary.Interface
{
    /// <summary>
    /// Interface of a single map tile.
    /// </summary>
    public interface ITile
    {
        int X { get; set; }
        int Y { get; set; }
        DateTime Timestamp { get; }
        TileType Type { get; }
    }
}
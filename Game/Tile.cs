using System;
using GameLibrary;
using GameLibrary.Enum;

namespace Game
{
    /// <summary>
    /// Tile that represents Game Master knowledge about a particular board field.
    /// </summary>
    public class Tile : TileBase
    {
        private const int UnknownAgentId = -1;

        /// <summary>
        /// Whether there is a piece on the tile and whether it is fake.
        /// </summary>
        public Piece Piece { get; private set; }

        /// <summary>
        /// ID of the Agent that occupies the tile.
        /// </summary>
        /// <remarks>Equal to -1 if no Agent on the tile.</remarks>
        public int AgentId { get; private set; }

        public Tile()
        {
            Piece = Piece.Null;
            AgentId = UnknownAgentId;
        }

        /// <summary>
        /// Modify given tile.
        /// </summary>
        /// <param name="timestamp">New timestamp.</param>
        /// <param name="agentId">New agent ID to be assigned.</param>
        /// <param name="piece">New piece value.</param>
        /// <param name="type">New tile type.</param>
        public void UpdateTile(DateTime timestamp, int agentId = -1, Piece? piece = null, TileType? type = null)
        {
            base.UpdateTile(timestamp, type);
            AgentId = agentId;
            if (piece.HasValue)
                Piece = piece.Value;
        }

        /// <summary>
        /// Modify piece value of the tile.
        /// </summary>
        /// <param name="timestamp">New timestamp.</param>
        /// <param name="piece">New piece value.</param>
        public void UpdateTileAddPiece(DateTime timestamp, Piece? piece)
        {
            Timestamp = timestamp;
            Piece = piece.Value;
        }
    }
}
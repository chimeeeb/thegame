using GameLibrary.Enum;
using System;

namespace GameLibrary.Interface
{
    /// <summary>
    /// Base interface for all game strategies.
    /// </summary>
    public interface IStrategy
    {
        PieceStatus Piece { get; set; }
        TimeSpan WaitingTime { get; set; }
        int CouldntMove { get; set; }
        bool IsGettingCloser { get; set; }
        Team Team { get; set; }
        int MapHeight { get; set; }
        int MapWidth { get; set; }
        int MapGoalAreaHeight { get; set; }
        bool WaitingForExchangeAnswer { get; set; }
    }
}

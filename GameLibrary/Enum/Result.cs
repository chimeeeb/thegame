namespace GameLibrary.Enum
{
    /// <summary>
    /// Results of putting the piece
    /// </summary>
    public enum Result
    {
        /// <summary>
        /// Piece was put in task area
        /// </summary>
        PiecePutInTaskArea,
        /// <summary>
        /// Putted a piece on the goal
        /// </summary>
        GoalCompleted,
        /// <summary>
        /// Putted a piece on the non-goal tile
        /// </summary>
        NonGoalDiscovered,
        /// <summary>
        /// Put fake piece in goal area
        /// </summary>
        FakePieceInGoalArea
    }
}

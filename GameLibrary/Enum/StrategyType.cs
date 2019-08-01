namespace GameLibrary.Enum
{
    /// <summary>
    /// Different strategies to choose from Agent settings menu.
    /// </summary>
    public enum StrategyType
    {
        /// <summary>
        /// A quite reasonable one, with each Agent playing on the whole board.
        /// </summary>
        Normal,
        /// <summary>
        /// Completely random actions.
        /// </summary>
        Random,
        /// <summary>
        /// Uses discovery to determine each Agent move.
        /// </summary>
        Discoverer,
        /// <summary>
        /// Only exchange info action used - mainly for testing.
        /// </summary>
        Exchange,
        /// <summary>
        /// A sophisticated tactic proved to be a winning one, with board division between Agents and faster goals detection.
        /// </summary>
        Superior
    }
}
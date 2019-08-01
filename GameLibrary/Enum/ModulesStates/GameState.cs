namespace GameLibrary.Enum
{
    /// <summary>
    /// GM and Agents GUI states during the game.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The game is stopped (error messages generated).
        /// </summary>
        Stopped,
        /// <summary>
        /// An Agent searches for a game ("Searching..." button).
        /// </summary>
        Searching,
        /// <summary>
        /// An Agent waits for other playes ("Waiting for players..." button).
        /// </summary>
        WaitingForStart,
        /// <summary>
        /// The game has started ("Game started" button).
        /// </summary>
        Started
    }
}
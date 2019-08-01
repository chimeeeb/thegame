namespace GameLibrary.Enum
{
    /// <summary>
    /// State of an Agent during the game.
    /// </summary>
    public enum AgentState
    {
        /// <summary>
        /// Agent is not connected to the game yet (can be connected to CS at TCP level).
        /// </summary>
        Disconnected,
        /// <summary>
        /// Agent is connected to the game.
        /// </summary>
        Connected,
        /// <summary>
        /// The game is running, Agent is playing.
        /// </summary>
        IsPlaying
    }
}

namespace GameLibrary.Enum
{
    /// <summary>
    /// Game Master states.
    /// </summary>
    public enum GameMasterState
    {
        /// <summary>
        /// Not connected to the server (can be connected on TCP level).
        /// </summary>
        Disconnected,
        /// <summary>
        /// Connected to the server, waiting for Agents to join.
        /// </summary>
        Connected,
        /// <summary>
        /// All agents have connected to the game, setup finished.
        /// </summary>
        ReadyForGame,
        /// <summary>
        /// The game is being played.
        /// </summary>
        GameRunning
    }
}

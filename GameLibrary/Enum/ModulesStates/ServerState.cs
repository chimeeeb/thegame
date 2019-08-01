namespace GameLibrary.Enum
{
    /// <summary>
    /// Communication server states.
    /// </summary>
    public enum ServerState
    {
        /// <summary>
        /// GM not connected yet.
        /// </summary>
        GmNotConnected,
        /// <summary>
        /// Game Master connected, waiting for all Agents.
        /// </summary>
        GmConnected,
        /// <summary>
        /// The game is running.
        /// </summary>
        GameRunning,
        /// <summary>
        /// The game has ended, the server invalidates Agents requests and disconnects the Agents.
        /// </summary>
        GameEnded
    }
}

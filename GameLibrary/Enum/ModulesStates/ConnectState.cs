namespace GameLibrary.Enum
{
    /// <summary>
    /// State of TCP Client.
    /// </summary>
    public enum ConnectState
    {
        /// <summary>
        /// Not connected to TCP server.
        /// </summary>
        Disconnected,
        /// <summary>
        /// Attempting to connect to TCP server.
        /// </summary>
        Connecting,
        /// <summary>
        /// Connected to TCP server.
        /// </summary>
        Connected
    }
}
namespace GameLibrary.Enum
{
    /// <summary>
    /// Available statuses of a piece for an Agent.
    /// </summary>
    public enum PieceStatus
    {
        /// <summary>
        /// Tells that the Agent does not have a piece.
        /// </summary>
        None,
        /// <summary>
        /// Tells that the Agent holds a yet untested piece.
        /// </summary>
        Unidentified,
        /// <summary>
        /// Tells that the Agent holds a real piece.
        /// </summary>
        Real,
        /// <summary>
        /// Tells that the Agent holds a piece already confirmed to be fake.
        /// </summary>
        Sham
    }
}

namespace GameLibrary.Enum
{
    /// <summary>
    /// Types of teams.
    /// </summary>
    public enum Team
    {
        /// <summary>
        /// Tells that the Agent is assigned to Red team (0), Red team's goal area is on the bottom of the map
        /// </summary>
        Red,
        /// <summary>
        /// Tells that the Agent is assigned to Blue team (1), Blue team's goal area is on the top of the map
        /// </summary>
        Blue,
        /// <summary>
        /// Tells that the Agent does not have a team.
        /// </summary>
        None
    }
}
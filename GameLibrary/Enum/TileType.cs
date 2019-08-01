namespace GameLibrary.Enum
{
    /// <summary>
    /// Represents type of a tile.
    /// </summary>
    public enum TileType
    {
        /// <summary>
        /// No information about tile.
        /// </summary>
        Unknown,
        /// <summary>
        /// Tile in the goal area that does not have a goal.
        /// </summary>
        NoGoal,
        /// <summary>
        /// Tile in the goal area that contains a goal not unlocked yet.
        /// </summary>
        Goal,
        /// <summary>
        /// Tile in the goal area that contains a goal but it's already discovered by an agent.
        /// </summary>
        DiscoveredGoal,
        /// <summary>
        /// Tile in the task area.
        /// </summary>
        Task
    }
}
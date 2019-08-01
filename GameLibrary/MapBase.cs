using System;
using GameLibrary.Interface;
using log4net;
using Newtonsoft.Json;

namespace GameLibrary
{
    /// <summary>
    /// Base class for all maps in the game.
    /// </summary>
    public abstract class MapBase : IMap
    {
        /// <summary>
        /// Random engine
        /// </summary>
        protected readonly Random Random;
        /// <summary>
        /// The instance of a logger
        /// </summary>
        protected ILog Logger;
        /// <summary>
        /// Gets the width size of the map in tiles.
        /// </summary>
        [JsonIgnore]
        public int Width { get; }
        /// <summary>
        /// Gets the height size of the map in tiles.
        /// </summary>
        [JsonIgnore]
        public int Height { get; }
        /// <summary>
        /// Gets the height size of the goal area in tiles.
        /// </summary>
        [JsonIgnore]
        public int GoalAreaHeight { get; }

        /// <summary>
        /// Create a basic map
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        /// <param name="goalAreaHeight">Height of the goal area on the map</param>
        protected MapBase(int width, int height, int goalAreaHeight)
        {
            Random = new Random();
            Width = width;
            Height = height;
            GoalAreaHeight = goalAreaHeight;
        }
    }
}
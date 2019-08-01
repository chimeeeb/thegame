using System;
using System.Collections.Generic;
using System.Linq;
using GameLibrary.Interface;
using Newtonsoft.Json;

namespace GameLibrary
{
    /// <summary>
    /// Base class for all maps in the game, with tiles of given type.
    /// </summary>
    /// <typeparam name="T">Tile type.</typeparam>
    public abstract class MapBase<T> : MapBase, IMap<T> where T : class, ITile, new()
    {
        [JsonProperty]
        protected T[,] Tiles;

        /// <summary>
        /// Creates a new MapBase object.
        /// </summary>
        /// <param name="width">Width of the map.</param>
        /// <param name="height">Height of the map.</param>
        /// <param name="goalAreaHeight">Height of the goal area of a team.</param>
        /// <remarks>
        /// Assumes that task area is of even height and placed in the center of the game board.
        /// </remarks>
        protected MapBase(int width, int height, int goalAreaHeight) : base(width, height, goalAreaHeight)
        {
            Tiles = new T[Width, Height];
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                this[x, y] = new T {X = x, Y = y};
        }

        /// <summary>
        /// A helper function to randomly find given number of tiles that meet the condition specified, and to execute an action on them.
        /// </summary>
        /// <param name="condition">Condition to filter tiles.</param>
        /// <param name="count">Number of tiles to return.</param>
        /// <param name="actionOnRestTiles">An action to be done on matching tiles.</param>
        /// <param name="actionOnChosenTiles">An action to be done on not matched tiles.</param>
        /// <returns></returns>
        protected IEnumerable<T> GetRandomTiles(Func<T, bool> condition, int count, Action<T> actionOnRestTiles = null, Action<T> actionOnChosenTiles = null)
        {
            List<T> filteredTiles = new List<T>();
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (condition(this[i, j]))
                        filteredTiles.Add(this[i, j]);

            for (int i = 0; i < filteredTiles.Count; i++)
                if (Random.Next(filteredTiles.Count - i) < count)
                {
                    count--;
                    actionOnChosenTiles?.Invoke(filteredTiles[i]);
                    yield return filteredTiles[i];
                }
                else
                    actionOnRestTiles?.Invoke(filteredTiles[i]);
        }

        /// <summary>
        /// A helper function to randomly find a tile that meets given condition, and to execute an action on that tile.
        /// </summary>
        /// <param name="condition">Condition to filter the tile.</param>
        /// <param name="actionOnRestTiles">An action to be done on the tile.</param>
        /// <returns></returns>
        protected T GetRandomTile(Func<T, bool> condition, Action<T> actionOnRestTiles = null) =>
            GetRandomTiles(condition, 1, actionOnRestTiles).First();

        public T this[int i, int j]
        {
            get
            {
                if (i < 0 || j < 0 || i >= Width || j >= Height) return null;
                return Tiles[i, j];
            }
            set
            {
                if (i < 0 || j < 0 || i >= Width || j >= Height)
                    throw new ArgumentException("Index out of the array");
                Tiles[i, j] = value;
            }
        }
    }
}
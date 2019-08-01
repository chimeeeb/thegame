using System;
using GameLibrary;
using log4net;
using GameLibrary.Messages;
using GameLibrary.Enum;
using System.Collections.Generic;

namespace Player
{
    /// <summary>
    /// Game board as seen by an Agent
    /// </summary>
    public class Map : MapBase<Tile>
    {
        /// <summary>
        /// Creates a new AgentMap object.
        /// </summary>
        /// <param name="width">Width of the map.</param>
        /// <param name="height">Height of the map.</param>
        /// <param name="goalAreaHeight">Height of the goal area of a team.</param>
        /// <remarks>
        /// Assumes that task area is of even height and placed in the center of the game board.
        /// </remarks>
        public Map(int width, int height, int goalAreaHeight) : base(width, height, goalAreaHeight)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = goalAreaHeight; j < height - goalAreaHeight; j++)
                {
                    this[i, j].UpdateTile(DateTime.Now, -1, TileType.Task);
                }
            }
            Logger = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// Updates a 3x3 square of AgentTiles from AgentMap after a successful discovery request.
        /// </summary>
        /// <param name="distanceToPiece">New distance to piece for updated tiles</param>
        /// <param name="timeStamp">New time stamp for updated tiles</param>
        public void UpdateAfterDiscovery(List<JMapTile> distanceToPiece, DateTime timeStamp)
        {
            foreach (JMapTile f in distanceToPiece)
            {
                this[f.X, f.Y]?.UpdateTile(timeStamp, f.Distance);
            }
        }

        /// <summary>
        /// Updates the whole map after a successful information exchange
        /// </summary>
        /// <param name="infoMap"></param>
        public void UpdateAfterExchangeInfo(Map infoMap)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (infoMap.Tiles[i, j].Type == TileType.DiscoveredGoal || infoMap.Tiles[i, j].Type == TileType.NoGoal || infoMap.Tiles[i, j].Type == TileType.Goal)
                    {
                        Tiles[i, j].UpdateTile(DateTime.Now, Tiles[i, j].DistanceToPiece, infoMap.Tiles[i, j].Type);
                    }
                }
            }
        }

        /// <summary>
        /// Converts the map into a string representation in order to pass it in info exchange message.
        /// </summary>
        /// <param name="map">Agent's map to be converted.</param>
        /// <returns>A string that represents relevant map data.</returns>
        /// <remarks>
        /// Writes map in order (0,0) --> (0, map.Width - 1) --> (map.Height - 1, map.Width - 1).
        /// For each tile, it writes a number corresponding to tile type.
        /// </remarks>
        public static string GetDataStringFromMap(Map map)
        {
            string data = "";
            for(int i = 0; i < map.Width; i++)
            {
                for(int j = 0; j < map.Height; j++)
                {
                    data += (int)map[i, j].Type;
                }
            }
            return data;
        }

        /// <summary>
        /// Gets map object from its string representation.
        /// </summary>
        /// <param name="data">String containing information about goal area tile types.</param>
        /// <param name="width">Map width.</param>
        /// <param name="height">Map height.</param>
        /// <param name="goalAreaHeight">Map goal area height.</param>
        /// <returns>A complete map object re-created from the string.</returns>
        /// /// <remarks>
        /// Goal area data is in order (0,0) --> (0, map.Width - 1) --> (map.Height - 1, map.Width - 1).
        /// For each tile, it contains a number corresponding to tile type.
        /// </remarks>
        public static Map GetMapFromDataString(string data, int width, int height, int goalAreaHeight)
        {
            Map map = new Map(width, height, goalAreaHeight);
            for(int i = 0; i < map.Width; i++)
            {
                for(int j = 0; j < map.Height; j++)
                {
                    map[i, j].UpdateTile(DateTime.Now, -1, Enum.Parse(typeof(TileType), data[i * height + j].ToString()) as TileType?);
                }
            }
            return map;
        }
    }
}
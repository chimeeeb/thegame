using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using GameLibrary;
using GameLibrary.Enum;
using log4net;

namespace Game
{
    public class Map : MapBase<Tile>
    {
        /// <summary>
        /// Creates a new GameMasterMap object.
        /// </summary>
        /// <param name="width">Width of the map.</param>
        /// <param name="height">Height of the map.</param>
        /// <param name="goalAreaHeight">Height of the goal area of a team.</param>
        /// <remarks>
        /// Assumes that task area is of even height and placed in the center of the game board.
        /// </remarks>
        public Map(int width, int height, int goalAreaHeight, int numberOfGoalsPerTeam) : base(width, height, goalAreaHeight)
        {
            Logger = LogManager.GetLogger(GetType());
            GenerateGoalTiles(numberOfGoalsPerTeam);
            for(int i = 0; i < Width; i++)
            {
                for(int j = GoalAreaHeight; j < Height - GoalAreaHeight; j++)
                {
                    this[i, j].UpdateTile(DateTime.Now, type: TileType.Task);
                }
            }
        }

        /// <summary>
        /// Generates all tiles in the goal areas, with NumberOfGoalsPerTeam goal tiles in each goal area.
        /// </summary>
        /// <remarks>
        /// Assumes that blue team has its goal area on the top and red team - at the bottom of the game board.
        /// </remarks>
        public void GenerateGoalTiles(int numberOfGoalsPerTeam)
        {
            Logger.Debug($"Generating goal tiles");
            GetRandomTiles(tile => tile.Y < GoalAreaHeight, numberOfGoalsPerTeam,
                tile =>
                {
                    tile.UpdateTile(DateTime.Now, type: TileType.NoGoal);
                    Logger.Debug($"Setting red-side non-goal on position ({tile.X}, {tile.Y})");
                    this[Width - 1 - tile.X, Height - 1 - tile.Y].UpdateTile(DateTime.Now, type: TileType.NoGoal);
                    Logger.Debug($"Setting blue-side non-goal on position ({Width - 1 - tile.X}, {Height - 1 - tile.Y})");
                },
                tile =>
                {
                    tile.UpdateTile(DateTime.Now, type: TileType.Goal);
                    Logger.Debug($"Setting red-side goal on position ({tile.X}, {tile.Y})");
                    this[Width - 1 - tile.X, Height - 1 - tile.Y].UpdateTile(DateTime.Now, type: TileType.Goal);
                    Logger.Debug($"Setting blue-side goal on position ({Width - 1 - tile.X}, {Height - 1 - tile.Y})");
                }).ToList();
        }

        /// <summary>
        /// Generates one piece which is randomly defined to be either real or fake, and places it at random task tile on the map.
        /// </summary>
        public void GeneratePiece(double probabilityOfBadPiece)
        {
            int rows = Height - GoalAreaHeight;
            List<Point> freePositions = new List<Point>();
            for (int i = 0; i < Width; i++)
            {
                for (int j = GoalAreaHeight; j < rows; j++)
                {
                    if (this[i, j].Piece == Piece.Null)
                    {
                        freePositions.Add(new Point(i, j));
                    }
                }
            }
            if (freePositions.Count == 0) return;
            Point chosenPosition = freePositions[Random.Next(freePositions.Count)];
            bool isReal = Random.NextDouble() > probabilityOfBadPiece;
            this[chosenPosition.X, chosenPosition.Y].UpdateTileAddPiece(DateTime.Now, isReal ? Piece.Real : Piece.Fake);
            Logger.Debug($"Generated piece on position ({chosenPosition.X}, {chosenPosition.Y})");
        }

        /// <summary>
        /// Finds and returns randomly a valid place for a new agent.
        /// </summary>
        /// <param name="agentId">Id of agent to be placed on map</param>
        /// <returns>A valid position for a new agent</returns>
        public Tile FindPlaceForAgent(int agentId, Team team)
        {
            var randomTile = GetRandomTile(tile => (team == Team.Red ? tile.Y < GoalAreaHeight : tile.Y >= Height - GoalAreaHeight) && tile.AgentId == -1);
            randomTile?.UpdateTile(DateTime.Now, agentId);
            return randomTile;
        }

        /// <summary>
        /// Returns a distance to the closest piece from a given point.
        /// </summary>
        /// <param name="x">X-coordinate of requested tile.</param>
        /// <param name="y">Y-coordinate of requested tile.</param>
        /// <returns>Calculated distance to a piece</returns>
        public int DistanceToPiece(int x, int y)
        {
            if (this[x, y].Piece != Piece.Null)
                return 0;

            int distance = int.MaxValue;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (this[i, j].Piece != Piece.Null && Math.Abs(i - x) + Math.Abs(j - y) < distance)
                        distance = Math.Abs(i - x) + Math.Abs(j - y);
            if (distance == int.MaxValue)
                return -1;
            return distance;
        }
    }
}
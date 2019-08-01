//using System;
//using System.Collections.Generic;
//using System.Drawing;

//using ProjectGame;
//using ProjectGame.Communication;
//using ProjectGame.Items;
//using ProjectGame.Logic;

//namespace Tests.TestClasses
//{
//    /// <summary>
//    /// A mocking class to test pure game logic inside Game Master without communication with Agents.
//    /// </summary>
//    /// <remarks>
//    /// The class is based on past Game Master version which was later modified to work with Agents in the multithreading environment. Its main purpose was to test all Agent actions and map updates before introducing communication solutions.
//    /// </remarks>
//    public class GameMasterMock
//    {
//        public bool IsRunning { get; set; }
//        public int NumberOfPlayers { get; private set; }
//        public int NumberOfTeamRedPlayers { get; private set; }
//        public int NumberOfTeamBluePlayers { get; private set; }
//        public Team WinningTeam { get; private set; }
//        public GameMasterMap Map { get; set; }
//        public int NumberOfPieces;
//        private readonly Random _random;
//        public Point[] AgentsPositions;
//        public Dictionary<int, Piece> AgentIdsToPieces;

//        /// <summary>
//        /// Mock Game Master constructor. It is different from original Game Master as in the real program input parameters are validated, clamped and set inside settings form, and the original constructor is parameterless. Here, validation is replaced with exception throwing.
//        /// </summary>
//        public GameMasterMock(int width, int height, int goalAreaHeight, int maxNumberOfPieces, int maxNumberOfPlayers = 2, int goalsPerTeam = 1)
//        {
//            if (width < 4) throw new ArgumentException("Board width must be bigger than 3.");
//            if (height < 4) throw new ArgumentException("Board height must be bigger than 3.");
//            if (goalAreaHeight < 1) throw new ArgumentException("Goal area height must be bigger than 0.");
//            if (maxNumberOfPieces < 1) throw new ArgumentException("There must be at least one piece on the board.");
//            if (2 * goalAreaHeight + 2 >= height) throw new ArgumentException("Goal area height must be smaller than half of total height - 1.");

//            Parameters.MapWidth = width;
//            Parameters.MapHeight = height;
//            Parameters.GoalAreaHeight = goalAreaHeight;
//            Parameters.NumberOfGoalsPerTeam = goalsPerTeam;
//            Parameters.NumberOfPieces = maxNumberOfPieces;
//            Parameters.NumberOfPlayers = maxNumberOfPlayers;

//            Map = new GameMasterMap(Parameters.MapWidth, Parameters.MapHeight, Parameters.GoalAreaHeight);
//            NumberOfPieces = 0;
//            _random = new Random();
//            IsRunning = false;
//            NumberOfPlayers = 0;
//            NumberOfTeamRedPlayers = 0;
//            NumberOfTeamBluePlayers = 0;
//            WinningTeam = Team.None;
//            AgentsPositions = new Point[Parameters.NumberOfPlayers];
//            AgentIdsToPieces = new Dictionary<int, Piece>();
//        }

//        public void PlayGeneratePieceOnly()
//        {
//            DateTime nextPieceGenerationTime = GeneratePiece();
//            while(NumberOfPieces < Parameters.NumberOfPieces)
//            {
//                if (DateTime.Now >= nextPieceGenerationTime)
//                {
//                    nextPieceGenerationTime = GeneratePiece();
//                }
//            }
//        }

//        public (bool, Result) PlayWithPutPiece(int agentId)
//        {
//            bool putPieceAllowed = false;
//            Result result = Result.NoInformation;

//            IsRunning = true;
//            DateTime nextPieceGenerationTime = GeneratePiece();

//            while (IsRunning)
//            {
//                (putPieceAllowed, result) = PlayerPutPiece(agentId);

//                if (DateTime.Now >= nextPieceGenerationTime && NumberOfPieces < Parameters.NumberOfPieces)
//                {
//                    nextPieceGenerationTime = GeneratePiece();
//                }
//            }

//            return (putPieceAllowed, result);
//        }

//        public void UpdateWinningTeam(bool? currentResult)
//        {
//            if (!currentResult.HasValue)
//            {
//                WinningTeam = Team.None;
//                return;
//            }
//            IsRunning = false;
//            if (currentResult.Value) WinningTeam = Team.Red;
//            else WinningTeam = Team.Blue;
//        }

//        private DateTime GeneratePiece()
//        {
//            Map.GeneratePiece();
//            NumberOfPieces++;
//            return DateTime.Now.AddMilliseconds(Parameters.PieceGenerationInterval);
//        }

//        public void AddNewPlayer(Team team)
//        {
//            if (NumberOfPlayers == Map.Width * Map.Height) throw new ArgumentOutOfRangeException("Game board is already full.");
//            if (CheckPlayer(team))
//            {
//                Point position = GeneratePosition(NumberOfPlayers);
//                AgentsPositions[NumberOfPlayers] = position;
//                Map.UpdateTile(position.X, position.Y, NumberOfPlayers);
//                if (team == Team.Blue) NumberOfTeamBluePlayers++;
//                else NumberOfTeamRedPlayers++;
//                NumberOfPlayers++;
//            }
//            else throw new ArgumentOutOfRangeException("Requested team is already full.");
//        }

//        public void AddNewPlayer(Team team, int x, int y)
//        {
//            if (NumberOfPlayers == Map.Width * Map.Height) throw new ArgumentOutOfRangeException("Game board is already full.");
//            if (CheckPlayer(team))
//            {
//                AgentsPositions[NumberOfPlayers] = new Point(x, y);
//                Map.UpdateTile(x, y, NumberOfPlayers);
//                if (team == Team.Blue) NumberOfTeamBluePlayers++;
//                else NumberOfTeamRedPlayers++;
//                NumberOfPlayers++;
//            }
//            else throw new ArgumentOutOfRangeException("Requested team is already full.");
//        }

//        private bool CheckPlayer(Team team)
//        {
//            if (team == Team.Blue && NumberOfTeamBluePlayers < Parameters.NumberOfPlayers / 2)
//                    return true;

//            if (team == Team.Red && NumberOfTeamRedPlayers < Parameters.NumberOfPlayers / 2)
//                    return true;

//            return false;
//        }

//        private Point GeneratePosition(int agentId)
//        {
//            return Map.FindPlaceForAgent(agentId);
//        }

//        public bool PlayerMove(Direction direction, int agentId)
//        {
//            bool permission = true;
//            switch (direction)
//            {
//                case Direction.Up:
//                    if (AgentsPositions[agentId].Y - 1 >= 0)
//                    {
//                        GameMasterTile tile = Map.GetTile(AgentsPositions[agentId].X, AgentsPositions[agentId].Y - 1);
//                        if (tile.AgentId != -1)
//                            permission = false;
//                    }
//                    else
//                        permission = false;
//                    if (permission)
//                    {
//                        Map.UpdateTile(AgentsPositions[agentId].X, AgentsPositions[agentId].Y, -1);
//                        Map.UpdateTile(AgentsPositions[agentId].X, AgentsPositions[agentId].Y - 1, agentId);
//                        AgentsPositions[agentId] = new Point(AgentsPositions[agentId].X, AgentsPositions[agentId].Y - 1);
//                    }
//                    break;
//                case Direction.Right:
//                    if (AgentsPositions[agentId].X + 1 < Map.Width)
//                    {
//                        GameMasterTile tile = Map.GetTile(AgentsPositions[agentId].X + 1, AgentsPositions[agentId].Y);
//                        if (tile.AgentId != -1)
//                            permission = false;
//                    }
//                    else
//                        permission = false;
//                    if (permission)
//                    {
//                        Map.UpdateTile(AgentsPositions[agentId].X, AgentsPositions[agentId].Y, -1);
//                        Map.UpdateTile(AgentsPositions[agentId].X + 1, AgentsPositions[agentId].Y, agentId);
//                        AgentsPositions[agentId] = new Point(AgentsPositions[agentId].X + 1, AgentsPositions[agentId].Y);
//                    }
//                    break;
//                case Direction.Down:
//                    if (AgentsPositions[agentId].Y + 1 < Map.Height)
//                    {
//                        GameMasterTile tile = Map.GetTile(AgentsPositions[agentId].X, AgentsPositions[agentId].Y + 1);
//                        if (tile.AgentId != -1)
//                            permission = false;
//                    }
//                    else
//                        permission = false;
//                    if (permission)
//                    {
//                        Map.UpdateTile(AgentsPositions[agentId].X, AgentsPositions[agentId].Y, -1);
//                        Map.UpdateTile(AgentsPositions[agentId].X, AgentsPositions[agentId].Y + 1, agentId);
//                        AgentsPositions[agentId] = new Point(AgentsPositions[agentId].X, AgentsPositions[agentId].Y + 1);
//                    }
//                    break;
//                case Direction.Left:
//                    if (AgentsPositions[agentId].X - 1 >= 0)
//                    {
//                        GameMasterTile tile = Map.GetTile(AgentsPositions[agentId].X - 1, AgentsPositions[agentId].Y);
//                        if (tile.AgentId != -1)
//                            permission = false;
//                    }
//                    else
//                        permission = false;
//                    if (permission)
//                    {
//                        Map.UpdateTile(AgentsPositions[agentId].X, AgentsPositions[agentId].Y, -1);
//                        Map.UpdateTile(AgentsPositions[agentId].X - 1, AgentsPositions[agentId].Y, agentId);
//                        AgentsPositions[agentId] = new Point(AgentsPositions[agentId].X - 1, AgentsPositions[agentId].Y);
//                    }
//                    break;
//            }
//            int distanceToPiece = -1;
//            if (permission)
//            {
//                distanceToPiece = Map.DistanceToPiece(AgentsPositions[agentId].X, AgentsPositions[agentId].Y);
//            }
//            return permission;
//        }

//        public int[] PlayerDiscovery(int agentId)
//        {
//            int[] discoveryResults = new int[9];
//            Point p = AgentsPositions[agentId];
//            int k = 0;
//            for (int j = -1; j <= 1; j++)
//            {
//                for (int i = -1; i <= 1; i++)
//                {
//                    if (p.X + i >= 0 && p.X + i < Map.Width && p.Y + j >= 0 && p.Y + j < Map.Height)
//                        discoveryResults[k] = Map.DistanceToPiece(p.X + i, p.Y + j);
//                    else
//                        discoveryResults[k] = 0;
//                    k++;
//                }
//            }
//            return discoveryResults;
//        }

//        public bool PlayerPickUpPiece(int agentId)
//        {
//            int x = AgentsPositions[agentId].X;
//            int y = AgentsPositions[agentId].Y;
//            GameMasterTile agentTile = Map.GetTile(x, y);
//            if (agentTile.Piece != null && !AgentIdsToPieces.ContainsKey(agentId))
//            {
//                AgentIdsToPieces.Add(agentId, agentTile.Piece);
//                agentTile.Piece = null;
//                return true;
//            }
//            return false;
//        }

//        public PieceStatus PlayerTestPiece(int agentId)
//        {
//            if (AgentIdsToPieces.TryGetValue(agentId, out Piece piece))
//            {
//                if (piece.IsReal) return PieceStatus.Real;
//                return PieceStatus.Sham;
//            }
//            return PieceStatus.Unidentified;
//        }

//        public bool PlayerDestroyPiece(int agentId)
//        {
//            if (AgentIdsToPieces.ContainsKey(agentId))
//            {
//                AgentIdsToPieces.Remove(agentId);
//                return true;
//            }
//            return false;
//        }

//        public (bool, Result) PlayerPutPiece(int agentId)
//        {
//            int x = AgentsPositions[agentId].X;
//            int y = AgentsPositions[agentId].Y;
//            GameMasterTile agentTile = Map.GetTile(x, y);
//            Result result;

//            if (AgentIdsToPieces.TryGetValue(agentId, out Piece piece) && agentTile.Piece == null)
//            {
//                if (piece.IsReal)
//                {
//                    switch (agentTile.Type)
//                    {
//                        case Tile.TileType.Goal:
//                            agentTile.Type = Tile.TileType.DiscoveredGoal;
//                            bool? currentResult = Map.UpdateTeamResult(y);
//                            UpdateWinningTeam(currentResult);
//                            result = Result.GoalCompleted;
//                            break;

//                        case Tile.TileType.NoGoal:
//                            result = Result.NonGoalDiscovered;
//                            break;

//                        case Tile.TileType.DiscoveredGoal:
//                            result = Result.GoalCompleted;
//                            break;

//                        case Tile.TileType.Task:
//                            agentTile.Piece = piece;
//                            result = Result.NoInformation;
//                            break;

//                        default:
//                            throw new ArgumentException("Unexpected tile type");
//                    }
//                }
//                else
//                {
//                    if (agentTile.Type == Tile.TileType.Task)
//                        agentTile.Piece = piece;
//                    result = Result.NoInformation;
//                }
//                AgentIdsToPieces.Remove(agentId);
//                return (true, result);
//            }

//            return (false, Result.NoInformation);
//        }
//    }
//}

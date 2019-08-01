using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GameLibrary.Interface;
using GameLibrary.Enum;
using GameLibrary.Messages;
using GameLibrary;
using GameLibrary.Configuration;
using log4net;
using System.Collections.Generic;
using GameLibrary.GUI;
using Timer = System.Timers.Timer;
using System.Linq;

namespace Game
{
    /// <summary>
    /// A class that represents the Game Master who controls the game.
    /// </summary>
    public class GameMaster
    {
        /// <summary>
        /// Game settings set by the user in the settings window.
        /// </summary>
        public GameSettings Settings { get; set; }
        /// <summary>
        /// The instance of a logger
        /// </summary>
        private readonly ILog _logger;
        /// <summary>
        /// The instance of Communication Client responsible for managing the communication for the Agent, that is, passing and receiving messages from Communication Server.
        /// </summary>
        private TcpClient _client;
        /// <summary>
        /// GM Agent objects to represent GM state of information about each Agent.
        /// </summary>
        public Dictionary<int, Agent> Agents;
        /// <summary>
        /// Pairs of IDs of Agents that exchange information and the data from the first Agent.
        /// </summary>
        public Dictionary<string, string> Exchanges;
        /// <summary>
        /// Defines number of pieces on the board.
        /// </summary>
        public int NumberOfPieces;
        /// <summary>
        /// Tells which of the team won.
        /// </summary>
        public Team WinningTeam;
        /// <summary>
        /// Measures time from the start of the game till its end.
        /// </summary>
        private Stopwatch _gameTimer;
        /// <summary>
        /// Calls GeneratePiece() according to specified time intervals
        /// </summary>
        private Timer _pieceTimer;
        /// <summary>
        /// Shows the number of Agents already connected to the game.
        /// </summary>
        public int NumberOfPlayers { get; set; }
        /// <summary>
        /// Shows the number of Agents of the Read team already connected to the game.
        /// </summary>
        public int NumberOfTeamRedPlayers { get; set; }
        /// <summary>
        /// Shows the number of Agents of the Blue team already connected to the game.
        /// </summary>
        public int NumberOfTeamBluePlayers { get; set; }
        /// <summary>
        /// Tells what state GM is in.
        /// </summary>
        public GameMasterState State;
        /// <summary>
        /// Score of the red team
        /// </summary>
        public int RedTeamPoints { get; private set; }
        /// <summary>
        /// Score of the blue team
        /// </summary>
        public int BlueTeamPoints { get; private set; }
        /// <summary>
        /// Game Master view of a game board.
        /// </summary>
        public Map Map { get; set; }
        /// <summary>
        /// Game statistics that are presented at the end of the game.
        /// </summary>
        public Statistics Statistics { get; set; }
        /// <summary>
        /// Blue team leader Id, if not yet chosen equal -1
        /// </summary>
        public int BlueLeaderId { get; set; } = -1;
        /// <summary>
        /// Red team leader Id, if not yet chosen equal -1
        /// </summary>
        public int RedLeaderId { get; set; } = -1;
        /// <summary>
        /// Ids of agents from Blue team
        /// </summary>
        public List<int> BlueTeamIds { get; set; } = new List<int>();
        /// <summary>
        /// Ids of agents from Red team
        /// </summary>
        public List<int> RedTeamIds { get; set; } = new List<int>();

        /// <summary>
        /// Events that are fired to signify state changes
        /// </summary>
        public event Action GmConnected;
        public event Action PlayersReady;
        public event Action PlayersNotReady; // When a player disconnects before game start
        public event Action<Team, int, int> GameEnded;
        public event Action<PlayerInfo> PlayerConnected;
        public event Action<int> PlayerDisconnected;
        public event Action ServerDisconnected;
        public event Action<int> ReceivedRequestInGame;
        public event Action AgentBlockedInGame;
        public event Action AgentExchangeInfoResponse;

        /// <summary>
        /// Lock to make GM processing Agent requests synchronously.
        /// </summary>
        private object _syncLock = new object();

        /// <summary>
        /// Creates a new Game Master object.
        /// </summary>
        public GameMaster(GameSettings settings)
        {
            Settings = settings;
            _logger = LogManager.GetLogger(GetType());

            if(!Settings.IsLoggingEnabled)
            {
                log4net.Repository.Hierarchy.Hierarchy h = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
                log4net.Repository.Hierarchy.Logger rootLogger = h.Root;
                rootLogger.RemoveAllAppenders();
            }

            _logger.Debug("Creating new GameMaster instance");
            _gameTimer = new Stopwatch();
            _pieceTimer = new Timer(Settings.PieceGenerationInterval);
            _pieceTimer.Elapsed += async (sender, e) => await GeneratePiece();
            Agents = new Dictionary<int, Agent>();
            Exchanges = new Dictionary<string, string>();
            ResetGame();
        }

        /// <summary>
        /// Process an incoming message.
        /// </summary>
        /// <param name="message">Message received.</param>
        private void HandleMessage(Message message)
        {
            lock (_syncLock)
            {
                _logger.Debug($"Got new message of type {message.GetType()}");
                switch (State)
                {
                    case GameMasterState.Disconnected:
                        HandleMessageBeforeConnecting(message);
                        break;
                    case GameMasterState.Connected:
                        HandleMessageOnGameSetup(message);
                        break;
                    case GameMasterState.ReadyForGame:
                        HandleMessageBeforeGame(message);
                        break;
                    case GameMasterState.GameRunning:
                        HandleMessageDuringGame(message);
                        break;
                }
            }
        }

        /// <summary>
        /// Connect to Server, proccess possible error messages.
        /// </summary>
        /// <param name="message">Message received.</param>
        private void HandleMessageBeforeConnecting(Message message)
        {
            switch (message)
            {
                case AcceptedGmMessage acceptedGmMessage:
                    if (acceptedGmMessage.IsConnected)
                    {
                        _logger.Info("Game Master successfully connected to the communication server");
                        State = GameMasterState.Connected;
                        GmConnected?.Invoke();
                        return;
                    }
                    _logger.Error("Game Master was rejected by communication server");
                    break;
                case InvalidJsonMessage invalidJsonMessage:
                    _logger.Error("Invalid JSON message format");
                    break;
                case InvalidActionMessage invalidActionMessage:
                    _logger.Error("Invalid message submitted");
                    break;
                default:
                    // Unexpected, something went wrong
                    _logger.Error("Unexpected message received");
                    _client.Send(new InvalidActionMessage
                    {
                        AgentId = message is RequestMessage requestMessage ? requestMessage.AgentId : -1,
                        RequestId = message.RequestId
                    });
                    break;
            }
        }

        /// <summary>
        /// Connects Agents to the game and assigns them IDs.
        /// </summary>
        /// <param name="message">Message received.</param>
        private void HandleMessageOnGameSetup(Message message)
        {
            switch (message)
            {
                case ConnectToGameMessage connectMessage:
                    AddNewPlayer(connectMessage);
                    if (NumberOfPlayers == Settings.NumberOfPlayers)
                    {
                        State = GameMasterState.ReadyForGame;
                        PlayersReady?.Invoke();
                    }
                    break;
                case InvalidJsonMessage invalidJsonMessage:
                    _logger.Error("Invalid JSON message format");
                    break;
                case InvalidActionMessage invalidActionMessage:
                    _logger.Error("Invalid message submitted");
                    break;
                case AgentNotRespondingMessage agentNotRespondingMessage:
                    PlayerRemove(agentNotRespondingMessage);
                    break;
                default:
                    // Unexpected, something went wrong
                    _logger.Error("Unexpected message received");
                    _client.Send(new InvalidActionMessage
                    {
                        AgentId = message is RequestMessage requestMessage ? requestMessage.AgentId : -1,
                        RequestId = message.RequestId
                    });
                    break;
            }
        }

        /// <summary>
        /// Manages messages before game start.
        /// </summary>
        /// <param name="message">Message received.</param>
        public void HandleMessageBeforeGame(Message message)
        {
            switch (message)
            {
                case AgentNotRespondingMessage agentNotRespondingMessage:
                    PlayerRemove(agentNotRespondingMessage);
                    State = GameMasterState.Connected;
                    PlayersNotReady?.Invoke();
                    break;
                case InvalidJsonMessage invalidJsonMessage:
                    _logger.Error("Invalid JSON message format");
                    break;
                case InvalidActionMessage invalidActionMessage:
                    _logger.Error("Invalid message submitted");
                    break;
                default:
                    // Unexpected, something went wrong
                    _logger.Error("Unexpected message received");
                    _client.Send(new InvalidActionMessage
                    {
                        AgentId = message is RequestMessage requestMessage ? requestMessage.AgentId : -1,
                        RequestId = message.RequestId
                    });
                    break;
            }
        }

        /// <summary>
        /// Handles messages during game.
        /// </summary>
        /// <param name="message">Message received.</param>
        private void HandleMessageDuringGame(Message message)
        {
            if (message as RequestMessage != null)
            {
                RequestMessage request = (message as RequestMessage);

                if (!Agents.ContainsKey(request.AgentId) || Agents[request.AgentId].Disconnected)
                {
                    _client.Send(new InvalidActionMessage
                    {
                        AgentId = request.AgentId,
                        RequestId = message.RequestId
                    });
                    return;
                }

                if (Agents[request.AgentId].WaitingUntilTime.Ticks == 0 ||
                    Agents[request.AgentId].WaitingUntilTime.TotalMilliseconds <= _gameTimer.ElapsedMilliseconds)
                {
                    if (Agents[request.AgentId].CalledByLeader)
                    {
                        _client.Send(new InvalidActionMessage
                        {
                            AgentId = request.AgentId,
                            RequestId = request.RequestId
                        });
                        AgentBlockedInGame?.Invoke();
                    }
                    else
                    {
                        switch (request)
                        {
                            case MoveRequestMessage moveMessage:
                                PlayerMove(moveMessage);
                                break;
                            case DiscoveryRequestMessage discoveryMessage:
                                PlayerDiscovery(discoveryMessage);
                                break;
                            case PickUpRequestMessage pickMessage:
                                PlayerPickUpPiece(pickMessage);
                                break;
                            case TestPieceRequestMessage testMessage:
                                PlayerTestPiece(testMessage);
                                break;
                            case DestroyPieceRequestMessage destroyMessage:
                                PlayerDestroyPiece(destroyMessage);
                                break;
                            case PutPieceRequestMessage putMessage:
                                PlayerPutPiece(putMessage);
                                break;
                            case ExchangeInfosRequestMessage requestMessage:
                                PlayerExchangeInfos(requestMessage);
                                break;
                            default:
                                // Unexpected, something went wrong
                                _logger.Error("Unexpected message received");
                                _client.Send(new InvalidActionMessage
                                {
                                    AgentId = request.AgentId,
                                    RequestId = request.RequestId
                                });
                                break;
                        }
                    }
                }
                else
                {
                    // Agent request rejected due to violation of waiting time after his previous action.
                    _client.Send(new RequestDuringPenaltyMessage
                    {
                        AgentId = request.AgentId,
                        GameTimeStamp = (int)_gameTimer.ElapsedMilliseconds,
                        WaitUntilTime = (int)Agents[request.AgentId].WaitingUntilTime.TotalMilliseconds,
                        RequestId = request.RequestId
                    });
                }
                ReceivedRequestInGame?.Invoke(request.AgentId);
            }
            else
            {
                switch (message)
                {
                    case ExchangeInfosResponseMessage responseMessage:
                        PlayerRespondInfosExchange(responseMessage);
                        AgentExchangeInfoResponse?.Invoke();
                        break;
                    case InvalidJsonMessage invalidJsonMessage:
                        _logger.Error("Invalid JSON message format");
                        break;
                    case InvalidActionMessage invalidActionMessage:
                        _logger.Error("Invalid message submitted");
                        break;
                    case AgentNotRespondingMessage agentNotRespondingMessage:
                        PlayerRemove(agentNotRespondingMessage);
                        break;
                    default:
                        // Unexpected, something went wrong
                        _logger.Error("Unexpected message received");
                        _client.Send(new InvalidActionMessage
                        {
                            AgentId = -1,
                            RequestId = message.RequestId
                        });
                        break;
                }
            }
        }

        /// <summary>
        /// Starts the game once all Agents joined.
        /// </summary>
        public void StartGame()
        {
            foreach (var entry in Agents)
            {
                Agent agent = entry.Value;
                int id = entry.Key;
                _client.Send(new GameInfoMessage(Settings)
                {
                    AgentId = id,
                    TeamLeaderId = agent.Team == Team.Red ? RedLeaderId : BlueLeaderId,
                    AgentIdsFromTeam = agent.Team == Team.Red ? RedTeamIds.Where(val => val != id).ToArray() : BlueTeamIds.Where(val => val != id).ToArray(),
                    GameTime = (int)_gameTimer.ElapsedMilliseconds,
                    InitialXPosition = agent.Tile.X,
                    InitialYPosition = agent.Tile.Y
                });
            }
            _gameTimer.Start();
            _pieceTimer.Start();
            _logger.Info("Game Master starts game");
            State = GameMasterState.GameRunning;
        }

        /// <summary>
        /// Ends the game.
        /// </summary>
        private void EndGame()
        {
            InformAboutWinner();
            Disconnect();
        }

        /// <summary>
        /// Sends message with game result to first Agent. CS will then broadcast it to all Agents.
        /// </summary>
        public void InformAboutWinner()
        {
            GameEnded?.Invoke(WinningTeam, BlueTeamPoints, RedTeamPoints);
            if (Agents.Count > 0)
            {
                _client.Send(new GameEndedMessage
                {
                    WinningTeam = WinningTeam,
                    AgentId = Agents[0].Id,
                    GameTimeStamp = (int)_gameTimer.ElapsedMilliseconds
                });
            }

            Statistics = new Statistics
            {
                WinningTeam = WinningTeam,
                BlueTeamPoints = BlueTeamPoints,
                RedTeamPoints = RedTeamPoints,
                GameTime = _gameTimer.Elapsed
            };

            _logger.Info(Statistics);
        }

        /// <summary>
        /// Wraps Map.GeneratePiece() and updates pieces counter.
        /// </summary>
        /// <returns></returns>
        private async Task GeneratePiece()
        {
            if (NumberOfPieces < Settings.NumberOfPieces)
            {
                NumberOfPieces++;
                _logger.Debug("New piece is generated on the board");
                Map.GeneratePiece(Settings.ProbabilityOfBadPiece);
            }
        }

        /// <summary>
        /// Checks whether a player in given team can connect to the game.
        /// </summary>
        /// <param name="connectMessage">Received connect to game request</param>
        /// <returns> True if Agent can be assigned to requested team and connected to the game, false otherwise.</returns>
        private bool CheckPlayer(ConnectToGameMessage connectMessage)
        {
            if (connectMessage.TeamId == Team.Blue)
                if (NumberOfTeamBluePlayers < Settings.NumberOfPlayers / 2)
                    return true;

            if (connectMessage.TeamId == Team.Red)
                if (NumberOfTeamRedPlayers < Settings.NumberOfPlayers / 2)
                    return true;

            return false;
        }

        /// <summary>
        /// Adds a new player to the game.
        /// </summary>
        /// <param name="connectMessage">Received connect to game request</param>
        public void AddNewPlayer(ConnectToGameMessage connectMessage)
        {
            if (CheckPlayer(connectMessage))
            {
                Tile tile = Map.FindPlaceForAgent(connectMessage.AgentId, connectMessage.TeamId);
                Agent agent = new Agent(connectMessage.AgentId, false, connectMessage.TeamId)
                {
                    Tile = tile
                };
                NumberOfPlayers++;
                // Resolve leadership
                switch (connectMessage.TeamId)
                {
                    case Team.Red:
                        NumberOfTeamRedPlayers++;
                        RedTeamIds.Add(connectMessage.AgentId);
                        if ((connectMessage.WantToBeLeader || NumberOfTeamRedPlayers == Settings.NumberOfPlayers / 2)
                            && RedLeaderId == -1)
                        {
                            RedLeaderId = connectMessage.AgentId;
                            agent.IsLeader = true;
                        }
                        break;
                    case Team.Blue:
                        NumberOfTeamBluePlayers++;
                        BlueTeamIds.Add(connectMessage.AgentId);
                        if ((connectMessage.WantToBeLeader || NumberOfTeamBluePlayers == Settings.NumberOfPlayers / 2)
                            && BlueLeaderId == -1)
                        {
                            BlueLeaderId = connectMessage.AgentId;
                            agent.IsLeader = true;
                        }
                        break;
                    default:
                        break;
                }
                Agents.Add(connectMessage.AgentId, agent);
                _client.Send(new AcceptedToGameMessage
                {
                    AgentId = connectMessage.AgentId,
                    IsConnected = true,
                    RequestId = connectMessage.RequestId

                });
                PlayerConnected?.Invoke(new PlayerInfo
                {
                    Id = agent.Id,
                    Team = agent.Team,
                    IsLeader = agent.IsLeader
                });
                _logger.Debug(
                    $"Added new player on position ({tile.X}, {tile.Y}) to {connectMessage.TeamId:G} team. Current number of players: Blue={NumberOfTeamBluePlayers}, Red={NumberOfTeamRedPlayers}");
            }
            else
                _client.Send(new AcceptedToGameMessage
                {
                    AgentId = connectMessage.AgentId,
                    IsConnected = false,
                    RequestId = connectMessage.RequestId

                });
        }

        public bool InDirection(int agentId, int x, int y)
        {
            int ax = Agents[agentId].Tile.X;
            int ay = Agents[agentId].Tile.Y;
            int px = ax + x;
            int py = ay + y;

            if (!(px >= 0 && px < Map.Width && py >= 0 && py < Map.Height))
                return false;

            // Agents cannot enter enemy team's goal area
            if (Agents[agentId].Team == Team.Red)
            {
                if (py >= Settings.MapHeight - Settings.GoalAreaHeight)
                    return false;
            }
            else
            {
                if (py < Settings.GoalAreaHeight)
                    return false;
            }

            //precheck before locking, should prevent some unnecessary locks
            if (Map[px, py].AgentId != -1)
                return false;

            (int x, int y) Point1 = (ax, ay), Point2 = (px, py);
            DetermineLockOrder(Point1, Point2, out (int x, int y) Lock1, out (int x, int y) Lock2);
            lock(Map[Lock1.x,Lock1.y]) lock (Map[Lock2.x, Lock2.y])
            {
                if (Map[px,py].AgentId != -1)
                    return false;
                Map[ax, ay].UpdateTile(DateTime.Now, -1);
                Map[px, py].UpdateTile(DateTime.Now, agentId);
                Agents[agentId].Tile = Map[px, py];
            }
            return true;
        }

        private static void DetermineLockOrder((int x, int y) Point1, (int x, int y) Point2, out (int x, int y) Lock1, out (int x, int y) Lock2)
        {
            if (Point1.y < Point2.y)
            {
                Lock1 = Point1; Lock2 = Point2;
            }
            else
            {
                if (Point1.y == Point2.y)
                {
                    if (Point1.x < Point2.x)
                    {
                        Lock1 = Point1; Lock2 = Point2;
                    }
                    else
                    {
                        Lock1 = Point2; Lock2 = Point1;
                    }
                }
                else //Point1.y > Point2.y
                {
                    Lock1 = Point2; Lock2 = Point1;
                }
            }
        }

        /// <summary>
        /// Checks whether agent can move in given direction and if so gives him a distance from new position to the closest piece.
        /// </summary>
        /// <param name="moveMessage">Received move request</param>
        public void PlayerMove(MoveRequestMessage moveMessage)
        {
            _logger.Debug($"Attempt to move in {moveMessage.Direction:G} direction by agent {moveMessage.AgentId}.");
            bool permission = true;
            int x = 0, y = 0;
            switch (moveMessage.Direction)
            {
                case Direction.Up:
                    y++;
                    break;
                case Direction.Right:
                    x++;
                    break;
                case Direction.Down:
                    y--;
                    break;
                case Direction.Left:
                    x--;
                    break;
            }
            permission = InDirection(moveMessage.AgentId, x, y);
            int distanceToPiece = -1;
            long elapsedMilliseconds = _gameTimer.ElapsedMilliseconds;
            if (permission)
            {
                _logger.Debug("Move succeeded");
                Agents[moveMessage.AgentId].WaitingUntilTime
                    = new TimeSpan((elapsedMilliseconds + Settings.WaitMove * Settings.WaitBase) * TimeSpan.TicksPerMillisecond);

                distanceToPiece = Map.DistanceToPiece(Agents[moveMessage.AgentId].Tile.X, Agents[moveMessage.AgentId].Tile.Y);
                _client.Send(new MoveResultMessage
                {
                    AgentId = moveMessage.AgentId,
                    GameTimeStamp = (int)elapsedMilliseconds,
                    WaitUntilTime = (int)Agents[moveMessage.AgentId].WaitingUntilTime.TotalMilliseconds,
                    DistanceToPiece = distanceToPiece,
                    RequestId = moveMessage.RequestId
                });
            }
            else
            {
                _logger.Debug("Move failed");
                _client.Send(new CannotMoveThereMessage
                {
                    AgentId = moveMessage.AgentId,
                    GameTimeStamp = (int)elapsedMilliseconds,
                    RequestId = moveMessage.RequestId
                });
            }
        }

        /// <summary>
        /// Sends information about 9 closest tiles to the agent.
        /// </summary>
        /// <param name="discoveryMessage">Received discovery request</param>
        public void PlayerDiscovery(DiscoveryRequestMessage discoveryMessage)
        {
            _logger.Debug($"Performing discovery by agent {discoveryMessage.AgentId}");
            List<JMapTile> discoveryResults = new List<JMapTile>();
            ITile t = Agents[discoveryMessage.AgentId].Tile;
            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    int posX = t.X + i;
                    int posY = t.Y + j;
                    if (posX >= 0 && posX < Map.Width && posY >= 0 && posY < Map.Height)
                        discoveryResults.Add(new JMapTile
                        {
                            X = posX,
                            Y = posY,
                            Distance = Map.DistanceToPiece(t.X + i, t.Y + j)
                        });
                }
            }
            long elapsedMilliseconds = _gameTimer.ElapsedMilliseconds;
            Agents[discoveryMessage.AgentId].WaitingUntilTime
                    = new TimeSpan((elapsedMilliseconds + Settings.WaitDiscovery * Settings.WaitBase) * TimeSpan.TicksPerMillisecond);

            _client.Send(new DiscoveryResultMessage
            {
                AgentId = discoveryMessage.AgentId,
                GameTimeStamp = (int)elapsedMilliseconds,
                WaitUntilTime = (int)Agents[discoveryMessage.AgentId].WaitingUntilTime.TotalMilliseconds,
                DiscoveryResults = discoveryResults,
                RequestId = discoveryMessage.RequestId
            });
        }

        /// <summary>
        /// Sends result of picking up a piece to the agent.
        /// </summary>
        /// <param name="pickMessage">Received pick request</param>
        public void PlayerPickUpPiece(PickUpRequestMessage pickMessage)
        {
            Piece piece = Agents[pickMessage.AgentId].Piece;
            Tile t = Agents[pickMessage.AgentId].Tile as Tile;
            _logger.Debug($"Trying to pick up a piece on ({t.X}, {t.Y}) position by agent {pickMessage.AgentId}");
            if (piece == Piece.Null && t.Piece != Piece.Null)
            {
                Agents[pickMessage.AgentId].Piece = t.Piece;
                (Agents[pickMessage.AgentId].Tile as Tile).UpdateTile(DateTime.Now, pickMessage.AgentId, Piece.Null);
                long elapsedMilliseconds = _gameTimer.ElapsedMilliseconds;
                Agents[pickMessage.AgentId].WaitingUntilTime
                  = new TimeSpan((elapsedMilliseconds + Settings.WaitPickPiece * Settings.WaitBase) * TimeSpan.TicksPerMillisecond);

                _client.Send(new PickUpResultMessage
                {
                    AgentId = pickMessage.AgentId,
                    GameTimeStamp = (int)elapsedMilliseconds,
                    WaitUntilTime = (int)Agents[pickMessage.AgentId].WaitingUntilTime.TotalMilliseconds,
                    RequestId = pickMessage.RequestId
                });

                _logger.Debug("Picking up succeeded");
            }
            else
            {
                _client.Send(new InvalidActionMessage
                {
                    AgentId = pickMessage.AgentId,
                    RequestId = pickMessage.RequestId

                });
                _logger.Debug("Picking up failed");
            }
        }

        /// <summary>
        /// Sends result of testing a piece to the agent.
        /// </summary>
        /// <param name="testMessage">Received test request</param>
        public void PlayerTestPiece(TestPieceRequestMessage testMessage)
        {
            _logger.Debug($"Trying to test a piece by agent {testMessage.AgentId}");
            Piece piece = Agents[testMessage.AgentId].Piece;
            if (piece != Piece.Null)
            {
                bool status = piece == Piece.Real ? true : false;

                long elapsedMilliseconds = _gameTimer.ElapsedMilliseconds;
                Agents[testMessage.AgentId].WaitingUntilTime
                  = new TimeSpan((elapsedMilliseconds + Settings.WaitTestPiece * Settings.WaitBase) * TimeSpan.TicksPerMillisecond);

                _client.Send(new TestPieceResultMessage
                {
                    AgentId = testMessage.AgentId,
                    GameTimeStamp = (int)elapsedMilliseconds,
                    WaitUntilTime = (int)Agents[testMessage.AgentId].WaitingUntilTime.TotalMilliseconds,
                    IsReal = status,
                    RequestId = testMessage.RequestId
                });

                _logger.Debug("Testing piece succeeded");
            }
            else
            {
                _client.Send(new InvalidActionMessage
                {
                    AgentId = testMessage.AgentId,
                    RequestId = testMessage.RequestId

                });
                _logger.Debug("Testing piece failed");
            }
        }

        /// <summary>
        /// Sends result of destroying a piece to the agent.
        /// </summary>
        /// <param name="destroyMessage">Received destroy request</param>
        public void PlayerDestroyPiece(DestroyPieceRequestMessage destroyMessage)
        {
            Piece piece = Agents[destroyMessage.AgentId].Piece;
            _logger.Debug($"Trying to destroy a piece by agent {destroyMessage.AgentId}");
            if (piece != Piece.Null)
            {
                Agents[destroyMessage.AgentId].Piece = Piece.Null;

                long elapsedMilliseconds = _gameTimer.ElapsedMilliseconds;
                Agents[destroyMessage.AgentId].WaitingUntilTime
                  = new TimeSpan((elapsedMilliseconds + Settings.WaitDestroyPiece * Settings.WaitBase) * TimeSpan.TicksPerMillisecond);

                _client.Send(new DestroyPieceResultMessage
                {
                    AgentId = destroyMessage.AgentId,
                    GameTimeStamp = (int)elapsedMilliseconds,
                    WaitUntilTime = (int)Agents[destroyMessage.AgentId].WaitingUntilTime.TotalMilliseconds,
                    RequestId = destroyMessage.RequestId
                });

                NumberOfPieces--;
                _logger.Debug("Destroying piece succeeded");
            }
            else
            {
                _client.Send(new InvalidActionMessage
                {
                    AgentId = destroyMessage.AgentId,
                    RequestId = destroyMessage.RequestId

                });
                _logger.Debug("Destroying piece failed");
            }
        }

        /// <summary>
        /// Sends result of putting a piece to the agent.
        /// </summary>
        /// <param name="putMessage">Received put request</param>
        public void PlayerPutPiece(PutPieceRequestMessage putMessage)
        {
            Piece piece = Agents[putMessage.AgentId].Piece;
            Tile agentTile = Agents[putMessage.AgentId].Tile as Tile;
            _logger.Debug($"Trying to put up a piece on ({agentTile.X}, {agentTile.Y}) position by agent {putMessage.AgentId}");
            int discoveredGoalY = -1;
            if (piece != Piece.Null && agentTile.Piece == Piece.Null)
            {
                Result result;
                if (piece == Piece.Real)
                {
                    switch (agentTile.Type)
                    {
                        case TileType.Goal:
                            Map[agentTile.X, agentTile.Y].UpdateTile(DateTime.Now, putMessage.AgentId, Piece.Null, TileType.DiscoveredGoal);
                            Agents[putMessage.AgentId].Tile = Map[agentTile.X, agentTile.Y];
                            discoveredGoalY = agentTile.Y;
                            NumberOfPieces--;
                            Agents[putMessage.AgentId].Piece = Piece.Null;
                            result = Result.GoalCompleted;
                            break;
                        case TileType.NoGoal:
                            Map[agentTile.X, agentTile.Y].UpdateTile(DateTime.Now, putMessage.AgentId, Piece.Null, TileType.NoGoal);
                            Agents[putMessage.AgentId].Tile = Map[agentTile.X, agentTile.Y];
                            NumberOfPieces--;
                            Agents[putMessage.AgentId].Piece = Piece.Null;
                            result = Result.NonGoalDiscovered;
                            break;
                        case TileType.DiscoveredGoal:
                            Map[agentTile.X, agentTile.Y].UpdateTile(DateTime.Now, putMessage.AgentId, Piece.Null, TileType.DiscoveredGoal);
                            Agents[putMessage.AgentId].Tile = Map[agentTile.X, agentTile.Y];
                            NumberOfPieces--;
                            Agents[putMessage.AgentId].Piece = Piece.Null;
                            result = Result.GoalCompleted;
                            break;
                        case TileType.Task:
                            Map[agentTile.X, agentTile.Y].UpdateTile(DateTime.Now, putMessage.AgentId, piece, TileType.Task);
                            Agents[putMessage.AgentId].Tile = Map[agentTile.X, agentTile.Y];
                            Agents[putMessage.AgentId].Piece = Piece.Null;
                            result = Result.PiecePutInTaskArea;
                            break;
                        default:
                            throw new ArgumentException("Unexpected tile type");
                    }
                }
                else
                {
                    if (agentTile.Type == TileType.Task)
                    {
                        Map[agentTile.X, agentTile.Y].UpdateTile(DateTime.Now, putMessage.AgentId, piece, TileType.Task);
                        Agents[putMessage.AgentId].Tile = Map[agentTile.X, agentTile.Y];
                        Agents[putMessage.AgentId].Piece = Piece.Null;
                        result = Result.PiecePutInTaskArea;
                    }
                    else
                    {
                        NumberOfPieces--;
                        Agents[putMessage.AgentId].Piece = Piece.Null;
                        result = Result.FakePieceInGoalArea;
                    }

                }
                long elapsedMilliseconds = _gameTimer.ElapsedMilliseconds;
                Agents[putMessage.AgentId].WaitingUntilTime
                  = new TimeSpan((elapsedMilliseconds + Settings.WaitPutPiece * Settings.WaitBase) * TimeSpan.TicksPerMillisecond);

                _client.Send(new PutPieceResultMessage
                {
                    AgentId = putMessage.AgentId,
                    GameTimeStamp = (int)elapsedMilliseconds,
                    WaitUntilTime = (int)Agents[putMessage.AgentId].WaitingUntilTime.TotalMilliseconds,
                    Effect = result,
                    RequestId = putMessage.RequestId
                });

                Agents[putMessage.AgentId].Piece = Piece.Null;
                _logger.Debug("Putting piece succeeded");
            }
            else
            {
                _client.Send(new InvalidActionMessage
                {
                    AgentId = putMessage.AgentId,
                    RequestId = putMessage.RequestId
                });
                _logger.Debug("Putting piece failed");
            }
            if (discoveredGoalY != -1)
            {
                UpdateTeamResult(discoveredGoalY);
            }
        }

        /// <summary>
        /// Sends an info exchange request to the second agent
        /// </summary>
        /// <param name="infosMessage"></param>
        public void PlayerExchangeInfos(ExchangeInfosRequestMessage infosMessage)
        {
            _logger.Debug($"agent {infosMessage.AgentId} is trying to exchange an info with an agent {infosMessage.WithAgentId}");
            if (Agents.ContainsKey(infosMessage.WithAgentId) && !Agents[infosMessage.WithAgentId].Disconnected && Agents[infosMessage.AgentId].Team == Agents[infosMessage.WithAgentId].Team)
            {
                string key = infosMessage.AgentId.ToString() + "," + infosMessage.WithAgentId.ToString();
                if (!Exchanges.ContainsKey(key))
                {
                    Exchanges.Add(key, infosMessage.Data);
                }
                else
                {
                    _client.Send(new InvalidActionMessage
                    {
                        AgentId = infosMessage.AgentId,
                        RequestId = infosMessage.RequestId
                    });
                    _logger.Debug("Exchange infos failed, exchange exists.");
                    return;
                }

                if (Agents[infosMessage.AgentId].IsLeader)
                    Agents[infosMessage.WithAgentId].CalledByLeader = true;

                _client.Send(new ExchangeInfosAskingMessage
                {
                    AgentId = infosMessage.WithAgentId,
                    WithAgentId = infosMessage.AgentId,
                    GameTimeStamp = (int)_gameTimer.ElapsedMilliseconds,
                    RequestId = infosMessage.RequestId
                });
            }
            else
            {
                _client.Send(new InvalidActionMessage
                {
                    AgentId = infosMessage.AgentId,
                    RequestId = infosMessage.RequestId
                });
                _logger.Debug("Exchange infos failed, invalid partner ID.");
            }
        }

        /// <summary>
        /// Receives an info exchange reply from the second Agent.
        /// </summary>
        /// <param name="responseMessage">The other Agent reply.</param>
        public void PlayerRespondInfosExchange(ExchangeInfosResponseMessage responseMessage)
        {
            if (!Agents.ContainsKey(responseMessage.WithAgentId) || Agents[responseMessage.WithAgentId].Disconnected)
            {
                _client.Send(new InvalidActionMessage
                {
                    AgentId = responseMessage.AgentId,
                    RequestId = responseMessage.RequestId
                });
                return;
            }
            if (Agents[responseMessage.WithAgentId].IsLeader)
            {
                if (!responseMessage.Agreement)
                {
                    _client.Send(new InvalidActionMessage
                    {
                        AgentId = responseMessage.AgentId,
                        RequestId = responseMessage.RequestId
                    });
                    return;
                }
                Agents[responseMessage.AgentId].CalledByLeader = false;
            }

            // Sends data to the agent who firstly wanted to exchange an information
            long elapsedMilliseconds = _gameTimer.ElapsedMilliseconds;
            Agents[responseMessage.WithAgentId].WaitingUntilTime = new TimeSpan
                ((elapsedMilliseconds + Settings.WaitInfoExchange * Settings.WaitBase) * TimeSpan.TicksPerMillisecond);

            _client.Send(new ExchangeInfosDataResultMessage
            {
                AgentId = responseMessage.WithAgentId,
                WithAgentId = responseMessage.AgentId,
                Agreement = responseMessage.Agreement,
                GameTimeStamp = (int)elapsedMilliseconds,
                WaitUntilTime = (int)Agents[responseMessage.WithAgentId].WaitingUntilTime.TotalMilliseconds,
                Data = responseMessage.Data,
                RequestId = responseMessage.RequestId
            });

            // Sends data to the agent who was asked to exchange information
            string key = responseMessage.WithAgentId.ToString() + "," + responseMessage.AgentId.ToString();
            if (responseMessage.Agreement)
            {
                string data = Exchanges[key];

                elapsedMilliseconds = _gameTimer.ElapsedMilliseconds;
                Agents[responseMessage.AgentId].WaitingUntilTime
                 = new TimeSpan((elapsedMilliseconds + Settings.WaitInfoExchange * Settings.WaitBase) * TimeSpan.TicksPerMillisecond);

                _client.Send(new ExchangeInfosDataResultMessage
                {
                    AgentId = responseMessage.AgentId,
                    WithAgentId = responseMessage.WithAgentId,
                    Agreement = true,
                    GameTimeStamp = (int)elapsedMilliseconds,
                    WaitUntilTime = (int)Agents[responseMessage.AgentId].WaitingUntilTime.TotalMilliseconds,
                    Data = data,
                    RequestId = responseMessage.RequestId
                });
            }
            Exchanges.Remove(key);
        }

        /// <summary>
        /// Removes an Agent from the game after it is disconnected.
        /// </summary>
        /// <param name="message">Agent disconnected message received.</param>
        private void PlayerRemove(AgentNotRespondingMessage message)
        {
            if (!Agents.ContainsKey(message.AgentId) || Agents[message.AgentId].Disconnected)
                return;
            _logger.Error($"Agent {message.AgentId} is disconnected, removing it from the game...");
            Tile t = Map[Agents[message.AgentId].Tile.X, Agents[message.AgentId].Tile.Y];
            if (BlueTeamIds.Remove(message.AgentId))
            {
                NumberOfTeamBluePlayers--;
                if(message.AgentId == BlueLeaderId)
                {
                    BlueLeaderId = -1;
                }
            }
            else if (RedTeamIds.Remove(message.AgentId))
            {
                NumberOfTeamRedPlayers--;
                if(message.AgentId == RedLeaderId)
                {
                    RedLeaderId = -1;
                }
            }
            if (Agents.ContainsKey(message.AgentId))
            {
                Agents[message.AgentId].Disconnected = true;
                NumberOfPlayers--;
            }
            PlayerDisconnected?.Invoke(message.AgentId);
        }

        /// <summary>
        /// Updates team goals counters and winning team after an Agent put a piece on a goal tile in goal area.
        /// </summary>
        /// <param name="y">y-coordinate of the goal tile</param>
        /// <remarks>
        /// Assumes that y is already checked to be inside one of goal areas.
        /// Assumes that blue team has its goal area on the top and red team - at the bottom of the game board
        /// </remarks>
        public void UpdateTeamResult(int y)
        {
            WinningTeam = Team.None;
            if (y < Map.GoalAreaHeight)
            {
                RedTeamPoints++;
                if (RedTeamPoints == Settings.NumberOfGoalsPerTeam)
                {
                    WinningTeam = Team.Red;
                    EndGame();
                    return;
                }
            }
            else
            {
                BlueTeamPoints++;
                if (BlueTeamPoints == Settings.NumberOfGoalsPerTeam)
                {
                    WinningTeam = Team.Blue;
                    EndGame();
                    return;
                }
            }
        }

        /// <summary>
        /// Connects GM to Communication Server on TCP level.
        /// </summary>
        public bool ConnectClientToServer()
        {
            _logger.Debug("Connecting to the server");
            _client = new TcpClient(Settings.ServerIp, Settings.ServerPort);
            ServerDisconnected += () => ResetGame();
            _client.ServerDisconnected = delegate
            {
                ServerDisconnected?.Invoke();
                return true;
            };
            _client.GameMessageReceived += HandleMessage;
            return _client.ConnectToServer();
        }

        /// <summary>
        /// Connects GM to CS with a message.
        /// </summary>
        public void ConnectToServer()
        {
            _client.Send(new ConnectGmMessage());
        }

        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        public void Disconnect()
        {
            _logger.Debug("Disconnecting from the server");
            if(_client.Connected)
            {
                _client.Close();
                ResetGame();
            }
        }

        /// <summary>
        /// Reset all game parameters, so the game can be started anew.
        /// </summary>
        public void ResetGame()
        {
            _gameTimer.Reset();
            _pieceTimer.Stop();

            BlueTeamIds.Clear();
            RedTeamIds.Clear();
            BlueTeamPoints = 0;
            RedTeamPoints = 0;
            Agents.Clear();
            Exchanges.Clear();
            State = GameMasterState.Disconnected;
            _logger.Debug(
            $"GameMaster map: width={Settings.MapWidth}, height={Settings.MapHeight}, goal area height={Settings.GoalAreaHeight}, goals per team={Settings.NumberOfGoalsPerTeam}");
            Map = new Map(Settings.MapWidth, Settings.MapHeight, Settings.GoalAreaHeight, Settings.NumberOfGoalsPerTeam);
            NumberOfPieces = 0;
            NumberOfPlayers = 0;
            NumberOfTeamRedPlayers = 0;
            NumberOfTeamBluePlayers = 0;
            WinningTeam = Team.None;
            RedLeaderId = -1;
            BlueLeaderId = -1;
        }

        /// <summary>
        /// Sends a dummy message to detect server disconnected.
        /// </summary>
        /// <remarks>Used for testing.</remarks>
        public void CheckConnection()
        {
            _client.CheckConnection();
        }
    }
}
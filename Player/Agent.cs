using System;
using System.Threading;
using log4net;
using GameLibrary;
using GameLibrary.Configuration;
using GameLibrary.Enum;
using GameLibrary.Messages;
using GameLibrary.Strategies;
using System.Collections.Generic;
using System.Linq;

namespace Player
{
    /// <summary>
    /// The class to represent an Agent playing the game.
    /// </summary>
    public class Agent : AgentBase
    {
        /// <summary>
        /// The instance of a logger		
        /// </summary>		
        private readonly ILog _logger;
        /// <summary>
        /// The instance of Communication Client responsible for managing the communication for the Agent, that is, passing and receiving messages from Communication Server.
        /// </summary>
        private TcpClient _client;
        /// <summary>
        /// Stores current Agent's strategy module.
        /// </summary>
        public StrategyBase Strategy;
        /// <summary>
        /// Tells what state the Agent is in.
        /// </summary>
        public AgentState State;
        /// <summary>
        /// Stores current Agent's knowledge of the game board.
        /// </summary>
        public Map Map;
        /// <summary>
        /// Tells whether Agent will be playing according to some strategy or just react to incoming messages
        /// </summary>
        public bool IsUsingStrategy { get; set; } = true;
        /// <summary>
        /// Latest received message
        /// </summary>
        public Message ReceivedMessage { get; set; } = null;
        /// <summary>
        /// Agent configuration
        /// </summary>
        public AgentSettings Settings;
        /// <summary>
        /// Requests counter used to assign unique request ID to each message submitted.
        /// </summary>
        private int _requestId;
        /// <summary>
        /// Dictionary to store move requests that haven't been processed by GM yet.
        /// </summary>
        private Dictionary<int, Direction> moveRequests;
        /// <summary>
        /// Dictionary to hold types of unanswered request messages.
        /// </summary>
        private Dictionary<int, Type> messageTypes;
        /// <summary>
        /// Whether the Agent has recently performed a discovery action.
        /// </summary>
        private bool afterDiscovery;

        /// <summary>
        /// Events fired to signal certain state changes.
        /// </summary>
        public event Action<bool> ConnectResponse;
        public event Action GameStarted;
        public event Action<Team> GameEnded;
        public event Action ServerDisconnected;
        public event Action ReceivedMessageDuringGame;

        /// <summary>
        /// Creates a new Agent.
        /// </summary>
        /// <param name="settings">Agent settings</param>
        public Agent(AgentSettings settings)
        {
            _logger = LogManager.GetLogger(GetType());

            _logger.Debug("Creating new Agent");
            ResetGame();
            Settings = settings;
        }

        /// <summary>
        /// Handles GameMessageReceived event.
        /// </summary>
        /// <param name="message">Message received.</param>
        private void HandleMessage(Message message)
        {
            _logger.Debug($"Got new message of type {message.GetType()}");
            ReceivedMessage = message;
            switch (State)
            {
                case AgentState.Disconnected:
                    HandleMessageBeforeConnecting(message);
                    break;
                case AgentState.Connected:
                    HandleMessageBeforeGame(message);
                    break;
                case AgentState.IsPlaying:
                    HandleMessageDuringGame(message);
                    ReceivedMessageDuringGame?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Handles message received before the Agent joined a game.
        /// </summary>
        /// <param name="message">Message received.</param>
        private void HandleMessageBeforeConnecting(Message message)
        {
            switch (message)
            {
                case AcceptedToGameMessage acceptedMessage:
                    HandleAcceptedMessage(acceptedMessage);
                    return;
                case GmNotConnectedMessage gmNotConnectedMessage:
                    ConnectResponse?.Invoke(false);
                    _logger.Error("GM is not yet connected, exiting...");
                    break;
                case InvalidJsonMessage invalidJsonMessage:
                    _logger.Error("Invalid JSON message format, exiting...");
                    break;
                case InvalidActionMessage invalidActionMessage:
                    _logger.Error("Invalid message submitted, exiting...");
                    break;
                case GmNotRespondingMessage gmNotRespondingMessage:
                    _logger.Error("GM not responding, exiting...");
                    break;
                default:
                    // Not expected, something went wrong
                    _logger.Error("Unexpected message format received, exiting...");
                    break;
            }
            Disconnect();
        }

        /// <summary>
        /// Handles message received before the Agent starts playing.
        /// </summary>
        /// <param name="message">Message received.</param>
        private void HandleMessageBeforeGame(Message message)
        {
            switch (message)
            {
                case GameInfoMessage gameInfoMessage:
                    HandleGameInfoMessage(gameInfoMessage);
                    GameStarted?.Invoke();
                    break;
                case GmNotRespondingMessage gmNotRespondingMessage:
                    _logger.Error("GM not responding, exiting...");
                    Disconnect();
                    break;
                default:
                    // Unexpected, something went wrong
                    _logger.Error($"Unexpected message format received.");
                    break;
            }
        }

        /// <summary>
        /// Handles message received when the Agent plays the game.
        /// </summary>
        /// <param name="message">Message received.</param>
        private void HandleMessageDuringGame(Message message)
        {
            switch (message)
            {
                case GameEndedMessage gameEnded:
                    GameEnded?.Invoke(gameEnded.WinningTeam);
                    _logger.Info($"Agent {Id} is finishing the game, congrats to team {gameEnded.WinningTeam}");
                    Disconnect();
                    break;
                case MoveResultMessage moveResult:
                    GetMovePermission(moveResult);
                    break;
                case DiscoveryResultMessage discoveryResult:
                    GetDiscoveryResults(discoveryResult);
                    break;
                case PickUpResultMessage pickResult:
                    GetPickUpPermission(pickResult);
                    break;
                case TestPieceResultMessage testResult:
                    GetTestPermission(testResult);
                    break;
                case DestroyPieceResultMessage destroyResult:
                    GetDestroyPermission(destroyResult);
                    break;
                case PutPieceResultMessage putResult:
                    GetPutPermission(putResult);
                    break;
                case ExchangeInfosDataResultMessage exchangeResult:
                    GetExchangeInfosPermission(exchangeResult);
                    break;
                case ExchangeInfosAskingMessage askingMessage:
                    GetExchangeInfosAsking(askingMessage);
                    break;
                case CannotMoveThereMessage cannotMoveThereMessage:
                    if (moveRequests.ContainsKey(cannotMoveThereMessage.RequestId))
                        moveRequests.Remove(cannotMoveThereMessage.RequestId);
                    if (messageTypes.ContainsKey(cannotMoveThereMessage.RequestId))
                        messageTypes.Remove(cannotMoveThereMessage.RequestId);
                    Strategy.CouldntMove++;
                    break;
                case RequestDuringPenaltyMessage requestDuringPenaltyMessage:
                    Strategy.WaitingTime = new TimeSpan((requestDuringPenaltyMessage.WaitUntilTime - requestDuringPenaltyMessage.GameTimeStamp) * TimeSpan.TicksPerMillisecond);
                    break;
                case GmNotRespondingMessage gmNotRespondingMessage:
                    _logger.Error("GM not responding, exiting...");
                    Disconnect();
                    return;
                case InvalidJsonMessage invalidJsonMessage:
                    _logger.Error("Invalid JSON message format.");
                    break;
                case InvalidActionMessage invalidActionMessage:
                    _logger.Error("Invalid message submitted.");
                    if (messageTypes.ContainsKey(invalidActionMessage.RequestId) && messageTypes[invalidActionMessage.RequestId] == typeof(PickUpRequestMessage))
                    {
                        Map[Tile.X, Tile.Y].UpdateTile(DateTime.Now, -1);
                        Tile = Map[Tile.X, Tile.Y];
                    }
                    if (messageTypes.ContainsKey(invalidActionMessage.RequestId))
                        messageTypes.Remove(invalidActionMessage.RequestId);
                    if (moveRequests.ContainsKey(invalidActionMessage.RequestId))
                        moveRequests.Remove(invalidActionMessage.RequestId);
                    break;
                default:
                    // Not expected, something went wrong
                    _logger.Error("Unexpected message format received.");
                    break;
            }
            if (message is ResultMessage)
            {
                Strategy.WaitingTime = new TimeSpan(((message as ResultMessage).WaitUntilTime - (message as ResultMessage).GameTimeStamp) * TimeSpan.TicksPerMillisecond);
                if (Strategy.WaitingTime.TotalMilliseconds < 0)
                    Strategy.WaitingTime = TimeSpan.Zero;
            }
            _logger.Info($"Agent {Id} is waiting for {(int)Strategy.WaitingTime.TotalMilliseconds} ms");
            Thread.Sleep((int)Strategy.WaitingTime.TotalMilliseconds);
            Strategy.WaitingTime = TimeSpan.Zero;
            if (IsUsingStrategy && !(message is ExchangeInfosAskingMessage))
            {
                if (message is ExchangeInfosDataResultMessage)
                {
                    if (messageTypes.ContainsKey(message.RequestId))
                    {
                        messageTypes.Remove(message.RequestId);
                        UseStrategy();
                    }
                }
                else
                    UseStrategy();
            }
        }

        /// <summary>
        /// Handles acceptance message from Game Master.
        /// </summary>
        /// <param name="acceptedMessage">Accepted to game reply</param>
        private void HandleAcceptedMessage(AcceptedToGameMessage acceptedMessage)
        {
            if (!Settings.IsLoggingEnabled)
            {
                log4net.Repository.Hierarchy.Hierarchy h = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
                log4net.Repository.Hierarchy.Logger rootLogger = h.Root;
                rootLogger.RemoveAllAppenders();
            }

            if (acceptedMessage.IsConnected)
            {
                _logger.Info($"Agent in {Team} was accepted, got an id {acceptedMessage.AgentId}");
                Id = acceptedMessage.AgentId;
                State = AgentState.Connected;
            }
            else
            {
                _logger.Info($"Agent in {Team} got rejected, exiting...");
            }
            ConnectResponse?.Invoke(acceptedMessage.IsConnected);
        }


        /// <summary>
        /// Handles game info message from Game Master.
        /// </summary>
        /// <param name="acceptedMessage">Game info from Game Master</param>
        private void HandleGameInfoMessage(GameInfoMessage gameInfoMessage)
        {
            int BoardHeight = gameInfoMessage.TaskAreaHeight + 2 * gameInfoMessage.GoalAreaHeight;
            _logger.Debug($"Setting Agent map to new map (width: {gameInfoMessage.AgentMapWidth}, height: {BoardHeight}, goal are height: {gameInfoMessage.GoalAreaHeight})");
            Map = new Map(gameInfoMessage.AgentMapWidth, BoardHeight, gameInfoMessage.GoalAreaHeight);
            _logger.Debug($"Setting Agent Tile to ({gameInfoMessage.InitialXPosition}, {gameInfoMessage.InitialYPosition})");
            Tile = Map[gameInfoMessage.InitialXPosition, gameInfoMessage.InitialYPosition];
            var ids = new List<int>(gameInfoMessage.AgentIdsFromTeam);
            ids.Remove(gameInfoMessage.AgentId);
            var teammatesIDs = ids.ToArray();
            Strategy = StrategyBase.GetStrategy(Settings.Strategy, Team, Map.Height, Map.Width, Map.GoalAreaHeight, teammatesIDs, gameInfoMessage.TeamLeaderId);

            moveRequests = new Dictionary<int, Direction>();
            messageTypes = new Dictionary<int, Type>();
            afterDiscovery = false;

            Interlocked.Add(ref _requestId, Id * 1000000);

            if (Strategy is SuperiorStrategy superiorStrategy)
            {
                List<int> allTeamIds = new List<int>(gameInfoMessage.AgentIdsFromTeam);
                allTeamIds.Add(Id);
                allTeamIds = allTeamIds.OrderBy(id => id).ToList();
                float jump = (float)Map.Width / (allTeamIds.Count / 2);
                if (allTeamIds.IndexOf(Id) < (allTeamIds.Count / 2)) //first line
                {
                    if (Team == Team.Red)
                    {
                        superiorStrategy.HomeY = Map.GoalAreaHeight + gameInfoMessage.TaskAreaHeight / 3;
                    }
                    else
                    {
                        superiorStrategy.HomeY = Map.Height - Map.GoalAreaHeight - gameInfoMessage.TaskAreaHeight / 3;
                    }
                    superiorStrategy.HomeX = (int)(jump * allTeamIds.IndexOf(Id)) + 1;
                }
                else //second line
                {
                    if (Team == Team.Red)
                    {
                        superiorStrategy.HomeY = Map.GoalAreaHeight + gameInfoMessage.TaskAreaHeight * 2 / 3;
                    }
                    else
                    {
                        superiorStrategy.HomeY = Map.Height - Map.GoalAreaHeight - gameInfoMessage.TaskAreaHeight * 2 / 3;
                    }
                    superiorStrategy.HomeX = (int)(jump * (allTeamIds.IndexOf(Id) - (allTeamIds.Count / 2)));
                }

                List<int> idsInLine = new List<int>();
                for (int i = 0; i < allTeamIds.Count / 2; i++)
                {
                    idsInLine.Add(allTeamIds[i]);
                    idsInLine.Add(allTeamIds[i + (allTeamIds.Count / 2)]);
                }
                float jumpGoal = (float)Map.Width / allTeamIds.Count;
                superiorStrategy.BaseGoalX = (int)(jumpGoal * idsInLine.IndexOf(Id));
                if (Team == Team.Red)
                {
                    superiorStrategy.BaseGoalY = Map.GoalAreaHeight;
                }
                else
                {
                    superiorStrategy.BaseGoalY = Map.Height - Map.GoalAreaHeight;
                }

                superiorStrategy.GoingHome = true;

                _logger.Debug($"Setting Agent Home to ({superiorStrategy.HomeX}, {superiorStrategy.HomeY})");
                _logger.Debug($"Setting Agent BaseGoal to ({superiorStrategy.BaseGoalX}, {superiorStrategy.BaseGoalY})");
            }

            State = AgentState.IsPlaying;
            Strategy.WaitingTime = TimeSpan.Zero;
            if (IsUsingStrategy)
                UseStrategy();
        }

        /// <summary>
        /// The Agent tries to move in a given direction.
        /// </summary>
        /// <param name="direction">Direction in which Agent wants to move</param>
        public void Move(Direction direction)
        {
            _logger.Debug($"Sending move request in ({direction:G}) direction");
            int requestId = Interlocked.Increment(ref _requestId);
            _client.Send(new MoveRequestMessage { Direction = direction, AgentId = Id, RequestId = requestId });
            moveRequests.Add(requestId, direction);
            messageTypes.Add(requestId, typeof(MoveRequestMessage));
        }

        /// <summary>
        /// The Agent tries to get an information about 8 tiles around him (3x3).
        /// </summary>
        public void Discovery()
        {
            _logger.Debug($"Sending discovery request");
            int requestId = Interlocked.Increment(ref _requestId);
            _client.Send(new DiscoveryRequestMessage { AgentId = Id, RequestId = requestId });
            messageTypes.Add(requestId, typeof(DiscoveryRequestMessage));
        }

        /// <summary>
        /// The Agent tries to pick up piece underneath him.
        /// </summary>
        public void PiecePickUp()
        {
            _logger.Debug($"Sending pick up piece request");
            int requestId = Interlocked.Increment(ref _requestId);
            _client.Send(new PickUpRequestMessage { AgentId = Id, RequestId = requestId });
            messageTypes.Add(requestId, typeof(PickUpRequestMessage));
        }
        /// <summary>
        /// The Agent tries to test the held piece.
        /// </summary>
        public void PieceTest()
        {
            _logger.Debug($"Sending test piece request");
            int requestId = Interlocked.Increment(ref _requestId);
            _client.Send(new TestPieceRequestMessage { AgentId = Id, RequestId = requestId });
            messageTypes.Add(requestId, typeof(TestPieceRequestMessage));
        }
        /// <summary>
        /// The Agent tries to destroy the held piece.
        /// </summary>
        public void PieceDestroy()
        {
            _logger.Debug($"Sending destroy piece request");
            int requestId = Interlocked.Increment(ref _requestId);
            _client.Send(new DestroyPieceRequestMessage { AgentId = Id, RequestId = requestId });
            messageTypes.Add(requestId, typeof(DestroyPieceRequestMessage));
        }
        /// <summary>
        /// The Agent tries to put the held piece.
        /// </summary>
        public void PiecePut()
        {
            _logger.Debug($"Sending put piece request");
            int requestId = Interlocked.Increment(ref _requestId);
            _client.Send(new PutPieceRequestMessage { AgentId = Id, RequestId = requestId });
            messageTypes.Add(requestId, typeof(PutPieceRequestMessage));
        }

        /// <summary>
        /// The Agent tries to exchange information with someone from team
        /// </summary>
        /// <param name="withAgentId"></param>
        public void InfosExchange(int withAgentId)
        {
            _logger.Debug($"Sending exchange info request");
            int requestId = Interlocked.Increment(ref _requestId);

            Strategy.WaitingForExchangeAnswer = true;

            _client.Send(new ExchangeInfosRequestMessage
            {
                AgentId = Id,
                WithAgentId = withAgentId,
                Data = Map.GetDataStringFromMap(Map),
                RequestId = requestId
            });
            messageTypes.Add(requestId, typeof(ExchangeInfosRequestMessage));
        }

        /// <summary>
        /// The Agent gets reply from the GameMaster and update his map if got a permission.
        /// </summary>
        /// <param name="resultMessage"></param>
        private void GetExchangeInfosPermission(ExchangeInfosDataResultMessage resultMessage)
        {
            if (resultMessage.Agreement)
            {
                _logger.Debug($"Getting exchange info data");
                Map infoMap = Map.GetMapFromDataString(resultMessage.Data, Map.Width, Map.Height, Map.GoalAreaHeight);
                Map.UpdateAfterExchangeInfo(infoMap);
                Tile = Map[Tile.X, Tile.Y];
            }
            else
            {
                _logger.Debug($"Exchange info request was rejected.");
            }

            if (Strategy is NormalStrategy normalStrategy)
            {
                normalStrategy.ExchangeInfoTargets.Remove(resultMessage.AgentId);
                normalStrategy.ExchangeInfoTargets.Add(resultMessage.AgentId);
                normalStrategy.DonePutPieceActions = 0;
            }
            Strategy.WaitingForExchangeAnswer = false;
        }

        /// <summary>
        /// The Agent gets a request message for the exchange from the GameMaster.
        /// </summary>
        /// <param name="askingMessage"></param>
        private void GetExchangeInfosAsking(ExchangeInfosAskingMessage askingMessage)
        {
            _logger.Debug("Received exchange info request");
            bool agreement = false;
            string data = "";
            Random rand = new Random();
            if (true)//(rand.Next(2) == 1) || askingMessage.WithAgentId == Strategy.LeaderId)
            {
                _logger.Debug($"Agent {Id} agreed to exchange info with Agent {askingMessage.WithAgentId}");
                agreement = true;
                data = Map.GetDataStringFromMap(Map);

                if (Strategy is NormalStrategy normalStrategy)
                {
                    normalStrategy.ExchangeInfoTargets.Remove(askingMessage.WithAgentId);
                    normalStrategy.ExchangeInfoTargets.Add(askingMessage.WithAgentId);
                }
            }
            _client.Send(new ExchangeInfosResponseMessage
            {
                AgentId = Id,
                WithAgentId = askingMessage.WithAgentId,
                Agreement = agreement,
                Data = data,
                RequestId = askingMessage.RequestId
            });
        }

        /// <summary>
        /// The Agent gets reply from the GameMaster and picks up piece if got a permission.
        /// </summary>
        /// <param name="pickResult">Pick reply</param>
        internal void GetPickUpPermission(PickUpResultMessage pickResult)
        {
            _logger.Debug($"Picked up a piece");
            Map[Tile.X, Tile.Y].UpdateTile(DateTime.Now, -1);
            Tile = Map[Tile.X, Tile.Y];
            Strategy.Piece = PieceStatus.Unidentified;
            if (messageTypes.ContainsKey(pickResult.RequestId))
                messageTypes.Remove(pickResult.RequestId);
        }

        /// <summary>
        /// The Agent gets reply from the GameMaster and updates info about its piece.
        /// </summary>
        /// <param name="testResult">Rest reply</param>
        internal void GetTestPermission(TestPieceResultMessage testResult)
        {
            _logger.Debug($"Tested a piece");
            if (testResult.IsReal)
                Strategy.Piece = PieceStatus.Real;
            else
                Strategy.Piece = PieceStatus.Sham;
            if (messageTypes.ContainsKey(testResult.RequestId))
                messageTypes.Remove(testResult.RequestId);
        }

        /// <summary>
        /// The Agent gets reply from the GameMaster and destroys held piece if got a permission.
        /// </summary>
        /// <param name="destroyResult">Destroy reply</param>
        internal void GetDestroyPermission(DestroyPieceResultMessage destroyResult)
        {
            _logger.Debug($"Destroyed a piece");
            Strategy.Piece = PieceStatus.None;
            if (messageTypes.ContainsKey(destroyResult.RequestId))
                messageTypes.Remove(destroyResult.RequestId);
        }

        /// <summary>
        /// The Agent gets reply from the GameMaster and puts down held piece if got a permission, then updates the map.
        /// </summary>
        /// <param name="putResult">Put reply</param>
        internal void GetPutPermission(PutPieceResultMessage putResult)
        {
            _logger.Debug($"Put a piece");
            Strategy.Piece = PieceStatus.None;
            switch (putResult.Effect)
            {
                case Result.GoalCompleted:
                    _logger.Debug($"Goal completed");
                    Map[Tile.X, Tile.Y].UpdateTile(Map[Tile.X, Tile.Y].Timestamp, -1, TileType.DiscoveredGoal);
                    break;
                case Result.NonGoalDiscovered:
                    _logger.Debug($"Non-goal tile discovered");
                    Map[Tile.X, Tile.Y].UpdateTile(Map[Tile.X, Tile.Y].Timestamp, -1, TileType.NoGoal);
                    break;
                default:
                    _logger.Debug($"Got no information after putting a piece");
                    Map[Tile.X, Tile.Y].UpdateTile(DateTime.Now, -1);
                    break;
            }
            Tile = Map[Tile.X, Tile.Y];
            if (messageTypes.ContainsKey(putResult.RequestId))
                messageTypes.Remove(putResult.RequestId);
            if (Strategy is NormalStrategy normalStrategy)
                normalStrategy.DonePutPieceActions++;
        }

        /// <summary>
        /// The Agent gets a reply from the GameMaster and moves if got a permission.
        /// </summary>
        /// <param name="moveResult">Move reply</param>
        public void GetMovePermission(MoveResultMessage moveResult)
        {
            Direction direction = moveRequests[moveResult.RequestId];
            _logger.Debug($"Moved to ({direction:G}) direction");
            int lastDistanceToPiece = Map[Tile.X, Tile.Y].DistanceToPiece;
            switch (direction)
            {
                case Direction.Up:
                    Tile = Map[Tile.X, Tile.Y + 1];
                    break;
                case Direction.Down:
                    Tile = Map[Tile.X, Tile.Y - 1];
                    break;
                case Direction.Right:
                    Tile = Map[Tile.X + 1, Tile.Y];
                    break;
                case Direction.Left:
                    Tile = Map[Tile.X - 1, Tile.Y];
                    break;
            }
            Strategy.CouldntMove = 0;
            Map[Tile.X, Tile.Y].UpdateTile(DateTime.Now, moveResult.DistanceToPiece);
            Tile = Map[Tile.X, Tile.Y];
            Strategy.IsGettingCloser = (moveResult.DistanceToPiece < lastDistanceToPiece);
            if (moveRequests.ContainsKey(moveResult.RequestId))
                moveRequests.Remove(moveResult.RequestId);
            if (messageTypes.ContainsKey(moveResult.RequestId))
                messageTypes.Remove(moveResult.RequestId);
        }

        /// <summary>
        /// The Agent gets a reply from the GameMaster and updates map.
        /// </summary>
        /// <param name="discoveryResult">Discovery reply</param>
        public void GetDiscoveryResults(DiscoveryResultMessage discoveryResult)
        {
            Map.UpdateAfterDiscovery(discoveryResult.DiscoveryResults, DateTime.Now);
            _logger.Info($"Agent {Id} got discovery results.");
            if (messageTypes.ContainsKey(discoveryResult.RequestId))
                moveRequests.Remove(discoveryResult.RequestId);
            afterDiscovery = true;
        }

        /// <summary>
        /// The method that applies an action recommended by the strategy pursued.
        /// </summary>
        private void UseStrategy()
        {
            if (Strategy.Piece == PieceStatus.Real && (Strategy is NormalStrategy normalStrategy))
            {
                if (normalStrategy.GoingToX == -1 || normalStrategy.GoingToY == -1 || Map[normalStrategy.GoingToX, normalStrategy.GoingToY].Type != TileType.Unknown)
                {
                    int goToX = Tile.X;
                    int goToY = Tile.Y;

                    if (Strategy is SuperiorStrategy superiorStrategy)
                    {
                        goToX = superiorStrategy.BaseGoalX;
                        goToY = superiorStrategy.BaseGoalY;
                    }
                    for (int i = 0; i < Math.Max(Map.Width, Map.Height); i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (Map[goToX + i, goToY + j] != null && Map[goToX + i, goToY + j].Type == TileType.Unknown)
                            {
                                normalStrategy.GoingToX = goToX + i;
                                normalStrategy.GoingToY = goToY + j;
                                break;
                            }
                            if (Map[goToX - i, goToY + j] != null && Map[goToX - i, goToY + j].Type == TileType.Unknown)
                            {
                                normalStrategy.GoingToX = goToX - i;
                                normalStrategy.GoingToY = goToY + j;
                                break;
                            }
                            if (Map[goToX + i, goToY - j] != null && Map[goToX + i, goToY - j].Type == TileType.Unknown)
                            {
                                normalStrategy.GoingToX = goToX + i;
                                normalStrategy.GoingToY = goToY - j;
                                break;
                            }
                            if (Map[goToX - i, goToY - j] != null && Map[goToX - i, goToY - j].Type == TileType.Unknown)
                            {
                                normalStrategy.GoingToX = goToX - i;
                                normalStrategy.GoingToY = goToY - j;
                                break;
                            }
                        }
                        if (normalStrategy.GoingToX != -1)
                            break;
                        for (int j = 0; j < i; j++)
                        {
                            if (Map[goToX + j, goToY + i] != null && Map[goToX + j, goToY + i].Type == TileType.Unknown)
                            {
                                normalStrategy.GoingToX = goToX + j;
                                normalStrategy.GoingToY = goToY + i;
                                break;
                            }
                            if (Map[goToX - j, goToY + i] != null && Map[goToX - j, goToY + i].Type == TileType.Unknown)
                            {
                                normalStrategy.GoingToX = goToX - j;
                                normalStrategy.GoingToY = goToY + i;
                                break;
                            }
                            if (Map[goToX + j, goToY - i] != null && Map[goToX + j, goToY - i].Type == TileType.Unknown)
                            {
                                normalStrategy.GoingToX = goToX + j;
                                normalStrategy.GoingToY = goToY - i;
                                break;
                            }
                            if (Map[goToX - j, goToY - i] != null && Map[goToX - j, goToY - i].Type == TileType.Unknown)
                            {
                                normalStrategy.GoingToX = goToX - j;
                                normalStrategy.GoingToY = goToY - i;
                                break;
                            }
                        }
                        if (normalStrategy.GoingToX != -1)
                            break;
                    }
                }
            }

            if (Strategy is NormalStrategy && afterDiscovery)
            {
                List<Direction> bestMoves = new List<Direction>();
                int bestDistance = int.MaxValue;
                if (Tile.X + 1 < Map.Width && Map[Tile.X + 1, Tile.Y].DistanceToPiece <= bestDistance)
                {
                    if (Map[Tile.X + 1, Tile.Y].DistanceToPiece < bestDistance)
                    {
                        bestMoves.Clear();
                    }
                    bestMoves.Add(Direction.Right);
                    bestDistance = Map[Tile.X + 1, Tile.Y].DistanceToPiece;
                }
                if (Tile.X - 1 >= 0 && Map[Tile.X - 1, Tile.Y].DistanceToPiece <= bestDistance)
                {
                    if (Map[Tile.X - 1, Tile.Y].DistanceToPiece < bestDistance)
                    {
                        bestMoves.Clear();
                    }
                    bestMoves.Add(Direction.Left);
                    bestDistance = Map[Tile.X - 1, Tile.Y].DistanceToPiece;
                }
                if (Tile.Y + 1 < Map.Height && Map[Tile.X, Tile.Y + 1].DistanceToPiece <= bestDistance)
                {
                    if (Map[Tile.X, Tile.Y + 1].DistanceToPiece < bestDistance)
                    {
                        bestMoves.Clear();
                    }
                    bestMoves.Add(Direction.Up);
                    bestDistance = Map[Tile.X, Tile.Y + 1].DistanceToPiece;
                }
                if (Tile.Y - 1 >= 0 && Map[Tile.X, Tile.Y - 1].DistanceToPiece <= bestDistance)
                {
                    if (Map[Tile.X, Tile.Y - 1].DistanceToPiece < bestDistance)
                    {
                        bestMoves.Clear();
                    }
                    bestMoves.Add(Direction.Down);
                    bestDistance = Map[Tile.X, Tile.Y - 1].DistanceToPiece;
                }
                afterDiscovery = false;
                Random rand = new Random();
                Move(bestMoves[rand.Next(bestMoves.Count)]);
            }
            else
                switch (Strategy.UseStrategy(Tile, (Tile as Tile).DistanceToPiece))
                {
                    case Actions.MoveUp:
                        Move(Direction.Up);
                        break;
                    case Actions.MoveDown:
                        Move(Direction.Down);
                        break;
                    case Actions.MoveLeft:
                        Move(Direction.Left);
                        break;
                    case Actions.MoveRight:
                        Move(Direction.Right);
                        break;
                    case Actions.Discovery:
                        Discovery();
                        break;
                    case Actions.DestroyPiece:
                        PieceDestroy();
                        break;
                    case Actions.TestPiece:
                        PieceTest();
                        break;
                    case Actions.PutPiece:
                        PiecePut();
                        break;
                    case Actions.PickPiece:
                        PiecePickUp();
                        break;
                    case Actions.InfoExchange:
                        InfosExchange(Strategy.ExchangeInfoTarget());
                        break;
                }
        }

        /// <summary>
        /// Connect to Communication Server.
        /// </summary>
        public bool ConnectToServer()
        {
            _logger.Debug("Connecting to the server");
            _client = new TcpClient(Settings.ServerIp, Settings.ServerPort);
            _client.GameMessageReceived += HandleMessage;
            ServerDisconnected += () => ResetGame();
            _client.ServerDisconnected += delegate
            {
                ServerDisconnected?.Invoke();
                return true;
            };
            IsLeader = Settings.WantBeALeader;
            Team = Settings.Team;
            return _client.ConnectToServer();
        }

        /// <summary>
        /// Connects Agent to game with a message.
        /// </summary>
        public void ConnectToGame()
        {
            _logger.Debug("Agent is joining a new game...");
            _client.Send(new ConnectToGameMessage { TeamId = Team, WantToBeLeader = IsLeader, RequestId = Interlocked.Increment(ref _requestId) });
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
        /// Resets all fields to initial state.
        /// </summary>
        private void ResetGame()
        {
            State = AgentState.Disconnected;
            ReceivedMessage = null;
            Map = null;
            Tile = null;
            Id = 0;
            Strategy = null;
            _requestId = 0;
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
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;
using log4net;
using WatsonTcp;
using GameLibrary.Enum;
using GameLibrary.Serialization;
using GameLibrary.Messages;

namespace Server
{
    /// <summary>
    /// Represents game Communication Server.
    /// </summary>
    public partial class TcpServer: WatsonTcpServer
    {
        /// <summary>
        /// The instance of a _logger
        /// </summary>
        private readonly ILog _logger;
        /// <summary>
        /// Dictionary matching agent IDs and IP:port strings.
        /// </summary>
        public Dictionary<int, string> AgentIdsToIpPort;
        /// <summary>
        /// ID that will be assigned to the next Agent, if any.
        /// </summary>
        private int _currentId;
        /// <summary>
        /// Stores GM IP:port string.
        /// </summary>
        public string GmIpPort;
        /// <summary>
        /// Tells what state the server is in.
        /// </summary>
        public ServerState State;
        /// <summary>
        /// Game over message send repeatedly after finishing the game.
        /// </summary>
        public GameEndedMessage GameEndedMessage;
        /// <summary>
        /// Lock to synchronize adding and removal of Agents from the dictionary.
        /// </summary>
        private object _assignAgentIdLock = new object();
        /// <summary>
        /// Event fired when a client disconnects from the server.
        /// </summary>
        public event Action ClientDisconnectedEvent;

        /// <summary>
        /// Creates an instance of server.
        /// </summary>
        /// <param name="ip">Server IP</param>
        /// <param name="port">Server port</param>
        public TcpServer(string ip, int port): base(ip, port)
        {
            ClientConnected = ClientConnectedCallback;
            ClientDisconnected = ClientDisconnectedCallback;
            MessageReceived = MessageReceivedCallback;
            AgentIdsToIpPort = new Dictionary<int, string>();
            Debug = false;
            ResetGame();
            _logger = LogManager.GetLogger(GetType());
            _logger.Debug($"Creating new communication server with {ip} ip and {port} port");
        }

        /// <summary>
        /// Server listening loop.
        /// </summary>
        public void Listen()
        {
            Start();
            _logger.Info("Communication server starts...");
        }

        /// <summary>
        /// Method invoked each time a new client connects to the server.
        /// </summary>
        /// <param name="ipPort">IP and port of the client.</param>
        /// <returns></returns>
        bool ClientConnectedCallback(string ipPort)
        {
            _logger.Debug("Client connected: " + ipPort);
            return true;
        }

        /// <summary>
        /// Method invoked each time a client disconnects from the server.
        /// </summary>
        /// <param name="ipPort">IP and port of the client.</param>
        /// <returns></returns>
        bool ClientDisconnectedCallback(string ipPort)
        {
            _logger.Debug("Client disconnected: " + ipPort);
            {
                if (ipPort == GmIpPort)
                {
                        ManageDisconnectedGM();
                }
                else
                {
                    int agentId = -1;
                    lock (_assignAgentIdLock)
                    {
                        if (AgentIdsToIpPort.Count > 0)
                        {
                            IEnumerable<int> ids = AgentIdsToIpPort.Where(entry => entry.Value == ipPort).Select(entry => entry.Key);
                            if (ids.Count() > 0) agentId = ids.First();
                        }
                    }
                    ManageDisconnectedAgent(agentId);
                }
            }
            
            return true;
        }

        /// <summary>
        /// Method invoked each time the server gets a message.
        /// </summary>
        /// <param name="ipPort">IP and port of the message sender.</param>
        /// <param name="data">Content of the message</param>
        /// <returns></returns>
        bool MessageReceivedCallback(string ipPort, byte[] data)
        {
            string JSONMessage = Encoding.UTF8.GetString(data);
            _logger.Debug("Message received from " + ipPort + ": " + JSONMessage);
            try
            {
                Message message = Serializer.Deserialize(JSONMessage);

                // GM not yet connected, waiting for it to connect
                if(State == ServerState.GmNotConnected)
                {
                    ProcessMessageBeforeGMConnected(ipPort, message);
                    return true;
                }

                // GM connected, but game not started yet
                if(State == ServerState.GmConnected)
                {
                    ProcessConnectToGameMessage(ipPort, message);
                    return true;
                }

                // The game is running
                if (State == ServerState.GameRunning)
                {
                    if (ipPort == GmIpPort)
                    {
                        ProcessGMMessage(message);
                    }
                    else if(AgentIdsToIpPort.ContainsValue(ipPort)) // Will not pass message from removed Agent
                    {
                        ProcessAgentMessage(ipPort, message);
                    }
                    return true;
                }

                // State == ServerState.GameEnded
                // Cleaning after game end
                ProcessMessageAfterGameEnd(ipPort, message);
            }
            catch(Exception e)
            {
                _logger.Debug("Invalid JSON format detected, sending invalid JSON message back");
                InvalidJsonMessage ijm = new InvalidJsonMessage() { AgentId = -1 };
                Send(ipPort, ijm);
            }
            return true;
        }

        /// <summary>
        /// Checks and passes messages when no Game Master is connected. Connects GM when appropriate message comes.
        /// </summary>
        /// <param name="fromIpPort">IP and port where the message came from.</param>
        /// <param name="message">Message received.</param>
        private void ProcessMessageBeforeGMConnected(string fromIpPort, Message message)
        {
            if (message.GetType() == typeof(ConnectGmMessage)) // GM join to game request
            {
                if (GmIpPort != "")
                {
                    Send(fromIpPort, new AcceptedGmMessage() { IsConnected = false });
                    _logger.Info("Rejected Game Master at: " + fromIpPort);
                }
                else
                {
                    GmIpPort = fromIpPort;
                    Send(fromIpPort, new AcceptedGmMessage() { IsConnected = true });
                    State = ServerState.GmConnected;
                    _logger.Info("Accepted Game Master at: " + fromIpPort);
                }
            }
            else if (message.GetType() == typeof(ConnectToGameMessage))
            {
                Send(fromIpPort, new GmNotConnectedMessage());
                _logger.Debug("Sending GM not yet connected message to early bird");
            }
            else
            {
                Send(fromIpPort, new InvalidActionMessage() { AgentId = -1 });
                _logger.Debug($"Invalid message detected: {message.GetType()}, sending invalid action message");
            }
        }

        /// <summary>
        /// Checks and passes messages before the game starts. Assigns IDs to new Agents.
        /// </summary>
        /// <param name="fromIpPort">IP and port where the message came from.</param>
        /// <param name="message">Message received.</param>
        private void ProcessConnectToGameMessage(string fromIpPort, Message message)
        {
            if (fromIpPort == GmIpPort)
            {
                if (message.GetType() == typeof(AcceptedToGameMessage))
                {
                    SendAcceptedToGameMessage(message as AcceptedToGameMessage);
                }
                else if(message.GetType() == typeof(GameInfoMessage))
                {
                    SendGameStartMessage(message as GameInfoMessage);
                }
                else
                {
                    _logger.Debug("Invalid message detected, sending invalid action message");
                    SendToGM(new InvalidActionMessage() { AgentId = -1 });
                }
            }
            else
            {
                if (message.GetType() == typeof(ConnectToGameMessage))
                {
                    SendConnectToGameMessage(fromIpPort, message as ConnectToGameMessage);
                }
                else
                {
                    _logger.Debug("Invalid message detected, sending invalid action message");
                    Send(fromIpPort, new InvalidActionMessage() { AgentId = -1 });
                }
            }
        }

        /// <summary>
        /// Checks and passes messages from Game Master.
        /// </summary>
        /// <param name="message">Message received.</param>
        private void ProcessGMMessage(Message message)
        {
            switch (message)
            {
                case RequestDuringPenaltyMessage requestDuringPenaltyMessage:
                    SendRequestDuringTimePenaltyMessage(requestDuringPenaltyMessage);
                    break;
                case CannotMoveThereMessage cannotMoveThereMessage:
                    SendCannotMoveMessage(cannotMoveThereMessage);
                    break;
                case InvalidActionMessage invalidActionMessage:
                    SendInvalidActionMessage(invalidActionMessage);
                    break;
                case GameInfoMessage gameInfoMessage:
                    SendGameStartMessage(gameInfoMessage);
                    break;
                case GameEndedMessage gameEndedMessage:
                    SendGameOverMessageToAll(gameEndedMessage);
                    break;
                case ExchangeInfosAskingMessage exchangeInfosAskingMessage:
                    SendActionCommunicationRequestMessage(exchangeInfosAskingMessage);
                    break;
                case ResultMessage resultMessage:
                    SendActionResponseMessage(resultMessage);
                    break;
                default:
                    _logger.Debug("Invalid message from GM detected, sending invalid action message");
                    SendToGM(new InvalidActionMessage() { AgentId = -1 });
                    break;
            }
        }

        /// <summary>
        /// Sends game over message as a response to any coming message and disconnects the client.
        /// </summary>
        /// <param name="fromIpPort">IP and port where the message came from.</param>
        /// <param name="message">Message received.</param>
        private void ProcessMessageAfterGameEnd(string fromIpPort, Message message)
        {
            int agentId = AgentIdsToIpPort.Where(entry => entry.Value == fromIpPort)
                                   .Select(entry => entry.Key).FirstOrDefault();
            GameEndedMessage.RequestId = message.RequestId;
            SendGameOverMessage(agentId, fromIpPort);
            GameEndedMessage.RequestId = 0;
            if(AgentIdsToIpPort.Count == 0)
            {
                ResetGame();
            }
        }

        /// <summary>
        /// Checks and passes messages from an Agent.
        /// </summary>
        /// <param name="fromIpPort">IP and port of the agent.</param>
        /// <param name="message">Message received.</param>
        private void ProcessAgentMessage(string fromIpPort, Message message)
        {
            switch(message)
            {
                case RequestMessage requestMessage:
                    SendActionRequestMessage(requestMessage);
                    break;
                case ExchangeInfosResponseMessage exchangeInfosResponseMessage:
                    SendExchangeInfoAgreementMessage(exchangeInfosResponseMessage);
                    break;
                default:
                    _logger.Debug("Invalid message detected, sending invalid action message");
                    Send(fromIpPort, new InvalidActionMessage() { AgentId = -1 });
                    break;
            }
        }

        /// <summary>
        /// Sends a broadcast message to all agents to inform about GM connection loss.
        /// </summary>
        private void ManageDisconnectedGM()
        {
            if(State == ServerState.GameEnded)
            {
                return;
            }

            _logger.Error("GM not responding, sending a broadcast message");
            lock (_assignAgentIdLock)
            {
                foreach (int id in AgentIdsToIpPort.Keys)
                {
                    if (AgentIdsToIpPort.TryGetValue(id, out string ipPort))
                    {
                        string JSONMessage = Serializer.Serialize(new GmNotRespondingMessage()
                        { AgentId = id });
                        try
                        {
                            Send(ipPort, Encoding.UTF8.GetBytes(JSONMessage));
                            DisconnectClient(AgentIdsToIpPort[id]);
                        }
                        catch { }
                    }
                }
            }
            ResetGame();
            ClientDisconnectedEvent?.Invoke();
        }

        /// <summary>
        /// Informs GM about disconnected agent and removes it from agents collection.
        /// </summary>
        /// <param name="agentId">ID of disconnected agent.</param>
        private void ManageDisconnectedAgent(int agentId)
        {
            lock (_assignAgentIdLock)
            {
                if (AgentIdsToIpPort.Remove(agentId))
                {
                    if (State == ServerState.GmConnected || State == ServerState.GameRunning)
                    {
                        _logger.Debug($"Agent {agentId} not responding, sending a message back to GM...");
                        string JSONMessage = Serializer.Serialize(new AgentNotRespondingMessage()
                        { AgentId = agentId });
                        try
                        {
                            Send(GmIpPort, Encoding.UTF8.GetBytes(JSONMessage));
                        }
                        catch
                        {

                        }
                    }
                    ClientDisconnectedEvent?.Invoke();
                    if(State == ServerState.GameEnded && AgentIdsToIpPort.Count == 0)
                    {
                        ResetGame();
                    }
                }
            }
        }

        /// <summary>
        /// Resets all fields to initial state.
        /// </summary>
        private void ResetGame()
        {
            State = ServerState.GmNotConnected;
            GmIpPort = "";
            GameEndedMessage = null;
            AgentIdsToIpPort.Clear();
            _currentId = 0;
        }

        /// <summary>
        /// Forces server to close connections.
        /// </summary>
        /// <remarks>Used for tests.</remarks>
        public void CloseServer()
        {
            ResetGame();
            Dispose();
        }
    }
}
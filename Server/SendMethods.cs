using System.Text;
using WatsonTcp;
using GameLibrary.Enum;
using GameLibrary.Serialization;
using GameLibrary.Messages;

namespace Server
{
    public partial class TcpServer : WatsonTcpServer
    {
        /// <summary>
        /// Send a message to given ip and port
        /// </summary>
        /// <param name="ipPort">String in format "ip:port", determining message address.</param>
        /// <param name="message">Message to be sent.</param>
        /// <remarks>Used before GM is connected and server has no known IPs.</remarks>
        private void Send(string ipPort, Message message)
        {
            string JSONMessage = Serializer.Serialize(message);
            Send(ipPort, Encoding.UTF8.GetBytes(JSONMessage));
        }

        /// <summary>
        /// Send a message to the agent of given id.
        /// </summary>
        /// <param name="agentId">Id of the receiving Agent.</param>
        /// <param name="message">Message to be sent.</param>
        private void SendToAgent(int agentId, Message message)
        {
            string JSONMessage = Serializer.Serialize(message);
            if (AgentIdsToIpPort.TryGetValue(agentId, out string ipPort))
            {
                if(!Send(ipPort, Encoding.UTF8.GetBytes(JSONMessage)))
                {
                    ManageDisconnectedAgent(agentId);
                }
            }
        }

        /// <summary>
        /// Send a message to GM
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        private void SendToGM(Message message)
        {
            string JSONMessage = Serializer.Serialize(message);
            if (!Send(GmIpPort, Encoding.UTF8.GetBytes(JSONMessage)))
            {
                ManageDisconnectedGM();
            }
        }

        private void SendAcceptedToGameMessage(AcceptedToGameMessage message)
        {
            if(!message.IsConnected)
            {
                _logger.Debug($"Removing rejected agent {message.AgentId}");
                string ipPort = AgentIdsToIpPort[message.AgentId];
                lock (_assignAgentIdLock)
                {
                    // Removing it here, so in ManageDisconnectedClient() it will not trigger a message to GM
                    AgentIdsToIpPort.Remove(message.AgentId);
                }
                DisconnectClient(ipPort);
            }
            else
            {
                _logger.Debug($"Passing accepted to game message to agent {message.AgentId}");
                SendToAgent(message.AgentId, message);
            }
        }

        private void SendGameStartMessage(GameInfoMessage message)
        {
            _logger.Debug($"The game has started, passing game start message to agent {message.AgentId}");
            State = ServerState.GameRunning;
            SendToAgent(message.AgentId, message);
        }

        private void SendGameOverMessageToAll(GameEndedMessage gameEndedMessage)
        {
            _logger.Debug($"The game has finished, passing game over message to all agents");
            GameEndedMessage = gameEndedMessage;
            State = ServerState.GameEnded;
            foreach(int agentId in AgentIdsToIpPort.Keys)
            {
                GameEndedMessage.AgentId = agentId;
                Send(AgentIdsToIpPort[agentId], GameEndedMessage);
                DisconnectClient(AgentIdsToIpPort[agentId]);
            }
        }

        private void SendGameOverMessage(int agentId, string fromIpPort)
        {
            _logger.Debug($"The game has finished, passing game over message to agent {GameEndedMessage.AgentId}");
            SendToAgent(GameEndedMessage.AgentId, GameEndedMessage);
            DisconnectClient(fromIpPort);
        }

        private void SendInvalidActionMessage(InvalidActionMessage message)
        {
            _logger.Debug($"Invalid message detected, sending invalid action message to Agent {message.AgentId}");
            SendToAgent(message.AgentId, message);
        }

        private void SendConnectToGameMessage(string fromIpPort, ConnectToGameMessage message)
        {
            lock (_assignAgentIdLock)
            {
                message.AgentId = _currentId++;
                AgentIdsToIpPort.Add(message.AgentId, fromIpPort);
            }

            _logger.Info($"Assigned ID {message.AgentId} to Agent at {fromIpPort}, sending join game request to GM...");
            SendToGM(message);
        }

        private void SendRequestDuringTimePenaltyMessage(RequestDuringPenaltyMessage message)
        {
            _logger.Debug($"Passing request during time penalty message to agent {message.AgentId}");
            SendToAgent(message.AgentId, message);
        }

        private void SendCannotMoveMessage(CannotMoveThereMessage message)
        {
            _logger.Debug($"Passing cannot move message to agent {message.AgentId}");
            SendToAgent(message.AgentId, message);
        }

        private void SendActionCommunicationRequestMessage(ExchangeInfosAskingMessage message)
        {
            _logger.Debug($"Passing GM info exchange request to agent {message.AgentId}");
            SendToAgent(message.AgentId, message);
        }

        private void SendActionResponseMessage(ResultMessage message)
        {
            _logger.Debug($"Passing action response message to agent {message.AgentId}");
            SendToAgent(message.AgentId, message);
        }

        private void SendActionRequestMessage(RequestMessage message)
        {
            _logger.Debug($"Passing action request message from agent {message.AgentId} to GM");
            SendToGM(message);
        }

        private void SendExchangeInfoAgreementMessage(ExchangeInfosResponseMessage message)
        {
            _logger.Debug($"Passing exchange info agreement message from agent {message.AgentId} to GM");
            SendToGM(message);
        }
    }
}

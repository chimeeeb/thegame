using GameLibrary.Messages;
using GameLibrary.Serialization;
using log4net;
using System.Text;
using WatsonTcp;

namespace GameLibrary
{
    /// <summary>
    /// A class that manages TCP connections for GM and Agents.
    /// </summary>
    public class TcpClient : WatsonTcpClient
    {
        /// <summary>
        /// The instance of a logger
        /// </summary>
        protected ILog Logger;
        /// <summary>
        /// Event invoked when a message is received - to be managed by Agent / GM.
        /// </summary>
        public event MessageHandler GameMessageReceived;
        public delegate void MessageHandler(Message args);

        /// <summary>
        /// Creates a basic TCP client.
        /// </summary>
        /// <param name="serverIp">Server IP</param>
        /// <param name="serverPort">Server port</param>
        public TcpClient(string serverIp, int serverPort) : base(serverIp, serverPort)
        {
            ServerConnected = ServerConnectedCallback;
            ServerDisconnected = ServerDisconnectedCallback;
            MessageReceived = MessageReceivedCallback;
            Debug = false;
            Logger = LogManager.GetLogger(GetType());
            Logger.Debug($"Created new communication client");
        }

        /// <summary>
        /// Start TCPClient and connect it to the server.
        /// </summary>
        public bool ConnectToServer()
        {
            Logger.Info("Communication client is starting...");
            try
            {
                Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Closes client
        /// </summary>
        public void Close()
        {
            Dispose();
            Logger.Info("Communication client closed...");
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="message">Message to be sent</param>
        public void Send(Message message)
        {
            string JSONMessage = Serializer.Serialize(message);
            if(!Send(Encoding.UTF8.GetBytes(JSONMessage)))
            {
                Logger.Error("Server disconnected");
                Close();
            }
        }

        /// <summary>
        /// Method invoked each time the client connects to a server.
        /// </summary>
        /// <param name="ipPort">IP and port of the client.</param>
        /// <returns></returns>
        bool ServerConnectedCallback()
        {
            Logger.Debug("Server connected");
            return true;
        }

        /// <summary>
        /// Method invoked each time the client disconnects from a server.
        /// </summary>
        /// <param name="ipPort">IP and port of the client.</param>
        /// <returns></returns>
        bool ServerDisconnectedCallback()
        {
            Logger.Debug("Server disconnected");
            return true;
        }

        /// <summary>
        /// Method invoked each time the client gets a message from the server.
        /// </summary>
        /// <param name="data">Content of the message</param>
        /// <returns></returns>
        bool MessageReceivedCallback(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                string messageString = Encoding.UTF8.GetString(data);
                Logger.Debug("Message from server: " + messageString);
                Message message = Serializer.Deserialize(messageString);
                if (message == null) return false;
                GameMessageReceived?.Invoke(message);
            }
            return true;
        }

        /// <summary>
        /// Sends a dummy message to detect server disconnected.
        /// </summary>
        /// <remarks>Used for testing.</remarks>
        public void CheckConnection()
        {
            Send(new InvalidActionMessage { AgentId = -1 });
        }
    }
}

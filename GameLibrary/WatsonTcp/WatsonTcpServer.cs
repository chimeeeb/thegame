using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using WatsonTcp.Message;

namespace WatsonTcp
{
    /// <summary>
    /// Watson TCP server, without SSL.
    /// </summary>
    public class WatsonTcpServer : IDisposable
    {
        #region Public-Members

        /// <summary>
        /// Buffer size to use when reading input and output streams.  Default is 65536.
        /// </summary>
        public int ReadStreamBufferSize
        {
            get
            {
                return _ReadStreamBufferSize;
            }
            set
            {
                if (value < 1) throw new ArgumentException("Read stream buffer size must be greater than zero.");
                _ReadStreamBufferSize = value;
            }
        }

        /// <summary>
        /// Enable or disable console debugging.
        /// </summary>
        public bool Debug = false;

        /// <summary>
        /// Method to call when a client connects to the server.  
        /// The IP:port is passed to this method as a string, and it is expected that the method will return true.
        /// </summary>
        public Func<string, bool> ClientConnected = null;

        /// <summary>
        /// Method to call when a client disconnects from the server.  
        /// The IP:port is passed to this method as a string, and it is expected that the method will return true.
        /// </summary>
        public Func<string, bool> ClientDisconnected = null;

        /// <summary>
        /// Method to call when a message is received from a client.  
        /// The IP:port is passed to this method as a string, along with a byte array containing the message data.  
        /// It is expected that the method will return true.
        /// </summary>
        public Func<string, byte[], bool> MessageReceived = null;

        #endregion

        #region Private-Members

        private bool _Disposed = false;
        private int _ReadStreamBufferSize = 65536;
        private string _ListenerIp;
        private int _ListenerPort; 
        private IPAddress _ListenerIpAddress;
        private TcpListener _Listener;

        private int _ActiveClients;
        private ConcurrentDictionary<string, ClientMetadata> _Clients;

        private readonly SemaphoreSlim _SendLock;
        private CancellationTokenSource _TokenSource;
        private CancellationToken _Token;

        #endregion

        #region Constructors-and-Factories
         
        /// <summary>
        /// Initialize the Watson TCP server without SSL.  Call Start() afterward to start Watson.
        /// </summary>
        /// <param name="listenerIp">The IP address on which the server should listen, nullable.</param>
        /// <param name="listenerPort">The TCP port on which the server should listen.</param>
        public WatsonTcpServer(
            string listenerIp,
            int listenerPort)
        {
            if (listenerPort < 1) throw new ArgumentOutOfRangeException(nameof(listenerPort));

            if (String.IsNullOrEmpty(listenerIp))
            {
                _ListenerIpAddress = IPAddress.Any;
                _ListenerIp = _ListenerIpAddress.ToString();
            }
            else
            {
                _ListenerIpAddress = IPAddress.Parse(listenerIp);
                _ListenerIp = listenerIp;
            }

            _ListenerPort = listenerPort;
            
            _Listener = new TcpListener(_ListenerIpAddress, _ListenerPort);

            _TokenSource = new CancellationTokenSource();
            _Token = _TokenSource.Token;

            _ActiveClients = 0;
            _Clients = new ConcurrentDictionary<string, ClientMetadata>();
            _SendLock = new SemaphoreSlim(1);
        }
        
        #endregion

        #region Public-Methods

        /// <summary>
        /// Tear down the server and dispose of background workers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Start the server.  
        /// </summary>
        public void Start()
        {
            Log("Watson TCP server starting on " + _ListenerIp + ":" + _ListenerPort);

            Task.Run(() => AcceptConnections(), _Token);
        }

        /// <summary>
        /// Send data to the specified client.
        /// </summary>
        /// <param name="ipPort">IP:port of the recipient client.</param>
        /// <param name="data">Byte array containing data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public bool Send(string ipPort, byte[] data)
        {
            if (!_Clients.TryGetValue(ipPort, out ClientMetadata client))
            {
                Log("*** Send unable to find client " + ipPort);
                return false;
            }

            WatsonMessage msg = new WatsonMessage(data, Debug);
            return MessageWrite(client, msg, data);
        }

        /// <summary>
        /// Disconnects the specified client.
        /// </summary>
        public void DisconnectClient(string ipPort)
        {
            if (!_Clients.TryGetValue(ipPort, out ClientMetadata client))
            {
                Log("*** DisconnectClient unable to find client " + ipPort);
            }
            else
            {
                client.Dispose();
            }
        }

        #endregion

        #region Private-Methods

        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed)
            {
                return;
            }

            if (disposing)
            {
                _TokenSource.Cancel();
                _TokenSource.Dispose();

                if (_Listener != null && _Listener.Server != null)
                {
                    _Listener.Server.Close();
                    _Listener.Server.Dispose();
                }

                if (_Clients != null && _Clients.Count > 0)
                {
                    foreach (KeyValuePair<string, ClientMetadata> currMetadata in _Clients)
                    {
                        currMetadata.Value.Dispose();
                    }
                }
            }

            _SendLock.Dispose(); 
            _Disposed = true;
        }

        private void Log(string msg)
        {
            if (Debug) Console.WriteLine(msg);
        }

        private async Task AcceptConnections()
        {
            _Listener.Start();
            while (!_Token.IsCancellationRequested)
            {
                string clientIpPort = String.Empty;

                try
                {
                    TcpClient tcpClient = await _Listener.AcceptTcpClientAsync();
                    tcpClient.LingerState.Enabled = false;

                    string clientIp = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();

                    ClientMetadata client = new ClientMetadata(tcpClient);
                    clientIpPort = client.IpPort;

                    Task unawaited = Task.Run(() =>
                        {
                            FinalizeConnection(client);
                        }, _Token);

                    Log("*** AcceptConnections accepted connection from " + client.IpPort);
                }
                catch (Exception e)
                {
                    Log("*** AcceptConnections exception " + clientIpPort + " " + e.Message);
                }
            }
        }

        private void FinalizeConnection(ClientMetadata client)
        {
            if (!AddClient(client))
            {
                Log("*** FinalizeConnection unable to add client " + client.IpPort);
                client.Dispose();
                return;
            }

            // Do not decrement in this block, decrement is done by the connection reader
            int activeCount = Interlocked.Increment(ref _ActiveClients);
            
            Log("*** FinalizeConnection starting data receiver for " + client.IpPort + " (now " + activeCount + " clients)");
            if (ClientConnected != null)
            {
                Task.Run(() => ClientConnected(client.IpPort));
            }

            Task.Run(async () => await DataReceiver(client));
        }

        private bool IsConnected(ClientMetadata client)
        {
            if (client.TcpClient.Connected)
            {
                if ((client.TcpClient.Client.Poll(0, SelectMode.SelectWrite)) 
                    && (!client.TcpClient.Client.Poll(0, SelectMode.SelectError)))
                {
                    byte[] buffer = new byte[1];
                    if (client.TcpClient.Client.Receive(buffer, SocketFlags.Peek) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private async Task DataReceiver(ClientMetadata client)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        if (!IsConnected(client))
                        {
                            break;
                        }

                        WatsonMessage msg = await MessageReadAsync(client);
                        if (msg == null)
                        {
                            // no message available
                            await Task.Delay(30);
                            continue;
                        }

                        if (MessageReceived != null)
                        {
                            Task<bool> unawaited = Task.Run(() => MessageReceived(client.IpPort, msg.Data));
                        }
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
            finally
            {
                int activeCount = Interlocked.Decrement(ref _ActiveClients);
                RemoveClient(client);

                if (ClientDisconnected != null)
                {
                    Task<bool> unawaited = Task.Run(() => ClientDisconnected(client.IpPort));
                }

                Log("*** DataReceiver client " + client.IpPort + " disconnected (now " + activeCount + " clients active)");
                client.Dispose();
            }
        }

        private bool AddClient(ClientMetadata client)
        {
            _Clients.TryRemove(client.IpPort, out ClientMetadata removedClient);
            _Clients.TryAdd(client.IpPort, client);

            Log("*** AddClient added client " + client.IpPort);
            return true;
        }

        private bool RemoveClient(ClientMetadata client)
        {
            _Clients.TryRemove(client.IpPort, out ClientMetadata removedClient);

            Log("*** RemoveClient removed client " + client.IpPort);
            return true;
        }

        private async Task<WatsonMessage> MessageReadAsync(ClientMetadata client)
        {
            /*
             *
             * Do not catch exceptions, let them get caught by the data reader
             * to destroy the connection
             *
             */

            WatsonMessage msg = null;
            msg = new WatsonMessage(client.NetworkStream, Debug);
            await msg.Build();
            return msg;
        }

        private bool MessageWrite(ClientMetadata client, WatsonMessage msg, byte[] data)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (msg == null) throw new ArgumentNullException(nameof(msg));

            int dataLen = 0;
            if (data != null) dataLen = data.Length;

            _SendLock.Wait();

            try
            {
                if (dataLen > 0) client.NetworkStream.Write(data, 0, dataLen);
                client.NetworkStream.Flush();

                return true;
            }
            catch (Exception)
            {
                Log("*** MessageWrite " + client.IpPort + " disconnected due to exception");
                return false;
            }
            finally
            {
                _SendLock.Release();
            }
        }
        #endregion
    }
}

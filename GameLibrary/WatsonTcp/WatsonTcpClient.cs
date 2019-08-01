using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using WatsonTcp.Message;

namespace WatsonTcp
{
    /// <summary>
    /// Watson TCP client, with or without SSL.
    /// </summary>
    public class WatsonTcpClient : IDisposable
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
        /// Function called when a message is received.  
        /// A byte array containing the message data is passed to this function.
        /// It is expected that 'true' will be returned.
        /// </summary>
        public Func<byte[], bool> MessageReceived = null;

        /// <summary>
        /// Function called when the client successfully connects to the server.
        /// It is expected that 'true' will be returned.
        /// </summary>
        public Func<bool> ServerConnected = null;

        /// <summary>
        /// Function called when the client disconnects from the server.
        /// It is expected that 'true' will be returned.
        /// </summary>
        public Func<bool> ServerDisconnected = null;

        /// <summary>
        /// Indicates whether or not the client is connected to the server.
        /// </summary>
        public bool Connected { get; private set; }

        #endregion

        #region Private-Members

        private bool _Disposed = false;
        private int _ReadStreamBufferSize = 65536;
        private string _SourceIp;
        private int _SourcePort;
        private string _ServerIp;
        private int _ServerPort; 
        private TcpClient _Client;

        
        private readonly SemaphoreSlim _SendLock;
        private CancellationTokenSource _TokenSource;
        private CancellationToken _Token;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Initialize the Watson TCP client without SSL.  Call Start() afterward to connect to the server.
        /// </summary>
        /// <param name="serverIp">The IP address or hostname of the server.</param>
        /// <param name="serverPort">The TCP port on which the server is listening.</param>
        public WatsonTcpClient(
            string serverIp,
            int serverPort)
        {
            if (String.IsNullOrEmpty(serverIp)) throw new ArgumentNullException(nameof(serverIp)); 
            if (serverPort < 1) throw new ArgumentOutOfRangeException(nameof(serverPort));
            _ServerIp = serverIp;
            _ServerPort = serverPort;
            _SendLock = new SemaphoreSlim(1);
        }
        #endregion

        #region Public-Methods

        /// <summary>
        /// Tear down the client and dispose of background workers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Start the client and establish a connection to the server.
        /// </summary>
        public void Start()
        {
            _Client = new TcpClient();
            IAsyncResult asyncResult = null;
            WaitHandle waitHandle = null;

            Log("Watson TCP client connecting to " + _ServerIp + ":" + _ServerPort);

            _Client.LingerState = new LingerOption(true, 0);
            asyncResult = _Client.BeginConnect(_ServerIp, _ServerPort, null, null);
            waitHandle = asyncResult.AsyncWaitHandle;

            try
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5), false))
                {
                    _Client.Close();
                    throw new TimeoutException("Timeout connecting to " + _ServerIp + ":" + _ServerPort);
                }

                _Client.EndConnect(asyncResult);

                _SourceIp = ((IPEndPoint)_Client.Client.LocalEndPoint).Address.ToString();
                _SourcePort = ((IPEndPoint)_Client.Client.LocalEndPoint).Port;
                Connected = true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                waitHandle.Close();
            }

            if (ServerConnected != null)
            {
                Task.Run(() => ServerConnected());
            }

            _TokenSource = new CancellationTokenSource();
            _Token = _TokenSource.Token;
            Task.Run(async () => await DataReceiver(_Token), _Token);
        }


        /// <summary>
        /// Send data to the server.
        /// </summary>
        /// <param name="data">Byte array containing data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public bool Send(byte[] data)
        {
            return MessageWrite(data);
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
                
                if (_Client != null)
                {
                    if (_Client.Connected)
                    {
                        try
                        {
                            NetworkStream ns = _Client.GetStream();
                            if (ns != null) ns.Close();
                        }
                        catch (Exception)
                        {

                        }
                    }
                    try
                    {
                        _Client.Close();
                    }
                    catch (Exception)
                    {

                    }
                }

                _TokenSource.Cancel();
                _TokenSource.Dispose();

                _SendLock.Dispose();

                Connected = false;
            }

            _Disposed = true;
        }
         

        private void Log(string msg)
        {
            if (Debug)
            {
                Console.WriteLine(msg);
            }
        }

        private void LogException(string method, Exception e)
        {
            Log("================================================================================");
            Log(" = Method: " + method);
            Log(" = Exception Type: " + e.GetType().ToString());
            Log(" = Exception Data: " + e.Data);
            Log(" = Inner Exception: " + e.InnerException);
            Log(" = Exception Message: " + e.Message);
            Log(" = Exception Source: " + e.Source);
            Log(" = Exception StackTrace: " + e.StackTrace);
            Log("================================================================================");
        }

        private async Task DataReceiver(CancellationToken? cancelToken=null)
        {
            try
            {
                while (true)
                {
                    cancelToken?.ThrowIfCancellationRequested();

                    if (_Client == null)
                    {
                        Log("*** DataReceiver null TCP interface detected, disconnection or close assumed");
                        break;
                    }

                    if (!_Client.Connected)
                    {
                        Log("*** DataReceiver server disconnected");
                        break;
                    }

                    WatsonMessage msg = new WatsonMessage(_Client.GetStream(), Debug);

                    await msg.Build();

                    if (msg == null)
                    {
                        await Task.Delay(30);
                        continue;
                    }

                    if (MessageReceived != null)
                    {
                        Task<bool> unawaited = Task.Run(() => MessageReceived(msg.Data));
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (ObjectDisposedException)
            {

            }
            catch (Exception e)
            {
                if (Debug)
                {
                    Log("*** DataReceiver server disconnected");
                    Log(Common.SerializeJson(e));
                }
            }
            finally
            {
                Connected = false;
                ServerDisconnected?.Invoke(); 
            }
        }

        private bool MessageWrite(byte[] data)
        {
            bool disconnectDetected = false;
            long dataLen = 0;
            if (data != null) dataLen = data.Length;

            try
            {
                if (_Client == null)
                {
                    Log("MessageWrite client is null");
                    disconnectDetected = true;
                    return false;
                }

                WatsonMessage msg = new WatsonMessage(data, Debug);
               
                _SendLock.Wait();
                try
                {
                    NetworkStream ns = _Client.GetStream();
                    if (data != null && data.Length > 0) ns.Write(data, 0, data.Length);
                    ns.Flush();
                }
                finally
                {
                    _SendLock.Release();
                }

                return true;
            }
            catch (ObjectDisposedException ObjDispInner)
            {
                Log("*** MessageWrite server disconnected (obj disposed exception): " + ObjDispInner.Message);
                disconnectDetected = true;
                return false;
            }
            catch (SocketException SockInner)
            {
                Log("*** MessageWrite server disconnected (socket exception): " + SockInner.Message);
                disconnectDetected = true;
                return false;
            }
            catch (InvalidOperationException InvOpInner)
            {
                Log("*** MessageWrite server disconnected (invalid operation exception): " + InvOpInner.Message);
                disconnectDetected = true;
                return false;
            }
            catch (IOException IOInner)
            {
                Log("*** MessageWrite server disconnected (IO exception): " + IOInner.Message);
                disconnectDetected = true;
                return false;
            }
            catch (Exception e)
            {
                LogException("MessageWrite", e);
                disconnectDetected = true;
                return false;
            }
            finally
            {
                if (disconnectDetected)
                {
                    Connected = false;
                    Dispose();
                }
            }
        }
        #endregion
    }
}

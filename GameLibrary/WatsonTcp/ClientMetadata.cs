using System;
using System.Net.Sockets;

namespace WatsonTcp
{
    public class ClientMetadata : IDisposable
    {
        private bool _Disposed = false;

        private TcpClient _TcpClient;
        private NetworkStream _NetworkStream;
        private string _IpPort;

        public ClientMetadata(TcpClient tcp)
        {
            _TcpClient = tcp ?? throw new ArgumentNullException(nameof(tcp));

            _NetworkStream = tcp.GetStream();

            _IpPort = tcp.Client.RemoteEndPoint.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TcpClient TcpClient
        {
            get { return _TcpClient; }
        }

        public NetworkStream NetworkStream
        {
            get { return _NetworkStream; }
        }

        public string IpPort
        {
            get { return _IpPort; }
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed)
            {
                return;
            }

            if (disposing)
            {

                if (_NetworkStream != null)
                {
                    _NetworkStream.Close();
                }

                if (_TcpClient != null)
                {
                    _TcpClient.Close();
                }
            }

            _Disposed = true;
        }
    }
}

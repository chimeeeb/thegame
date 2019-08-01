using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WatsonTcp.Message
{
    internal class WatsonMessage
    {
        #region Public-Members

        /// <summary>
        /// Length of all header fields and payload data.
        /// </summary>
        internal long Length { get; set; }

        /// <summary>
        /// Length of the data.
        /// </summary>
        internal long ContentLength { get; set; }

        /// <summary>
        /// Message data.
        /// </summary>
        internal byte[] Data { get; set; }

        /// <summary>
        /// Stream containing the message data.
        /// </summary>
        internal Stream DataStream { get; set; }

        #endregion

        private bool _Debug = false;
        private NetworkStream _NetworkStream;

        #region Constructors-and-Factories

        /// <summary>
        /// Do not use.
        /// </summary>
        internal WatsonMessage()
        {
        }

        /// <summary>
        /// Construct a new message to send.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="debug">Enable or disable debugging.</param>
        internal WatsonMessage(byte[] data, bool debug)
        {
            if (data == null || data.Length < 1) throw new ArgumentNullException(nameof(data));
            
            ContentLength = data.Length;
            Data = new byte[data.Length];
            Buffer.BlockCopy(data, 0, Data, 0, data.Length);
            DataStream = null; 

            _Debug = debug;
        }

        /// <summary>
        /// Construct a new message to send.
        /// </summary>
        /// <param name="contentLength">The number of bytes included in the stream.</param>
        /// <param name="stream">The stream containing the data.</param>
        /// <param name="debug">Enable or disable debugging.</param>
        internal WatsonMessage(long contentLength, Stream stream, bool debug)
        {
            if (contentLength < 0) throw new ArgumentException("Content length must be zero or greater.");
            if (contentLength > 0)
            {
                if (stream == null || !stream.CanRead)
                {
                    throw new ArgumentException("Cannot read from supplied stream.");
                }
            }

            ContentLength = contentLength;
            Data = null;
            DataStream = stream; 
            
            _Debug = debug;
        }

        /// <summary>
        /// Read from a TCP-based stream and construct a message.  Call Build() to populate.
        /// </summary>
        /// <param name="stream">NetworkStream.</param>
        /// <param name="debug">Enable or disable console debugging.</param>
        internal WatsonMessage(NetworkStream stream, bool debug)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException("Cannot read from stream.");

            _NetworkStream = stream;
            _Debug = debug;
        }
        #endregion

        #region Public-Methods

        /// <summary>
        /// Awaitable async method to build the Message object from data that awaits in a NetworkStream or SslStream, returning the full message data.
        /// </summary>
        /// <returns>Always returns true (void cannot be a return parameter).</returns>
        internal async Task<bool> Build()
        {
            using (MemoryStream msgLengthMs = new MemoryStream())
            {
                while (true)
                {
                    byte[] data = await ReadFromNetwork(1, "MessageLength");
                    if (data[0] == 10)
                    {
                        Data = msgLengthMs.ToArray();
                        return true;
                    }
                    await msgLengthMs.WriteAsync(data, 0, 1);
                }
            }
        }
        #endregion

        #region Private-Methods

        private async Task<byte[]> ReadFromNetwork(long count, string field)
        {
            if (_Debug) Console.WriteLine("ReadFromNetwork " + count + " " + field);
            string logMessage = null;

            try
            {
                if (count <= 0) return null;
                int read = 0;
                byte[] buffer = new byte[count];
                byte[] ret = null;

                InitByteArray(buffer);

                if (_NetworkStream != null)
                { 
                    while (true)
                    {
                        read = await _NetworkStream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == count)
                        {
                            ret = new byte[read];
                            Buffer.BlockCopy(buffer, 0, ret, 0, read);
                            break;
                        }
                    } 
                }
                if (ret != null && ret.Length > 0) logMessage = ByteArrayToHex(ret);
                else logMessage = "(null)";
                return ret;
            }
            finally
            {
                if (_Debug) Console.WriteLine("- Result: " + field + " " + count + ": " + logMessage);
            }
        }
        

        private void InitByteArray(byte[] data)
        {
            if (data == null || data.Length < 1) throw new ArgumentNullException(nameof(data));
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0x00;
            }
        }

        private string ByteArrayToHex(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data) hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        #endregion
    }
}

using System.Net.Sockets;

namespace Content.Communication.Protocol
{
    internal class AsyncStateObject
    {
        // Size of receive buffer.  
        public const int BufferSize = 4096;

        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];

        // Client socket.
        public Socket workSocket = null;
    }
}
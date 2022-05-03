using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Content.Communication
{
    public class Client
    {
        public Socket socket;
        private string _ip;
        private int _port;

        public Client(string ip, int port)
        {
            _ip = ip;
            _port = port;
            
            socket = new Socket(
                AddressFamily.InterNetwork, 
                SocketType.Stream, 
                ProtocolType.Tcp);
        }

        public void Connect()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _port);

            try
            {
                Debug.Log("Trying to connect with Server...");
                socket.BeginConnect(localEndPoint, ServerConnected, socket);
            }
            catch (Exception ex)
            {
                Debug.Log("Cannot connect to server. Reason : " + ex.Message);
            }
        }

        private void ServerConnected(IAsyncResult ar)
        {
            Debug.Log("Connect with server Complete!");
        }
    }
}

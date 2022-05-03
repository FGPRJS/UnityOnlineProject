using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using UnityEngine;
using UnityOnlineProjectProtocol;
using UnityOnlineProjectProtocol.Connection;
using UnityOnlineProjectProtocol.Protocol;

namespace Content.Communication
{
    public class Client
    {
        public Socket socket;
        private string _ip;
        private int _port;

        private byte[] sendBuffer;

        public Client(string ip, int port)
        {
            _ip = ip;
            _port = port;

            socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            socket.ReceiveBufferSize = AsyncStateObject.BufferSize;
            socket.SendBufferSize = AsyncStateObject.BufferSize;
        }

        public void Connect()
        {
            Debug.Log("Trying to connect to server...");
            
            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _port);

            try
            {
                socket.BeginConnect(localEndPoint, Connected, socket);
            }
            catch (Exception ex)
            {
                Debug.Log("Cannot connect to server. Reason : " + ex.Message);
            }
        }

        private void Connected(IAsyncResult ar)
        {
            socket.EndConnect(ar);
            
            Debug.Log("Successfully connected!");
            
            //Temp
            CommunicationMessage congratulationMessage = new CommunicationMessage();
            congratulationMessage.Message = "Hello! I'm new client!";
            SendData(congratulationMessage);
        }

        public void SendData(CommunicationMessage message)
        {
            Debug.Log("Trying to send...");

            try
            {
                var data = CommunicationUtility.Serialize(message);

                socket.BeginSend(
                    data,
                    0,
                    data.Length,
                    SocketFlags.None,
                    SendComplete,
                    socket);
            }
            catch (Exception ex)
            {
                Debug.Log("Send Failure. Reason : " + ex.Message);
            }
            
        }

        private void SendComplete(IAsyncResult ar)
        {
            socket.EndSend(ar);
            
            Debug.Log("Send Complete!");
        }
    }
}

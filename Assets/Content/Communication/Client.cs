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
        [NonSerialized]
        public bool isConnected = false;
        
        private Socket _socket;
        private string _ip;
        private int _port;

        private byte[] _receiveBuffer;
        
        public delegate void DataReceived(CommunicationMessage message);

        public event DataReceived DataReceivedEvent;
        

        public Client(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public void Initialize()
        {
            Debug.Log("Initialize client");

            if (_socket != null)
            {
                ShutDown();
            }
            _socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            _socket.ReceiveBufferSize = AsyncStateObject.BufferSize;
            _socket.SendBufferSize = AsyncStateObject.BufferSize;
            
            _receiveBuffer = new byte[AsyncStateObject.BufferSize];
        }
        
        public void Connect()
        {
            Debug.Log("Trying to connect to server...");
            
            IPAddress ipAddress = IPAddress.Parse(_ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _port);

            try
            {
                _socket.BeginConnect(localEndPoint, ConnectRequestCallback, _socket);
            }
            catch (Exception ex)
            {
                Debug.Log("Cannot connect to server. Reason : " + ex.Message);
            }
        }

        private void ConnectRequestCallback(IAsyncResult ar)
        {
            _socket.EndConnect(ar);
            isConnected = true;

            BeginReceive();
            
            Debug.Log("Successfully connected!");
        }

        void BeginReceive()
        {
            _socket.BeginReceive(
                _receiveBuffer,
                0,
                _receiveBuffer.Length, 
                SocketFlags.None, 
                DataReceivedCallback, 
                _socket);
        }
        
        private void DataReceivedCallback(IAsyncResult ar)
        {
            var receivedMessage = CommunicationUtility.Deserialize(_receiveBuffer);
            Debug.Log(receivedMessage.Message);
            DataReceivedEvent?.Invoke(receivedMessage);

            _socket.EndReceive(ar);
            BeginReceive();
        }

        public void SendData(CommunicationMessage message)
        {
            if (_socket == null) return;
            
            Debug.Log("Trying to send...");

            try
            {
                var byteData = CommunicationUtility.Serialize(message);

                SendData(byteData);
            }
            catch (Exception ex)
            {
                Debug.Log("Send Failure. Reason : " + ex.Message);
            }
            
        }

        public void SendData(byte[] byteData)
        {
            _socket?.BeginSend(
                byteData,
                0,
                byteData.Length,
                SocketFlags.None,
                SendComplete,
                _socket);
        }

        private void SendComplete(IAsyncResult ar)
        {
            _socket.EndSend(ar);
            
            Debug.Log("Send Complete!");
        }
        
        public void ShutDown()
        {
            Debug.Log("Shutdown client...");
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            _socket = null;
            isConnected = false;
            Debug.Log("Shutdown complete!");
        }
    }
}

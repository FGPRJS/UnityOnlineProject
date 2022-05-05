using System;
using System.Net;
using System.Net.Sockets;
using Content.Communication.Protocol;
using UnityEngine;

namespace Content.Communication
{
    public class Client
    {
        public bool isConnected = false;
        
        private Socket _socket;
        private string _ip;
        private int _port;

        private byte[] _receiveBuffer;
        
        public delegate void DataReceived(CommunicationMessage message);
        public event DataReceived DataReceivedEvent;

        public delegate void Connected();
        public event Connected ConnectedEvent;

        public delegate void Disconnected();
        public event Disconnected DisconnectedEvent;

        public delegate void DataSend();
        public event DataSend DataSendEvent;
        

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
            ConnectedEvent?.Invoke();
            
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
            try
            {
                int bytesRead = _socket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    var receivedMessage = CommunicationUtility.Deserialize(_receiveBuffer);
                    Debug.Log(receivedMessage.Message);
                    DataReceivedEvent?.Invoke(receivedMessage);
                }

                BeginReceive();
            }
            catch (NullReferenceException)
            {
                Debug.Log("Socket is null.");
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket is not usable. Reason : " + ex.Message);
                ShutDown();
            }
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
            try
            {
                _socket?.BeginSend(
                    byteData,
                    0,
                    byteData.Length,
                    SocketFlags.None,
                    SendComplete,
                    _socket);
            }
            catch (SocketException)
            {
                Debug.Log("Server Connection Lost.");
                ShutDown();
            }
        }

        private void SendComplete(IAsyncResult ar)
        {
            _socket.EndSend(ar);
            DataSendEvent?.Invoke();
            
            Debug.Log("Send Complete!");
        }
        
        public void ShutDown()
        {
            Debug.Log("Shutdown client...");
            try
            {
                _socket?.Shutdown(SocketShutdown.Both);
                _socket?.Close();
            }
            catch (SocketException)
            {
                Debug.Log("Already ShutDowned");
            }
            _socket = null;
            isConnected = false;
            DisconnectedEvent?.Invoke();
            Debug.Log("Shutdown complete!");
        }
    }
}

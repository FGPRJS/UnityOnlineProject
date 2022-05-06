using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Content.Communication.Protocol;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Content.Communication
{
    /// <summary>
    /// Client Socket Wrapper
    /// </summary>
    public class TCPCommunicator : MonoBehaviour
    {
        public static TCPCommunicator Instance;

        public string ip = "127.0.0.1";
        public int port = 8080;
        private Socket _socket;

        private Heartbeat _heartbeat;
        private byte[] _receiveBuffer;

        public UnityEvent connectedEvent;
        public UnityEvent disconnectedEvent;
        public UnityEvent<CommunicationMessage<Dictionary<string,string>>> dataReceivedEvent;
        public UnityEvent dataSendEvent;

        private void Awake()
        {
            #region Singleton
            if (Instance == null)
            {
                Instance = this;
                UnityEngine.Object.DontDestroyOnLoad(Instance);
            }
            else if (Instance != this)
            {
                Debug.Log("Instance already exists");
                Destroy(this);
            }
            #endregion
        }
        
        #region Socket Functions
        private void Initialize()
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

        public void ConnectToServer()
        {
            Initialize();
            Connect();
        }

        
        private void Connect()
        {
            Debug.Log("Trying to connect to server...");
            
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

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

            connectedEvent?.Invoke();
            
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
                    dataReceivedEvent?.Invoke(receivedMessage);
                    Debug.Log("Received Data Info : " + Encoding.ASCII.GetString(_receiveBuffer));
                    Array.Clear(_receiveBuffer, 0, _receiveBuffer.Length);
                    _heartbeat.ResetHeartbeat();
                    BeginReceive();
                }
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

        public void SendData(CommunicationMessage<Dictionary<string,string>> message)
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
                Debug.Log("Sending Data Info : " + JsonConvert.DeserializeObject(Encoding.ASCII.GetString(byteData)));
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
            dataSendEvent?.Invoke();
            
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
            disconnectedEvent?.Invoke();
            Debug.Log("Shutdown complete!");
        }
        
        #endregion
        
        private void FixedUpdate()
        {
            //Heartbeat
            if(_socket != null)
                _heartbeat.CountHeartbeat(Time.deltaTime);
        }
        
        private void OnEnable()
        {
            _heartbeat = new Heartbeat();
            _heartbeat.HeartbeatTickEvent += HeartbeatTick;
            _heartbeat.HeartbeatTimeOutEvent += HeartbeatTimeout;
            dataReceivedEvent.AddListener(HeartbeatReply);
        }

        void OnDisable()
        {
            ShutDown();

            connectedEvent.RemoveAllListeners();
            disconnectedEvent.RemoveAllListeners();
            dataReceivedEvent.RemoveAllListeners();
            dataSendEvent.RemoveAllListeners();

            _heartbeat.HeartbeatTickEvent -= HeartbeatTick;
            _heartbeat.HeartbeatTimeOutEvent -= HeartbeatTimeout;

            _heartbeat = null;
            _socket = null;
        }
        
        #region Heartbeat

        void HeartbeatTick()
        {
            SendData(Heartbeat.HeartbeatMessageByteData);
        }
        
        void HeartbeatTimeout()
        {
            ShutDown();
        }

        void HeartbeatReply(CommunicationMessage<Dictionary<string,string>> message)
        {
            if (message.header.MessageName == MessageType.HeartBeatRequest.ToString())
            {
                if (message.header.ACK == (int)ACK.ACK) return;

                message.header.ACK = (int)ACK.ACK;
            
                SendData(new CommunicationMessage<Dictionary<string,string>>()
                {
                    header = new Header()
                    {
                        MessageName = MessageType.HeartBeatRequest.ToString()
                    }
                });
            }
        }
        #endregion
    }
}

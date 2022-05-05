using System;
using Content.Communication.Protocol;
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
        private Client _client;
        private Heartbeat _heartbeat;

        public UnityEvent connectedEvent;
        public UnityEvent disconnectedEvent;
        public UnityEvent<CommunicationMessage> dataReceivedEvent;
        public UnityEvent dataSendEvent;
        

        private void Awake()
        {
            //Singleton
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
        }

        public void ConnectToServer()
        {
            _client.Initialize();
            _client.Connect();
        }

        private void FixedUpdate()
        {
            //Heartbeat
            if(_client.isConnected)
                _heartbeat.CountHeartbeat(Time.deltaTime);
        }
        
        private void OnEnable()
        {
            _client = new Client(ip, port);
            _client.DataReceivedEvent += DataReceived;
            _client.ConnectedEvent += Connected;
            _client.DisconnectedEvent += Disconnected;
            _client.DataSendEvent += DataSend;
            
            _heartbeat = new Heartbeat();
            _heartbeat.HeartbeatTickEvent += HeartbeatTick;
            _heartbeat.HeartbeatTimeOutEvent += HeartbeatTimeout;
        }

        #region Delegate Event Wrapping
        void DataReceived(CommunicationMessage message)
        {
            dataReceivedEvent?.Invoke(message);
        }

        void Connected()
        {
            connectedEvent?.Invoke();
        }

        void Disconnected()
        {
            disconnectedEvent?.Invoke();
        }

        void DataSend()
        {
            dataSendEvent.Invoke();
        }
        #endregion
        
        void OnDisable()
        {
            _client.ShutDown();

            _client.DataReceivedEvent -= DataReceived;
            _client.ConnectedEvent -= Connected;
            _client.DisconnectedEvent -= Disconnected;
            _client.DataSendEvent -= DataSend;
            
            connectedEvent.RemoveAllListeners();
            disconnectedEvent.RemoveAllListeners();
            dataReceivedEvent.RemoveAllListeners();
            dataSendEvent.RemoveAllListeners();

            _heartbeat.HeartbeatTickEvent -= HeartbeatTick;
            _heartbeat.HeartbeatTimeOutEvent -= HeartbeatTimeout;

            _heartbeat = null;
            _client = null;
        }
        
        #region Heartbeat

        void HeartbeatTick()
        {
            _client.SendData(Heartbeat.HeartbeatMessageByteData);
        }
        
        void HeartbeatTimeout()
        {
            _client.ShutDown();
        }
        #endregion
    }
}

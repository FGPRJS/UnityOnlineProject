using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityOnlineProjectProtocol.Protocol;
using Object = UnityEngine.Object;

namespace Content.Communication
{
    public class TCPCommunicator : MonoBehaviour
    {
        public static TCPCommunicator Instance;

        public string ip = "127.0.0.1";
        public int port = 8080;
        private Client _client;
        private Heartbeat _heartbeat;

        private void Awake()
        {
            _client = new Client(ip, port);
            _client.DataReceivedEvent += DataReceivedFromServer;
            
            _heartbeat = new Heartbeat();
            _heartbeat.HeartbeatTickEvent += HeartbeatTick;
            _heartbeat.HeartbeatTimeOutEvent += HeartbeatTimeout;
            //Singleton
            if (Instance == null)
            {
                Instance = this;
                Object.DontDestroyOnLoad(Instance);
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

        private void DataReceivedFromServer(CommunicationMessage message)
        {
            _heartbeat.ResetHeartbeat();
            //Temp
            switch (message.MessageType)
            {
                case CommandType.HeartBeatRequest:

                    //Just Reply
                    _client.SendData(Heartbeat.HeartbeatMessageByteData);
                    
                    break;
            }
        }

        void OnDestroy()
        {
            _client.ShutDown();
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Content.Communication.Protocol;
using UnityEngine;
using UnityEngine.Events;

namespace Content.Communication
{
    public abstract class Communicator : MonoBehaviour
    {
        public static Communicator Instance;
        public string dnsAddress;
        public int port;
        
        public UnityEvent connectedEvent;
        public UnityEvent disconnectedEvent;
        public UnityEvent<CommunicationMessage<Dictionary<string,string>>> dataReceivedEvent;
        public UnityEvent dataSendEvent;
        
        void Awake()
        {
            //Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.Log("Instance already exists");
                Destroy(this);
            }
            
            DontDestroyOnLoad(Instance);
        }
        
        #region Socket Functions
        
        public abstract void ConnectToServer();

        public abstract void SendData(CommunicationMessage<Dictionary<string, string>> message);

        public abstract void ShutDown();

        #endregion
        
        protected void OnDisable()
        {
            connectedEvent.RemoveAllListeners();
            disconnectedEvent.RemoveAllListeners();
            dataReceivedEvent.RemoveAllListeners();
            dataSendEvent.RemoveAllListeners();

            ShutDown();
        }
    }
}
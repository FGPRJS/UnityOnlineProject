using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Content.Communication.Protocol;
using Newtonsoft.Json;
using UnityEngine;

namespace Content.Communication
{
    public class JSWebSocketCommunicator : Communicator
    {
        void Initialize()
        {
            JSWebSocket.OpenEvent += (() =>
            {
                connectedEvent.Invoke();
            });
            JSWebSocket.CloseEvent += (() =>
            {
                disconnectedEvent.Invoke();
            });
            JSWebSocket.MessageEvent += ((message) =>
            {
                var deserialized = CommunicationUtility.Deserialize(message);
                dataReceivedEvent?.Invoke(deserialized);
            });
            JSWebSocket.ErrorEvent += (() =>
            {
                Debug.Log("ERROR!");
            });
            
            DontDestroyOnLoad(this);
        }

        private void MessageEventAction(string e)
        {
            var receivedMessage = CommunicationUtility.Deserialize(e);
            
            dataReceivedEvent?.Invoke(receivedMessage);
        }

        private void ErrorEventAction()
        {
            
        }

        private void OpenEventAction()
        {
            connectedEvent?.Invoke();
        }

        void CloseEventAction()
        {
            disconnectedEvent?.Invoke();
        }

        public override void ConnectToServer()
        {
            Initialize();
            Connect();
        }

        void Connect()
        {
            var uriBuilder = new UriBuilder(
                "ws",
                dnsAddress,
                port,
                "websocket");
            JSWebSocket.ConnectToServer(uriBuilder.ToString());
            DontDestroyOnLoad(this);
        }

        public override void SendData(CommunicationMessage<Dictionary<string, string>> message)
        {
            var json = JsonConvert.SerializeObject(message);
            JSWebSocket.Send(json);
        }

        public override void ShutDown()
        {
            JSWebSocket.Close(0, "Normal Shutdown");
        }
    }
}
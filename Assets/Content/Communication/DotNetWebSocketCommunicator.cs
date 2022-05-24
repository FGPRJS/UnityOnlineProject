using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Content.Communication.Protocol;
using Newtonsoft.Json;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.Events;

namespace Content.Communication
{
    public class DotNetWebSocketCommunicator : Communicator
    {
        private ClientWebSocket _socket;

        #region Socket Functions
        public void Initialize()
        {
            Debug.Log("Initialize client");

            if (_socket != null)
            {
                ShutDown();
            }

            _socket = new ClientWebSocket();
            DontDestroyOnLoad(this);
        }

        public override void ConnectToServer()
        {
            Initialize();
            Connect();
        }

        protected async void Connect()
        {
            Debug.Log("Trying to connect to server...");

            var uriBuilder = new UriBuilder("ws", dnsAddress, port, "websocket");
            
            try
            {
                await _socket.ConnectAsync(uriBuilder.Uri, CancellationToken.None);
                connectedEvent?.Invoke();
                ReceiveData();
            }
            catch (Exception ex)
            {
                Debug.Log("Cannot connect to server. Reason : " + ex.Message);
            }
        }


        protected async void ReceiveData()
        {
            while (_socket?.State == WebSocketState.Open)
            {
                var receivedData = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = null;

                do
                {
                    result = await _socket.ReceiveAsync(receivedData, CancellationToken.None);
                } while (!result.EndOfMessage);
                    
                var deserializedData = CommunicationUtility.Deserialize(receivedData.Array);
                
                dataReceivedEvent?.Invoke(deserializedData);
                Debug.Log("Received. \n" + JsonConvert.SerializeObject(deserializedData));
            }
        }
        
        public override void SendData(CommunicationMessage<Dictionary<string,string>> message)
        {
            if (_socket == null) return;
            
            Debug.Log("Trying to send...");

            try
            {
                message.header.SendTime = DateTime.Now;

                var byteData = CommunicationUtility.Serialize(message);

                SendData(byteData);
            }
            catch (Exception ex)
            {
                Debug.Log("Send Failure. Reason : " + ex.Message);
            }
        }

        public async Task SendData(byte[] byteData)
        {
            try
            {
                await _socket.SendAsync(
                    byteData, 
                    WebSocketMessageType.Text, 
                    true, 
                    CancellationToken.None);
                dataSendEvent?.Invoke();
                
                Debug.Log("Sending Data Info : " + JsonConvert.DeserializeObject(Encoding.ASCII.GetString(byteData)));
            }
            catch (Exception)
            {
                Debug.Log("Server Connection Lost.");
                ShutDown();
            }
        }

        public override async void ShutDown()
        {
            Debug.Log("Shutdown client...");
            try
            {
                await _socket.CloseOutputAsync(
                    WebSocketCloseStatus.NormalClosure,
                    String.Empty,
                    CancellationToken.None);
            }
            catch (NullReferenceException)
            {
                Debug.Log("Socket is Null");
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
    }
}
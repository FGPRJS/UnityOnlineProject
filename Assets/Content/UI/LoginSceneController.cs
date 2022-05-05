using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Content.Communication;
using Content.Communication.Protocol;
using Content.Global;
using Script.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.UI
{
    /// <summary>
    /// Very Important GameObject cause this object controls [Scene's ALL GameObjects] that have to communicate with Server.
    /// </summary>
    public class LoginSceneController : MonoBehaviour
    {
        public LaunchButton launchButton;
        public TCPCommunicator communicator;
        public LocalMessageWindow messageWindow;
        public NameInput nameInput;

        private bool _isConnected = false;
        
        private float _currentProcess = 0;

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            GameManager.Instance.sceneLoadStartEvent.AddListener(SceneLoadStart);
            GameManager.Instance.sceneLoadCompleteEvent.AddListener(SceneLoadComplete);
            launchButton.clickedEvent.AddListener(Launch);
            communicator.dataReceivedEvent.AddListener(DataReceived);
            communicator.connectedEvent.AddListener(ShowConnected);
            communicator.disconnectedEvent.AddListener(ShowDisconnected);
        }

        void Start()
        {
            messageWindow.AddMessage("SYSTEM", "Connecting to Server...");
            communicator.ConnectToServer();
        }

        void ShowConnected()
        {
            _isConnected = true;
            messageWindow.AddMessage("SYSTEM", "Connected to Server.");
        }

        void ShowDisconnected()
        {
            _isConnected = false;
            messageWindow.AddMessage("SYSTEM", "Disconnected to Server.");
        }

        void Launch()
        {
            if (!_isConnected)
            {
                messageWindow.AddMessage("SYSTEM", "NOT CONNECTED. Cannot Launch");
                communicator.ConnectToServer();
                return;
            }

            string inputName = nameInput.inputField.text;

            //Check Message
                
            //Name contains invalid char
            if(!Regex.IsMatch(inputName, @"^[0-9a-zA-Z ]+$"))
            {
                messageWindow.AddMessage("SYSTEM", "Name contains invalid character");
                return;
            }
            //Name is too short
            if (inputName.Length < 2)
            {
                messageWindow.AddMessage("SYSTEM", "Name is too short");
                return;
            }
            //Name is too long
            if (inputName.Length > 36)
            {
                messageWindow.AddMessage("SYSTEM", "Name is too long");
                return;
            }
            
            var newMessage = new CommunicationMessage<Dictionary<string, string>>()
            {
                header = new Header()
                {
                    MessageName = MessageType.LoginRequest.ToString(),
                },
                body = new Body<Dictionary<string, string>>()
                {
                    Any = new Dictionary<string, string> { { "UserName", inputName } }
                }
            };

            communicator.SendData(newMessage);
        }

        void DataReceived(CommunicationMessage<Dictionary<string,string>> message)
        {
            if (message.header.MessageName != MessageType.LoginRequest.ToString()) return;
            if (message.header.ACK == 1)
            {
                GameManager.Instance.ChangeScene(GameManager.Instance.mainSceneName);
            }
            else
            {
                messageWindow.AddMessage("SYSTEM", "Cannot Login to server. Reason : " + message.header.Reason);
            }
        }
        
        void SceneLoadStart()
        {
            messageWindow.AddMessage("SYSTEM", "Loading...");
        }
        
        void SceneLoadComplete()
        {
            messageWindow.AddMessage("SYSTEM", "Load Complete.");
        }

        void Update()
        {
            if (Math.Abs(_currentProcess - GameManager.Instance.SceneLoadProgress) > 0.01)
            {
                messageWindow.AddMessage("SYSTEM", "Process : " + (_currentProcess * 100));
                _currentProcess = GameManager.Instance.SceneLoadProgress;
            }
        }

        private void OnDisable()
        {
            GameManager.Instance.sceneLoadStartEvent.RemoveListener(SceneLoadStart);
            GameManager.Instance.sceneLoadCompleteEvent.RemoveListener(SceneLoadComplete);
            communicator.dataReceivedEvent.RemoveListener(DataReceived);
            launchButton.clickedEvent.RemoveListener(Launch);
            communicator.connectedEvent.RemoveListener(ShowConnected);
            communicator.disconnectedEvent.RemoveListener(ShowDisconnected);
        }
    }
}
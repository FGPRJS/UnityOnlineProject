using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Content.Communication;
using Content.Communication.Protocol;
using Content.Global;
using UnityEngine.InputSystem;

namespace Content.UI
{
    /// <summary>
    /// Very Important GameObject cause this object controls [Scene's ALL GameObjects] that have to communicate with Server.
    /// </summary>
    public class LoginSceneController : ControllerBase
    {
        public LaunchButton launchButton;
        private Communicator _communicator;
        public LocalMessageWindow messageWindow;
        public NameInput nameInput;

        private bool _isConnected = false;
        
        private float _currentProcess = 0;

        private void OnEnable()
        {
            _communicator = Communicator.Instance;

            GameManager.Instance.sceneLoadStartEvent.AddListener(SceneLoadStart);
            GameManager.Instance.sceneLoadCompleteEvent.AddListener(SceneLoadComplete);
            launchButton.clickedEvent.AddListener(Launch);
            _communicator.dataReceivedEvent.AddListener(DataReceived);
            _communicator.connectedEvent.AddListener(ShowConnected);
            _communicator.disconnectedEvent.AddListener(ShowDisconnected);
        }

        void Start()
        {
            AddAction(() =>
            {
                messageWindow.AddMessage("SYSTEM", "Connecting to Server...");
                _communicator.ConnectToServer();
            });
        }

        void ShowConnected()
        {
            AddAction(() =>
            {
                _isConnected = true;
                messageWindow.AddMessage("SYSTEM", "Connected to Server.");
            });
        }

        void ShowDisconnected()
        {
            AddAction(() =>
            {
                _isConnected = false;
                messageWindow.AddMessage("SYSTEM", "Disconnected to Server.");
            });
        }

        void Launch()
        {
            if (!_isConnected)
            {
                AddAction(() =>
                {
                    messageWindow.AddMessage("SYSTEM", "NOT CONNECTED. Cannot Launch");
                    _communicator.ConnectToServer();
                });
                
                return;
            }

            string inputName = nameInput.inputField.text;

            //Check Message
                
            //Name contains invalid char
            if(!Regex.IsMatch(inputName, @"^[0-9a-zA-Z ]+$"))
            {
                AddAction(() =>
                {
                    messageWindow.AddMessage("SYSTEM", "Name contains invalid character");
                });
                return;
            }
            //Name is too short
            if (inputName.Length < 2)
            {
                AddAction(() =>
                {
                    messageWindow.AddMessage("SYSTEM", "Name is too short");
                });
                return;
            }
            //Name is too long
            if (inputName.Length > 36)
            {
                AddAction(() =>
                {
                    messageWindow.AddMessage("SYSTEM", "Name is too long");
                });
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
                    Any = new Dictionary<string, string>
                    {
                         ["UserName"] = inputName
                    }
                }
            };

            _communicator.SendData(newMessage);
        }

        void DataReceived(CommunicationMessage<Dictionary<string,string>> message)
        {
            if (message.header.MessageName != MessageType.LoginRequest.ToString()) return;
            if (message.header.ACK == 1)
            {
                AddAction(() =>
                {
                    GameManager.Instance.ChangeScene(GameManager.Instance.mainSceneName);
                });
            }
            else
            {
                AddAction(() =>
                {
                    messageWindow.AddMessage("SYSTEM", "Cannot Login to server. Reason : " + message.header.Reason);
                });
            }
        }
        
        void SceneLoadStart()
        {
            AddAction(() =>
            {
                messageWindow.AddMessage("SYSTEM", "Loading...");            
            });
        }
        
        void SceneLoadComplete()
        {
            AddAction(() =>
            {
                messageWindow.AddMessage("SYSTEM", "Load Complete.");
            });
        }

        protected override void Update()
        {
            base.Update();
            
            if (Math.Abs(_currentProcess - GameManager.Instance.SceneLoadProgress) > 0.01)
            {
                AddAction(() =>
                {
                    messageWindow.AddMessage("SYSTEM", "Process : " + (_currentProcess * 100));
                });
                
                _currentProcess = GameManager.Instance.SceneLoadProgress;
            }
        }

        private void OnDisable()
        {
            GameManager.Instance.sceneLoadStartEvent.RemoveListener(SceneLoadStart);
            GameManager.Instance.sceneLoadCompleteEvent.RemoveListener(SceneLoadComplete);
            _communicator.dataReceivedEvent.RemoveListener(DataReceived);
            launchButton.clickedEvent.RemoveListener(Launch);
            _communicator.connectedEvent.RemoveListener(ShowConnected);
            _communicator.disconnectedEvent.RemoveListener(ShowDisconnected);
        }
    }
}
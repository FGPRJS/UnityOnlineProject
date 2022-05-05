using System;
using System.Collections.Generic;
using Content.Communication;
using Content.Global;
using Script.UI;
using UnityEngine;

namespace Content.UI
{
    public class LoginSceneUIController : MonoBehaviour
    {
        public LaunchButton launchButton;
        public TCPCommunicator communicator;
        public LocalMessageWindow messageWindow;

        private float _currentProcess = 0;

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            GameManager.Instance.sceneLoadStartEvent.AddListener(SceneLoadStart);
            GameManager.Instance.sceneLoadCompleteEvent.AddListener(SceneLoadComplete);
            launchButton.clickedEvent.AddListener(Launch);
            communicator.connectedEvent.AddListener(ShowConnected);
            communicator.disconnectedEvent.AddListener(ShowDisconnected);
        }


        void ShowConnected()
        {
            messageWindow.AddMessage("SYSTEM", "Connected to Server.");
            GameManager.Instance.ChangeScene(GameManager.Instance.mainSceneName);
        }

        void ShowDisconnected()
        {
            messageWindow.AddMessage("SYSTEM", "Disconnected to Server.");
        }

        void Launch()
        {
            messageWindow.AddMessage("SYSTEM", "Connecting to Server...");
            communicator.ConnectToServer();
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
                
                _currentProcess = GameManager.Instance.SceneLoadProgress;
            }
        }

        private void OnDisable()
        {
            GameManager.Instance.sceneLoadStartEvent.RemoveListener(SceneLoadStart);
            GameManager.Instance.sceneLoadCompleteEvent.RemoveListener(SceneLoadComplete);
            launchButton.clickedEvent.RemoveListener(Launch);
            communicator.connectedEvent.RemoveListener(ShowConnected);
            communicator.disconnectedEvent.RemoveListener(ShowDisconnected);
        }
    }
}
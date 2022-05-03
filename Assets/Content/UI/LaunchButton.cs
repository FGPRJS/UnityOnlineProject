using System;
using Content.Communication;
using UnityEngine;
using UnityEngine.UI;

namespace Content.UI
{
    public class LaunchButton : MonoBehaviour
    {
        public Button button;

        public void OnEnable()
        {
            button.onClick.AddListener(ButtonClicked);
        }

        public void OnDisable()
        {
            button.onClick.RemoveListener(ButtonClicked);
        }

        private void ButtonClicked()
        {
            TCPCommunicator.instance.ConnectToServer();
        }
    }
}

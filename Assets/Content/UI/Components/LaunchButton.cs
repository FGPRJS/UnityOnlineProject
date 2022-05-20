using System;
using Content.Communication;
using Content.Global;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Content.UI
{
    public class LaunchButton : MonoBehaviour
    {
        public UnityEvent clickedEvent;
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
            clickedEvent?.Invoke();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Content.UI
{
    public class LocalMessageWindow : MonoBehaviour
    {
        public LocalMessageWindowItem content;
        public GameObject contentPanel;

        public void AddMessage(string sender, string message)
        {
            var item = Instantiate(content, contentPanel.transform, false);
            item.MessageText.text = message;
            item.SenderNameText.text = sender;
        }
    }
}

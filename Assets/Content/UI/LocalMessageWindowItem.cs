using System;
using TMPro;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Script.UI
{
    public class LocalMessageWindowItem : MonoBehaviour
    {
        public TextMeshProUGUI SenderNameText;
        public TextMeshProUGUI MessageText;

        public void SetContent(string sender, string message)
        {
            SenderNameText.text = sender;
            MessageText.text = message;
        }
    }
}

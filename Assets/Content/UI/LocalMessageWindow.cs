using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content.UI
{
    public class LocalMessageWindow : MonoBehaviour
    {
        public LocalMessageWindowItem content;
        public GameObject contentPanel;

        private Queue<LocalMessageCommand> _messageQueue;

        private void Awake()
        {
            _messageQueue = new Queue<LocalMessageCommand>();
        }

        public void AddMessage(string sender, string message)
        {
            LocalMessageCommand cmd = new LocalMessageCommand
            {
                message = message,
                sender = sender
            };
            
            _messageQueue.Enqueue(cmd);
        }

        private void Update()
        {
            while (_messageQueue.Count > 0)
            {
                var cmd = _messageQueue.Dequeue();
                
                var item = Instantiate(content, contentPanel.transform, false);
                item.MessageText.text = cmd.message;
                item.SenderNameText.text = cmd.sender;
            }
        }
    }

    class LocalMessageCommand
    {
        internal string sender;
        internal string message;
    }
}

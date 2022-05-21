using Content.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Content.UI.Components
{
    public class ChatInput : MonoBehaviour
    {
        public TMP_InputField chatInput;
        public Button sendButton;

        private PlayerInputManager _inputManager;
        
        private bool _bfChatInputFocused = false;
        public UnityEvent<bool> chatInputChangedEvent;
        public UnityEvent<string> chatSendMessageEvent;

        private void Awake()
        {
            _inputManager = PlayerInputManager.Instance;
        }
        
        private void Start()
        {
            sendButton.onClick.AddListener(SendMessage);
        }

        private void Update()
        {
            #region Read Enter

            var pressed = _inputManager.enterAction.ReadValue<float>();
            if (pressed > 0)
            {
                SendMessage();
            }
            
            #endregion
            
            #region FocusCheck
            if (chatInput.isFocused)
            {
                if (!_bfChatInputFocused)
                {
                    chatInputChangedEvent?.Invoke(true);
                }
            }
            //is not Focused
            else
            {
                if (_bfChatInputFocused)
                {
                    chatInputChangedEvent?.Invoke(false);
                }
            }
            
            _bfChatInputFocused = chatInput.isFocused;
            #endregion
        }
        
        public void SendMessage()
        {
            if (chatInput.text == string.Empty) return;
            chatSendMessageEvent.Invoke(chatInput.text);
            chatInput.text = string.Empty;
        }
    }
}
using Content.Global;
using UnityEngine;

namespace Script.UI
{
    public class LocalMessageWindow : MonoBehaviour
    {
        public LocalMessageWindowItem content;
        
        private void OnEnable()
        {
            LocalMessageManager.Instance.MessageEvent.AddListener(MessageReceived);
        }

        private void MessageReceived(MessageEventArg arg0)
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnDestroy()
        {
            
        }
    }
}

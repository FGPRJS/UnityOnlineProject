using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Content.UI.SceneController
{
    public class ControllerBase : MonoBehaviour
    {
        private Queue<ControllerMessage> MessageQueue = new Queue<ControllerMessage>();

        protected virtual void Update()
        {
            while (MessageQueue.Count > 0)
            {
                var popped = MessageQueue.Dequeue();
                
                popped.DoAction();
            }
        }

        public void AddAction(UnityAction action)
        {
            MessageQueue.Enqueue(new ControllerMessage()
            {
                Action = action
            });
        }
    }

    public class ControllerMessage
    {
        public UnityAction Action;

        public virtual void DoAction()
        {
            Action.Invoke();
        }
    }
}
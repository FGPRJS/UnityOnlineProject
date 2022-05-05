using System.Collections.Generic;
using Content.Communication.Protocol;
using UnityEngine.Events;

namespace Content.Communication
{
    public class Heartbeat
    {
        private const float HeartbeatInterval = 30;
        private const float MaxHeartbeatTimeoutCount = 6;

        private float _currentHeartbeatTime = 0;
        private int _currentHeartbeatTimeoutCount = 0;

        internal static CommunicationMessage<Dictionary<string,string>> HeartbeatMessage;
        internal static byte[] HeartbeatMessageByteData;

        //Event is Sync cause using Task in unity cause non-predictable issue.
        internal delegate void HeartbeatTimeOut();
        internal event HeartbeatTimeOut HeartbeatTimeOutEvent ;
        internal delegate void HeartbeatTick();
        internal event HeartbeatTick HeartbeatTickEvent;

        internal Heartbeat()
        {
            if ((HeartbeatMessage != null) && (HeartbeatMessageByteData != null)) return;

            HeartbeatMessage = new CommunicationMessage<Dictionary<string,string>>()
            {
                header = new Header()
                {
                    MessageName = MessageType.HeartBeatRequest.ToString()
                }
            };

            HeartbeatMessageByteData = CommunicationUtility.Serialize(HeartbeatMessage);
        }

        public void ResetHeartbeat()
        {
            _currentHeartbeatTime = 0;
            _currentHeartbeatTimeoutCount = 0;
        }

        public void CountHeartbeat(float interval)
        {
            _currentHeartbeatTime += interval;
            if (_currentHeartbeatTime >= HeartbeatInterval)
            {
                HeartbeatTickEvent?.Invoke();
                _currentHeartbeatTimeoutCount++;
            }

            if (_currentHeartbeatTimeoutCount > MaxHeartbeatTimeoutCount)
            {
                HeartbeatTimeOutEvent?.Invoke();
            }
        }
    }
}
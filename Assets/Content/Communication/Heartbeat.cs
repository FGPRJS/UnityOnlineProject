using UnityEngine.Events;
using UnityOnlineProjectProtocol;
using UnityOnlineProjectProtocol.Protocol;

namespace Content.Communication
{
    public class Heartbeat
    {
        private const float HeartbeatInterval = 30;
        private const float MaxHeartbeatTimeoutCount = 6;

        private float _currentHeartbeatTime = 0;
        private int _currentHeartbeatTimeoutCount = 0;

        internal static CommunicationMessage HeartbeatMessage;
        internal static byte[] HeartbeatMessageByteData;

        //Event is Sync cause using Task in unity cause non-predictable issue.
        internal delegate void HeartbeatTimeOut();
        internal event HeartbeatTimeOut HeartbeatTimeOutEvent ;
        internal delegate void HeartbeatTick();
        internal event HeartbeatTick HeartbeatTickEvent;

        internal Heartbeat()
        {
            if ((HeartbeatMessage != null) && (HeartbeatMessageByteData != null)) return;

            HeartbeatMessage = new CommunicationMessage()
            {
                MessageType = CommandType.HeartBeatRequest
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
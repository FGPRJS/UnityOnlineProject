using System;

namespace Content.Communication.Protocol
{
    public enum CommandType
    {
        ConnectionRequest,
        HeartBeatRequest
    }

    [Serializable]
    public class CommunicationMessage
    {
        public CommandType MessageType;
        public string Message;
    }
}
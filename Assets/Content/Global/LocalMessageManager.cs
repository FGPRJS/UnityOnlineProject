using UnityEngine.Events;

namespace Content.Global
{
    public class LocalMessageManager
    {
        private static LocalMessageManager _instance;

        public static LocalMessageManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalMessageManager
                    {
                        MessageEvent = new UnityEvent<MessageEventArg>()
                    };
                }

                return _instance;
            }
        }

        public UnityEvent<MessageEventArg> MessageEvent;
    }

    public class MessageEventArg
    {
        public string Sender;
        public string Message;
    }
}

using System.Text;
using UnityEngine;

namespace Content.Communication.Protocol
{
    internal class CommunicationUtility
    {
        public static byte[] Serialize(CommunicationMessage message)
        {
            var data = JsonUtility.ToJson(message);

            return Encoding.ASCII.GetBytes(data);
        }

        public static CommunicationMessage Deserialize(string json)
        {
            return JsonUtility.FromJson<CommunicationMessage>(json);
        }

        public static CommunicationMessage Deserialize(byte[] jsonByte)
        {
            var data = Encoding.ASCII.GetString(jsonByte);

            return JsonUtility.FromJson<CommunicationMessage>(data);
        }
    }
}
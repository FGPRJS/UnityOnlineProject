using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Content.Communication.Protocol
{
    internal class CommunicationUtility
    {
        public static byte[] Serialize(CommunicationMessage<Dictionary<string,string>> message)
        {
            var data = JsonConvert.SerializeObject(message);

            return Encoding.UTF8.GetBytes(data);
        }

        public static CommunicationMessage<Dictionary<string,string>> Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<CommunicationMessage<Dictionary<string,string>>>(json);
        }

        public static CommunicationMessage<Dictionary<string,string>> Deserialize(byte[] jsonByte)
        {
            var data = Encoding.UTF8.GetString(jsonByte);

            return JsonConvert.DeserializeObject<CommunicationMessage<Dictionary<string,string>>>(data);
        }
    }
}
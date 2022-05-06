using System;
using System.Collections.Generic;
using Content.Communication;
using Content.Communication.Protocol;
using UnityEngine;

namespace Content.UI
{
    public class MainSceneController : MonoBehaviour
    {
        public LocalMessageWindow messageWindow;
        public Player.Player player;
        

        void DataReceivedEventActivated(CommunicationMessage<Dictionary<string,string>> message)
        {
            MessageType messageName = (MessageType)Enum.Parse(typeof(MessageType), message.header.MessageName);
            
            switch (messageName)
            {
                case MessageType.GameObjectSpawnRequest:

                    if (message.header.ACK != (int)ACK.ACK) return;

                    var readedVectorPos = message.body.Any["Position"].Split(',');
                    Vector3 position = new Vector3(
                        float.Parse(readedVectorPos[0]),
                        float.Parse(readedVectorPos[1]),
                        float.Parse(readedVectorPos[2]));
                    
                    var readedQuaternion = message.body.Any["Quaternion"].Split(',');
                    Quaternion quaternion = new Quaternion(
                        float.Parse(readedQuaternion[0]),
                        float.Parse(readedQuaternion[1]),
                        float.Parse(readedQuaternion[2]),
                        float.Parse(readedQuaternion[3]));

                    Instantiate(player, position, quaternion);

                    break;
            }
        }
        
        private void OnEnable()
        {
            //Subscribe Event
            TCPCommunicator.Instance.dataReceivedEvent.AddListener(DataReceivedEventActivated);
        }

        private void Start()
        {
            //Initial 
            TCPCommunicator.Instance.SendData(new CommunicationMessage<Dictionary<string,string>>()
            {
                header = new Header()
                {
                    MessageName = MessageType.GameObjectSpawnRequest.ToString()
                }
            });
        }
    }
}
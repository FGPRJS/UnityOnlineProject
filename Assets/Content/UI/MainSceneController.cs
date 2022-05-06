using System;
using System.Collections.Generic;
using Content.Communication;
using Content.Communication.Protocol;
using Content.Pawn;
using UnityEngine;
using Random = System.Random;

namespace Content.UI
{
    public class MainSceneController : ControllerBase
    {
        public LocalMessageWindow messageWindow;
        public Player.Player player;
        public List<Tank> tankInstances;

        void DataReceivedEventActivated(CommunicationMessage<Dictionary<string,string>> message)
        {
            MessageType messageName = (MessageType)Enum.Parse(typeof(MessageType), message.header.MessageName);
            
            switch (messageName)
            {
                case MessageType.TankSpawnRequest:

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

                    #region TankType

                    var tankType = int.Parse(message.body.Any["Type"]);

                    AddAction(() =>
                    {
                        var tank = Instantiate(tankInstances[(int)tankType], position, quaternion);
                        player.pawn = tank;
                    });
                    //Possess

                    #endregion
                    break;
            }
        }
        
        private void OnEnable()
        {
            //Subscribe Event
            TCPCommunicator.Instance.dataReceivedEvent.AddListener(DataReceivedEventActivated);
            
            //Initial 
            TCPCommunicator.Instance.SendData(new CommunicationMessage<Dictionary<string,string>>()
            {
                header = new Header()
                {
                    MessageName = MessageType.TankSpawnRequest.ToString()
                }
            });
        }

        private void OnDisable()
        {
            //Unsubscribe Event
            TCPCommunicator.Instance.dataReceivedEvent.RemoveListener(DataReceivedEventActivated);
        }
    }
}
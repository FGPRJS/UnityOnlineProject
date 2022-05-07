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

                    Vector3 position = NumericParser.ParseVector(message.body.Any["Position"]);
                    Quaternion quaternion = NumericParser.ParseQuaternion(message.body.Any["Quaternion"]);

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

        private void FixedUpdate()
        {
            #region Send Current Position
            if (!player.pawn) return;
            var pawnTransform = player.pawn.transform;
            
            var message = new CommunicationMessage<Dictionary<string, string>>()
            {
                header = new Header()
                {
                    MessageName = MessageType.TankPositionReport.ToString()
                },
                body = new Body<Dictionary<string, string>>()
                {
                    Any = new Dictionary<string, string>()
                    {
                        ["Position"] = pawnTransform.position.ToString(),
                        ["Quaternion"] = pawnTransform.rotation.ToString(),
                        ["TowerQuaternion"] = player.pawn.tower.transform.rotation.ToString(),
                        ["CannonQuaternion"] = player.pawn.cannon.transform.rotation.ToString(),
                    }
                }
            };

            TCPCommunicator.Instance.SendData(message);
            #endregion
            
            
        }
    }
}
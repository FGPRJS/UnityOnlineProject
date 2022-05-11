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
        public Dictionary<long,Pawn.Pawn> instances;

        void Start()
        {
            instances = new Dictionary<long,Pawn.Pawn>();
        }
        
        void DataReceivedEventActivated(CommunicationMessage<Dictionary<string,string>> message)
        {
            MessageType messageName = (MessageType)Enum.Parse(typeof(MessageType), message.header.MessageName);
            
            switch (messageName)
            {
                case MessageType.PawnSpawnRequest:

                    if (message.header.ACK != (int)ACK.ACK) return;

                    Vector3 position = NumericParser.ParseVector(message.body.Any["Position"]);
                    Quaternion quaternion = NumericParser.ParseQuaternion(message.body.Any["Quaternion"]);

                    #region GameObject Type

                    var objectType = (GameObjectType)Enum.Parse(typeof(GameObjectType),
                        message.body.Any["ObjectType"]);

                    switch (objectType)
                    {
                        case GameObjectType.Tank:

                            var subObjectType = (TankType)Enum.Parse(typeof(TankType),
                                message.body.Any["ObjectSubType"]);
                            
                            AddAction(() =>
                            {
                                var id = long.Parse(message.body.Any["ID"]);
                                
                                var tank = Instantiate(tankInstances[(int)subObjectType], position, quaternion);

                                tank.id = id;
                                instances.Add(id, tank);
                                
                                player.pawn = tank;
                            });

                            break;
                    }
                    
                    #endregion
                    break;
                
                case MessageType.PawnPositionReport:

                    long id = long.Parse(message.body.Any["ID"]);
                    
                    Vector3 objectPosition = NumericParser.ParseVector(message.body.Any["Position"]);
                    Quaternion objectRotation = NumericParser.ParseQuaternion(message.body.Any["Quaternion"]);

                    AddAction(() =>
                    {
                        var target = instances[id];
                        
                        var tr = target.transform;
                        tr.position = objectPosition;
                        tr.rotation = objectRotation;
                    });

                    break;
                
                
            }
        }
        
        private void OnEnable()
        {
            //Subscribe Event
            TCPCommunicator.Instance.dataReceivedEvent.AddListener(DataReceivedEventActivated);
            
            //Initial Request Character
            TCPCommunicator.Instance.SendData(new CommunicationMessage<Dictionary<string,string>>()
            {
                header = new Header()
                {
                    MessageName = MessageType.PawnSpawnRequest.ToString()
                },
                body = new Body<Dictionary<string, string>>()
                {
                    Any = new Dictionary<string,string>
                    {
                        ["ObjectType"] = "Tank"
                    }
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
                    MessageName = MessageType.PawnPositionReport.ToString()
                },
                body = new Body<Dictionary<string, string>>()
                {
                    Any = new Dictionary<string, string>()
                    {
                        ["ID"] = player.pawn.id.ToString(),
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
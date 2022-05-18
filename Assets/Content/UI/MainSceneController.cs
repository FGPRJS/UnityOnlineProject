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
        private Communicator _communicator;
        public Player.Player player;
        public List<Tank> tankInstances;
        public List<GameObject> gameObjects;

        void Awake()
        {
            gameObjects = new List<GameObject>();
            
            _communicator = Communicator.Instance;
        }

        private void OnEnable()
        {
            //Subscribe Event
            _communicator.dataReceivedEvent.AddListener(DataReceivedEventActivated);
            
            //Initial Request Character
            _communicator.SendData(new CommunicationMessage<Dictionary<string,string>>()
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
            _communicator.dataReceivedEvent.RemoveListener(DataReceivedEventActivated);
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

            _communicator.SendData(message);
            #endregion
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
                                var tank = Instantiate(tankInstances[(int)subObjectType], position, quaternion);
                                tank.id = long.Parse(message.body.Any["ID"]);
                                player.pawn = tank;
                            });

                            break;
                    }
                    
                    #endregion
                    break;
                
                case MessageType.PawnSpawnReport:


                    break;
                
                case MessageType.PawnPositionReport:

                    long id = long.Parse(message.body.Any["ID"]);
                    
                    Vector3 objectPosition = NumericParser.ParseVector(message.body.Any["Position"]);
                    Quaternion objectRotation = NumericParser.ParseQuaternion(message.body.Any["Quaternion"]);

                    AddAction(() =>
                    {
                        var tr = player.pawn.transform;
                        tr.position = objectPosition;
                        tr.rotation = objectRotation;
                    });
                    
                    break;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Content.Communication;
using Content.Communication.Protocol;
using Content.Pawn;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

namespace Content.UI
{
    public class MainSceneController : ControllerBase
    {
        public LocalMessageWindow messageWindow;
        private Communicator _communicator;
        public Player.Player player;
        public List<Tank> tankInstances;
        public Dictionary<long, Pawn.Pawn> Pawns;

        void Awake()
        {
            Pawns = new Dictionary<long, Pawn.Pawn>();
            
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
                    MessageName = MessageType.GameObjectSpawnRequest.ToString()
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
            var pawn = player.pawn;

            var message = new CommunicationMessage<Dictionary<string, string>>()
            {
                header = new Header()
                {
                    MessageName = MessageType.TankMovingReport.ToString()
                },
                body = new Body<Dictionary<string, string>>()
                {
                    Any = new Dictionary<string, string>()
                    {
                        ["ID"] = pawn.id.ToString(),
                        ["MoveDirection"] = pawn.moveVector.ToString(),
                        ["MoveDelta"] = pawn.moveDelta.ToString(),
                        ["RotationVector"] = pawn.rotationVector.ToString(),
                        ["RotationDelta"] = pawn.rotationDelta.ToString(),
                        ["TowerRotationVector"] = pawn.towerRotateVector.ToString(),
                        ["TowerRotationDelta"] = pawn.towerRotationDelta.ToString(),
                        ["CannonRotationVector"] = pawn.cannonRotateVector.ToString(),
                        ["CannonRotationDelta"] = pawn.cannonRotationDelta.ToString(),
                    }
                }
            };

            _communicator.SendData(message);
            
            message = new CommunicationMessage<Dictionary<string, string>>()
            {
                header = new Header()
                {
                    MessageName = MessageType.TankPositionReport.ToString()
                },
                body = new Body<Dictionary<string, string>>()
                {
                    Any = new Dictionary<string, string>()
                    {
                        ["ID"] = pawn.id.ToString(),
                        ["Position"] = pawn.transform.position.ToString(),
                        ["Quaternion"] = pawn.transform.rotation.ToString(),
                        ["TowerQuaternion"] = pawn.tower.transform.rotation.ToString(),
                        ["CannonQuaternion"] = pawn.cannon.transform.rotation.ToString()
                    }
                }
            };

            _communicator.SendData(message);
            #endregion
        }
        
         void DataReceivedEventActivated(CommunicationMessage<Dictionary<string,string>> message)
        {
            MessageType messageName = (MessageType)Enum.Parse(typeof(MessageType), message.header.MessageName);

            long id;

            switch (messageName)
            {
                case MessageType.GameObjectSpawnRequest:

                    id = long.Parse(message.body.Any["ID"]);

                    if (Pawns.ContainsKey(id)) return;
                    
                    AddAction(CreatePawnAction(message));
                    AddAction(PossessPlayertoPawnAction(id, player));
                    
                    break;
                
                case MessageType.GameObjectSpawnReport:

                    id = long.Parse(message.body.Any["ID"]);
                    
                    if (Pawns.ContainsKey(id)) return;
                    
                    AddAction(CreatePawnAction(message));

                    break;

                case MessageType.TankPositionReport:

                    id = long.Parse(message.body.Any["ID"]);

                    if (!Pawns.ContainsKey(id)) return;
                    
                    AddAction(CreateChangeTankPositionAction(message));

                    break;
                
                case MessageType.TankMovingReport:

                    id = long.Parse(message.body.Any["ID"]);

                    if (!Pawns.ContainsKey(id)) return;
                    
                    AddAction(CreateChangeTankMovingAction(message));

                    break;
                
                case MessageType.GameObjectDestroyReport:

                    id = long.Parse(message.body.Any["ID"]);

                    AddAction(() =>
                    {
                        Destroy(this);
                    });
                    
                    break;
            }
        }

         UnityAction CreatePawnAction(CommunicationMessage<Dictionary<string, string>> message)
         {
             UnityAction result = () => { };
             
             var objectType = (GameObjectType)Enum.Parse(typeof(GameObjectType),
                 message.body.Any["ObjectType"]);
                    
             var position = NumericParser.ParseVector(message.body.Any["Position"]);
             var quaternion = NumericParser.ParseQuaternion(message.body.Any["Quaternion"]);

             var id = long.Parse(message.body.Any["ID"]);

             #region GameObject Type

             switch (objectType)
             {
                 case GameObjectType.Tank:

                     var subObjectType = (TankType)Enum.Parse(typeof(TankType),
                         message.body.Any["ObjectSubType"]);
                     
                     result = () =>
                     {
                         var tank = Instantiate(tankInstances[(int)subObjectType], position, quaternion);
                         tank.id = id;
                         Pawns.Add(id, tank);
                     };

                     break;
             }
                    
             #endregion

             return result;
         }

         UnityAction CreateChangeTankPositionAction(CommunicationMessage<Dictionary<string, string>> message)
         {
             UnityAction result;
             
             var id = long.Parse(message.body.Any["ID"]);
             
             var rawPositionData = message.body.Any["Position"];
             var readedPositionData = NumericParser.ParseVector(rawPositionData);

             var rawRotationData = message.body.Any["Quaternion"];
             var readedRotationData = NumericParser.ParseQuaternion(rawRotationData);

             var rawTowerRotationData = message.body.Any["TowerQuaternion"];
             var readedTowerRotationData = NumericParser.ParseQuaternion(rawTowerRotationData);

             var rawCannonRotationData = message.body.Any["CannonQuaternion"];
             var readedCannonRotationData = NumericParser.ParseQuaternion(rawCannonRotationData);

             result = () =>
             {
                 var tank = Pawns[id] as Tank;
                 if (tank == null) return;

                 tank.controller.enabled = false;

                 var trnsf = tank.transform;
                 
                 trnsf.localRotation = readedRotationData;
                 trnsf.position = readedPositionData;
                 tank.tower.transform.rotation = readedTowerRotationData;
                 tank.cannon.transform.rotation = readedCannonRotationData;
                 
                 tank.controller.enabled = true;
             };

             return result;
         }

         UnityAction PossessPlayertoPawnAction(long id, Player.Player player)
         {
             return () =>
             {
                 var pawn = Pawns[id];
                 
                 if(!pawn.GetType().IsAssignableFrom(typeof(Tank))) return;
                 
                 player.pawn = (Tank)pawn;
             };
         }
         
         UnityAction CreateChangeTankMovingAction(CommunicationMessage<Dictionary<string, string>> message)
         {
             UnityAction result;
             
             var id = long.Parse(message.body.Any["ID"]);

             var moveDirection = NumericParser.ParseVector(message.body.Any["MoveDirection"]);
             var moveDelta = float.Parse(message.body.Any["MoveDelta"]);
             var rotationVector = NumericParser.ParseVector(message.body.Any["RotationVector"]);
             var rotationDelta = float.Parse(message.body.Any["RotationDelta"]);
             var towerRotationVector = NumericParser.ParseVector(message.body.Any["TowerRotationVector"]);
             var towerRotationDelta = float.Parse(message.body.Any["TowerRotationDelta"]);
             var cannonRotationVector = NumericParser.ParseVector(message.body.Any["CannonRotationVector"]);
             var cannonRotationDelta = float.Parse(message.body.Any["CannonRotationDelta"]);

             result = () =>
             {
                 var tank = Pawns[id] as Tank;
                 if (tank == null) return;

                 tank.moveVector = moveDirection;
                 tank.moveDelta = moveDelta;
                 tank.rotationVector = rotationVector;
                 tank.rotationDelta = rotationDelta;
                 tank.towerRotateVector = towerRotationVector;
                 tank.towerRotationDelta = towerRotationDelta;
                 tank.cannonRotateVector = cannonRotationVector;
                 tank.cannonRotationDelta = cannonRotationDelta;
             };

             return result;
         }

    }
}
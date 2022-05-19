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
                        ["ID"] = player.pawn.id.ToString(),
                        ["Position"] = pawnTransform.position.ToString(),
                        ["Velocity"] =  player.pawn.velocity.ToString(),
                        ["Quaternion"] = pawnTransform.localRotation.ToString(),
                        ["TowerQuaternion"] = player.pawn.tower.transform.localRotation.ToString(),
                        ["CannonQuaternion"] = player.pawn.cannon.transform.localRotation.ToString(),
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
                
                case MessageType.GameObjectDestroyReport:

                    id = long.Parse(message.body.Any["ID"]);
                    
                    AddAction(() =>
                    {
                        Destroy(Pawns[id].gameObject);
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
             
             var position = NumericParser.ParseVector(message.body.Any["Position"]);
             var velocity = NumericParser.ParseVector(message.body.Any["Velocity"]);
             var quaternion = NumericParser.ParseQuaternion(message.body.Any["Quaternion"]);
             var towerQuaternion = NumericParser.ParseQuaternion(message.body.Any["TowerQuaternion"]);
             var cannonQuaternion = NumericParser.ParseQuaternion(message.body.Any["CannonQuaternion"]);

             result = () =>
             {
                 var tank = Pawns[id] as Tank;
                 if (tank == null) return;

                 tank.controller.Move(velocity * Time.deltaTime / 0.1f);
                 tank.transform.localRotation = quaternion;
                 tank.tower.transform.localRotation = towerQuaternion;
                 tank.cannon.transform.localRotation = cannonQuaternion;
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
    }
}
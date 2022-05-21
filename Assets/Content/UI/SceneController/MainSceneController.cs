using System;
using System.Collections.Generic;
using Content.Communication;
using Content.Communication.Protocol;
using Content.Communication.TickTasking;
using Content.Pawn;
using Content.UI.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Content.UI.SceneController
{
    public class MainSceneController : ControllerBase
    {
        private Communicator _communicator;
        [Header("Play")]
        public Player.Player player;
        public List<Tank> tankInstances;
        public Dictionary<long, Pawn.Pawn> Pawns;
        private SendPawnMoving _sendPawnMoving;
        private SendPawnPosition _sendPawnPosition;
        
        [Header("Chat")]
        public LocalMessageWindow messageWindow;

        public ChatInput chatWindow;

        void Awake()
        {
            Pawns = new Dictionary<long, Pawn.Pawn>();
            
            _communicator = Communicator.Instance;
            
            _sendPawnMoving = new SendPawnMoving();
            _sendPawnMoving.TickEvent += SendPawnMove;
            _sendPawnPosition = new SendPawnPosition();
            _sendPawnPosition.TickEvent += SendPawnPos;
        }

        void Start()
        {
            chatWindow.chatInputChangedEvent.AddListener((isWindowFocused) =>
            {
                player.playerMode = isWindowFocused ? 
                    Player.Player.PlayerMode.UIMode : Player.Player.PlayerMode.PlayMode;
            });
            
            chatWindow.chatSendMessageEvent.AddListener((chat) =>
            {
                _communicator.SendData(new CommunicationMessage<Dictionary<string,string>>()
                {
                    header = new Header()
                    {
                        MessageName = MessageType.PlayerChatReport.ToString()
                    },
                    body = new Body<Dictionary<string, string>>()
                    {
                        Any = new Dictionary<string,string>
                        {
                            ["Sender"] = player.pawn.pawnName,
                            ["Message"] = chat
                        }
                    }
                });
            });
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
            Debug.LogError("Main Scene Controller Destroyed");
            //Unsubscribe Event
            _communicator.dataReceivedEvent.RemoveListener(DataReceivedEventActivated);
        }

        private void FixedUpdate()
        {
            #region Send Current Position/Move
            if (!player.pawn) return;
            
            _sendPawnMoving.CountTick(Time.fixedDeltaTime);
            _sendPawnPosition.CountTick(Time.fixedDeltaTime);
            
            #endregion
        }
        
        private void SendPawnPos(object sender, EventArgs arg)
        {
            AddAction(() =>
            {
                var pawn = player.pawn;

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
                            ["ID"] = pawn.id.ToString(),
                            ["Position"] = pawn.transform.position.ToString(),
                            ["Quaternion"] = pawn.transform.rotation.ToString(),
                            ["TowerQuaternion"] = pawn.tower.transform.rotation.ToString(),
                            ["CannonQuaternion"] = pawn.cannon.transform.rotation.ToString()
                        }
                    }
                };

                _communicator.SendData(message);
            });
        }

        private void SendPawnMove(object sender, EventArgs arg)
        {
            AddAction(() =>
            {
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
            });
        }
        
        
         void DataReceivedEventActivated(CommunicationMessage<Dictionary<string,string>> message)
        {
            MessageType messageName = (MessageType)Enum.Parse(typeof(MessageType), message.header.MessageName);

            long id;

            switch (messageName)
            {
                case MessageType.PlayerChatReport:

                    AddAction(() =>
                    {
                        messageWindow.AddMessage(
                            message.body.Any["Sender"],
                            message.body.Any["Message"]
                            );
                    });

                    break;
                
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

                    AddAction(DestroyPawn(id));
                    
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

             var pawnName = message.body.Any["PawnName"];
             
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
                         tank.pawnName = pawnName;
                         
                         Pawns.Add(id, tank);
                     };

                     break;
             }
                    
             #endregion

             return result;
         }

         UnityAction CreateChangeTankPositionAction(CommunicationMessage<Dictionary<string, string>> message)
         {
             var id = long.Parse(message.body.Any["ID"]);
             
             var rawPositionData = message.body.Any["Position"];
             var readedPositionData = NumericParser.ParseVector(rawPositionData);

             var rawRotationData = message.body.Any["Quaternion"];
             var readedRotationData = NumericParser.ParseQuaternion(rawRotationData);

             var rawTowerRotationData = message.body.Any["TowerQuaternion"];
             var readedTowerRotationData = NumericParser.ParseQuaternion(rawTowerRotationData);

             var rawCannonRotationData = message.body.Any["CannonQuaternion"];
             var readedCannonRotationData = NumericParser.ParseQuaternion(rawCannonRotationData);

             void Result()
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
             }

             return Result;
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
             var id = long.Parse(message.body.Any["ID"]);

             var moveDirection = NumericParser.ParseVector(message.body.Any["MoveDirection"]);
             var moveDelta = float.Parse(message.body.Any["MoveDelta"]);
             var rotationVector = NumericParser.ParseVector(message.body.Any["RotationVector"]);
             var rotationDelta = float.Parse(message.body.Any["RotationDelta"]);
             var towerRotationVector = NumericParser.ParseVector(message.body.Any["TowerRotationVector"]);
             var towerRotationDelta = float.Parse(message.body.Any["TowerRotationDelta"]);
             var cannonRotationVector = NumericParser.ParseVector(message.body.Any["CannonRotationVector"]);
             var cannonRotationDelta = float.Parse(message.body.Any["CannonRotationDelta"]);

             void Result()
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
             }

             return Result;
         }


         UnityAction DestroyPawn(long id)
         {
             void Result()
             {
                 if (!Pawns.ContainsKey(id)) return;
                 Pawns.TryGetValue(id, out var pawn);
                 Destroy(pawn);
             }

             return Result;
         }
    }
}
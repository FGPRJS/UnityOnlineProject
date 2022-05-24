using System;
using System.Collections.Generic;
using Content.Communication;
using Content.Communication.Protocol;
using Content.Communication.TickTasking;
using Content.Pawn;
using Content.Pawn.Bullet;
using Content.Pawn.Bumper;
using Content.UI.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Content.UI.SceneController
{
    public class MainSceneController : ControllerBase
    {
        private Communicator _communicator;
        [Header("Play")]
        public Player.Player player;
        public List<Tank> tankInstances;
        public TankControlBumper tankControlBumperInstance;
        public List<Bullet> bulletInstances;
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
                    MessageName = MessageType.PlayerTankSpawnRequest.ToString()
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
                var pawnBumper = pawn.bumper;

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
                            ["MoveDirection"] = pawnBumper.moveVector.ToString(),
                            ["MoveDelta"] = pawnBumper.moveDelta.ToString(),
                            ["MoveSpeed"] = pawn.pawnData.Speed.ToString(),
                            ["RotationVector"] = pawnBumper.rotationVector.ToString(),
                            ["RotationDelta"] = pawnBumper.rotationDelta.ToString(),
                            ["TowerRotationVector"] = pawnBumper.towerRotateVector.ToString(),
                            ["TowerRotationDelta"] = pawnBumper.towerRotationDelta.ToString(),
                            ["CannonRotationVector"] = pawnBumper.cannonRotateVector.ToString(),
                            ["CannonRotationDelta"] = pawnBumper.cannonRotationDelta.ToString(),
                            ["FrameRate"] = (1 / Time.deltaTime).ToString()
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
                
                case MessageType.PlayerTankSpawnRequest:

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
                
                case MessageType.BulletSpawnRequest :
                    
                    AddAction(CreateBullet(message));

                    break;
                
                case MessageType.GameObjectDestroyReport:

                    id = long.Parse(message.body.Any["ID"]);

                    if (!Pawns.ContainsKey(id)) return;
                    
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

             var subObjectType = (TankType)Enum.Parse(typeof(TankType),
                 message.body.Any["ObjectSubType"]);
             
             result = () =>
             {
                 var bumper = Instantiate(tankControlBumperInstance, position, quaternion);

                 var tank = Instantiate(tankInstances[(int)subObjectType], position, quaternion);
                 tank.id = id;
                 tank.pawnName = pawnName;
                 tank.bumper = bumper;

                 bumper.targetTank = tank;
                 
                 Pawns.Add(id, tank);
             };
             
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

                 var receivedPositionDateTime = message.header.SendTime;

                 var tankBumper = tank.bumper;
                 var tankBumperTransform = tankBumper.transform;

                 var latency = DateTime.Now - receivedPositionDateTime;
                 
                 var positionGap = (tankBumper.moveVector
                                    * tankBumper.moveDelta
                                    * tank.pawnData.Speed
                                    * (latency.Milliseconds / 1000 + latency.Seconds)
                                    * tank.frameRate);

                 var predictedPosition = readedPositionData + positionGap;
                 
                 tankBumperTransform.position = Vector3.Lerp(tankBumperTransform.position, predictedPosition, 0.5f);
                 tankBumperTransform.localRotation = readedRotationData;

                 tank.tower.transform.rotation = readedTowerRotationData;
                 tank.cannon.transform.rotation = readedCannonRotationData;
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
                 player.pawn.mainCamera.Priority = (int)CameraPriority.Playing;
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
             var rawFrameRate = message.body.Any["FrameRate"];
             var frameRate = float.Parse(rawFrameRate);

             void Result()
             {
                 var tank = Pawns[id] as Tank;
                 if (tank == null) return;

                 var tankBumper = tank.bumper;

                 tankBumper.moveVector = moveDirection;
                 tankBumper.moveDelta = moveDelta;
                 tankBumper.rotationVector = rotationVector;
                 tankBumper.rotationDelta = rotationDelta;
                 tankBumper.towerRotateVector = towerRotationVector;
                 tankBumper.towerRotationDelta = towerRotationDelta;
                 tankBumper.cannonRotateVector = cannonRotationVector;
                 tankBumper.cannonRotationDelta = cannonRotationDelta;
                 tank.frameRate = frameRate;
             }

             return Result;
         }

         UnityAction CreateBullet(CommunicationMessage<Dictionary<string, string>> message)
         {
             var id = long.Parse(message.body.Any["ID"]);

             var position = NumericParser.ParseVector(message.body.Any["Position"]);
             var quaternion = NumericParser.ParseQuaternion(message.body.Any["Quaternion"]);
             var force = NumericParser.ParseVector(message.body.Any["Force"]);

             #region GameObject Type

             var subObjectType = (BulletType)Enum.Parse(typeof(BulletType),
                 message.body.Any["ObjectSubType"]);
             
             void Action()
             {
                 var bullet = Instantiate(bulletInstances[(int)subObjectType], position, quaternion);
                 if(Pawns.ContainsKey(id))
                    bullet.pawnOwner = Pawns[id];

                 var normalBullet = bullet.GetComponent<NormalBullet>();
                 if (!normalBullet) return;

                 var bulletRigidBody = bullet.GetComponent<Rigidbody>();
                 if (!bulletRigidBody) return;
                 
                 bulletRigidBody.AddForce(force);
             };
             
             #endregion

             return Action;
         }

         UnityAction DestroyPawn(long id)
         {
             void Result()
             {
                 Pawns.TryGetValue(id, out var pawn);
                 Pawns.Remove(id);
                 if (pawn != null) Destroy(pawn.gameObject);
             }

             return Result;
         }
    }
}
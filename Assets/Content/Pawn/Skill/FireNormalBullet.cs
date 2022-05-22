using System.Collections.Generic;
using Content.Communication;
using Content.Communication.Protocol;
using Content.Pawn.Bullet;
using JetBrains.Annotations;
using UnityEngine;

namespace Content.Pawn.Skill
{
    public class FireNormalBullet : ASkill
    {
        [System.Diagnostics.CodeAnalysis.NotNull]
        public GameObject targetMuzzle;

        private void Awake()
        {
            Cooltime = 3.0f;
        }

        [CanBeNull]
        public override void UseSkill()
        {
            if (CurrentCooltime > 0) return;
            
            CurrentCooltime = Cooltime;

            var muzzleTransform = targetMuzzle.transform;
            
            var message = new CommunicationMessage<Dictionary<string, string>>()
            {
                header = new Header()
                {
                    MessageName = MessageType.BulletSpawnRequest.ToString()
                },
                body = new Body<Dictionary<string, string>>()
                {
                    Any = new Dictionary<string, string>()
                    {
                        ["ID"] = skillOwner.id.ToString(),
                        ["ObjectType"] = GameObjectType.Bullet.ToString(),
                        ["ObjectSubType"] = BulletType.Normal.ToString(),
                        ["Position"] = muzzleTransform.position.ToString(),
                        ["Quaternion"] = muzzleTransform.rotation.ToString(),
                        ["Force"] = (muzzleTransform.forward * 1000).ToString()
                    }
                }
            };
            
            Communicator.Instance.SendData(message);
        }
    }
}
using UnityEngine;

namespace Script
{
    [CreateAssetMenu(fileName = "Tank Data")]
    public class TankData : ScriptableObject
    {
        [SerializeField]
        private long hp;
        [SerializeField]
        private long maxHP;

        [SerializeField]
        private long damage;
        
        [SerializeField]
        private long speed;
        [SerializeField]
        private long rotateSpeed;
        [SerializeField]
        private long towerRotateSpeed;
        [SerializeField]
        private long cannonRotateSpeed;

        public float cannonRotationUPLimit;
        public float cannonRotationDOWNLimit;
        
        [SerializeField]
        private long jumpPower;
        
        #region Encapsulation
        public long Hp
        {
            get => hp;
            set
            {
                if (value > MaxHp)
                {
                    hp = MaxHp;
                }
                else if (value < 0)
                {
                    hp = 0;
                }
                else
                {
                    hp = value;
                }
            }
        }

        public long MaxHp
        {
            get => maxHP;
            set
            {
                if (value < hp)
                {
                    maxHP = value;
                    hp = maxHP;
                }
                else if (value < 0)
                {
                    maxHP = 0;
                }
                else
                {
                    maxHP = value;
                }
            }
        }

        public long Damage
        {
            get => damage;
            set
            {
                if (value < 0)
                {
                    damage = 0;
                }
                else
                {
                    damage = value;
                }
            }
        }

        public long Speed
        {
            get => speed;
            set
            {
                if (value < 0)
                {
                    speed = 0;
                }
                else
                {
                    speed = value;
                }
            }
        }

        public long JumpPower
        {
            get => jumpPower;
            set
            {
                if (value < 0)
                {
                    jumpPower = 0;
                }
                else
                {
                    jumpPower = value;
                }
            }
        }

        public long RotateSpeed
        {
            get => rotateSpeed;
            set
            {
                if (value < 0)
                {
                    rotateSpeed = 0;
                }
                else
                {
                    rotateSpeed = value;
                }
            }
        }

        public long TowerRotateSpeed
        {
            get => towerRotateSpeed;
            set
            {
                if (value < 0)
                {
                    towerRotateSpeed = 0;
                }
                else
                {
                    towerRotateSpeed = value;
                }
            }
        }

        public long CannonRotateSpeed
        {
            get => cannonRotateSpeed;
            set
            {
                if (value < 0)
                {
                    cannonRotateSpeed = 0;
                }
                else
                {
                    cannonRotateSpeed = value;
                }
            }
        }

        #endregion
    }
}

using UnityEngine;

namespace Content.Pawn
{
    [CreateAssetMenu(fileName = "PawnData")]
    public class PawnData : ScriptableObject
    {
        [SerializeField]
        private long hp;
        [SerializeField]
        private long maxHP;

        [SerializeField]
        private long damage;
        
        [SerializeField]
        private float speed;
        [SerializeField]
        private float rotateSpeed;
        
        [SerializeField]
        private float jumpPower;
        
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

        public float Speed
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

        public float JumpPower
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

        public float RotateSpeed
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
        
        #endregion
    }
}
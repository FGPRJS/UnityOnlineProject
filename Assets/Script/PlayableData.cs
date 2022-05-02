using UnityEngine;

namespace Script
{
    [CreateAssetMenu(fileName = "Playable Data")]
    public class PlayableData : ScriptableObject
    {
        [SerializeField]
        private long hp;
        [SerializeField]
        private long maxHP;

        [SerializeField]
        private long damage;
        
        
        #region Encapsulation
        public long Hp
        {
            get => hp;
            set => hp = value;
        }

        public long MaxHp
        {
            get => maxHP;
            set => maxHP = value;
        }

        public long Damage1
        {
            get => damage;
            set => damage = value;
        }
        #endregion
    }
}

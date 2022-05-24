using UnityEngine;

namespace Script
{
    [CreateAssetMenu(fileName = "TankData")]
    public class TankData : ScriptableObject
    {
        [SerializeField]
        private long towerRotateSpeed;
        [SerializeField]
        private long cannonRotateSpeed;

        public float cannonRotationUPLimit;
        public float cannonRotationDOWNLimit;

        #region Encapsulation

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

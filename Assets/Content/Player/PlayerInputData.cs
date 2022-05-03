using UnityEngine;

namespace Script
{
    [CreateAssetMenu(fileName = "Player Input Data")]
    public class PlayerInputData : ScriptableObject
    {
        public string MoveActionName;
        public string LookActionName;
        public string JumpActionName;
        public string AttackActionName;
    }
}

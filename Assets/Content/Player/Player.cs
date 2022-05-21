using Content.Input;
using Content.Pawn;
using Content.UI;
using UnityEngine;

namespace Content.Player
{
    public class Player : MonoBehaviour
    {
        public Tank pawn;

        public enum PlayerMode
        {
            PlayMode,
            UIMode
        }

        public PlayerMode playerMode;
        private PlayerInputManager _inputManager;

        void Awake()
        {
            _inputManager = PlayerInputManager.Instance;
        }
        
        void Update()
        {
            #region PAWN Control
            if (!pawn) return;
            if (playerMode == PlayerMode.UIMode) return;
            
            var readedMoveAction = _inputManager.moveAction.ReadValue<Vector2>();
            readedMoveAction *= Time.deltaTime;
            
            pawn.rotationDelta = readedMoveAction.x;
            pawn.moveDelta = readedMoveAction.y;
            
            var readedLookAction = _inputManager.lookAction.ReadValue<Vector2>();
            readedLookAction *= Time.deltaTime;
            
            pawn.cannonRotationDelta = readedLookAction.y;
            pawn.towerRotationDelta = readedLookAction.x;
            #endregion
        }
    }
}

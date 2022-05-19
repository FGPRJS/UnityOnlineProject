using Content.Pawn;
using Script;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Player
{
    public class Player : MonoBehaviour
    {
        public Tank pawn;
        
        public PlayerInput input;
        public PlayerInputData inputData;

        private InputAction moveAction;

        private InputAction lookAction;
        private InputAction attackAction;
        private InputAction jumpAction;
        
        // Start is called before the first frame update
        void Start()
        {
            //Set Input Actions
            moveAction = input.actions[inputData.MoveActionName];
            lookAction = input.actions[inputData.LookActionName];
            attackAction = input.actions[inputData.AttackActionName];
            jumpAction = input.actions[inputData.JumpActionName];
        }

        // Update is called once per frame
        void Update()
        {
            #region PAWN Control
            if (!pawn) return;
            
            var readedMoveAction = moveAction.ReadValue<Vector2>();
            readedMoveAction *= Time.deltaTime;
            
            pawn.rotationDelta = readedMoveAction.x;
            pawn.moveDelta = readedMoveAction.y;

            var readedLookAction = lookAction.ReadValue<Vector2>();
            readedLookAction *= Time.deltaTime;

            pawn.cannonRotationDelta = readedLookAction.y;
            pawn.towerRotationDelta = readedLookAction.x;
            #endregion
        }
    }
}

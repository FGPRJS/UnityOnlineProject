using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script
{
    public class Player : MonoBehaviour
    {
        public PlayableData playerData;
        public PlayerInput input;
        public PlayerInputData inputData;

        [SerializeField]
        private CharacterController controller;
        
        private InputAction moveAction;
        private float gravityYValue;
        
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
            #region Move
            #region Gravity

            gravityYValue +=
                Physics.gravity.y * Time.deltaTime;
            
            #endregion
            var readedMoveAction = moveAction.ReadValue<Vector2>();
            Vector3 normalizedMoveVector = 
                new Vector3(readedMoveAction.x, 0, readedMoveAction.y).normalized;
            var resultMoveVector = normalizedMoveVector * (playerData.Speed * Time.deltaTime);
            resultMoveVector.y += gravityYValue * Time.deltaTime;
            controller.Move(resultMoveVector);

            #endregion
            
            #region Jump

            var readedJumpAction = jumpAction.ReadValue<float>();
            if ((readedJumpAction > 0) && (controller.isGrounded))
            {
                gravityYValue = playerData.JumpPower;
            }
            
            #endregion
        }
    }
}

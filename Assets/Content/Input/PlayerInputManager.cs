using Script;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Content.Input
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance;
        
        public PlayerInput input;
        public PlayerInputData inputData;

        public InputAction moveAction;
        public InputAction lookAction;
        public InputAction attackAction;
        public InputAction jumpAction;
        public InputAction enterAction;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                
                //Set Input Actions
                moveAction = input.actions[inputData.MoveActionName];
                lookAction = input.actions[inputData.LookActionName];
                attackAction = input.actions[inputData.AttackActionName];
                jumpAction = input.actions[inputData.JumpActionName];
                enterAction = input.actions[inputData.EnterActionName];
                
            }
            else
            {
                Destroy(this);
            }
            
            DontDestroyOnLoad(Instance);
        }
    }
}
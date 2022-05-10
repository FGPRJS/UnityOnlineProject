using UnityEngine;

namespace Content.Pawn
{
    public class Pawn : MonoBehaviour
    {
        public long id;
        public string pawnName;
        
        protected float GravityYValue;

        [SerializeField]
        public CharacterController controller;

        protected virtual void Update()
        {
            #region Gravity
            GravityYValue +=
                Physics.gravity.y * Time.deltaTime;
            controller.Move(new Vector3(0, GravityYValue, 0));
            #endregion
        }
    }
}
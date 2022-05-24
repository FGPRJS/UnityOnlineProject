using UnityEngine;

namespace Content.Pawn.Bumper
{
    public class TankControlBumper : MonoBehaviour
    {
        public Tank targetTank;
        
        public CharacterController controller;
        
        public Vector3 GravityValue;

        public Vector3 moveVector = Vector3.zero;
        public float moveDelta = 0.0f;
        public Vector3 rotationVector = Vector3.zero;
        public float rotationDelta = 0.0f;
        public Vector3 towerRotateVector = Vector3.zero;
        public float towerRotationDelta = 0.0f;
        public Vector3 cannonRotateVector = Vector3.zero;
        public float cannonRotationDelta = 0.0f;
        
        protected void Update()
        {
            if(!targetTank) Destroy(gameObject);
            
            #region Gravity
            GravityValue +=
                Physics.gravity * Time.deltaTime;
            if (controller.isGrounded) GravityValue = Vector3.zero;
            #endregion
            
            //Rotate
            rotationVector.y += rotationDelta * targetTank.pawnData.RotateSpeed;
            
            transform.rotation = Quaternion.Euler(rotationVector);

            //Rotate Tower
            towerRotateVector.y += towerRotationDelta * targetTank.tankData.TowerRotateSpeed;
            
            //tower.transform.rotation = Quaternion.Euler(towerRotateVector);
            
            //Rotate Cannon
            cannonRotateVector.x += cannonRotationDelta * targetTank.tankData.CannonRotateSpeed;
            
            if (cannonRotateVector.x > targetTank.tankData.cannonRotationUPLimit)
            {
                cannonRotateVector.x = targetTank.tankData.cannonRotationUPLimit;
            }
            else if(cannonRotateVector.x < targetTank.tankData.cannonRotationDOWNLimit)
            {
                cannonRotateVector.x = targetTank.tankData.cannonRotationDOWNLimit;
            }

           // Vector3 cannonRotationVector3 = cannon.transform.rotation.eulerAngles;
            //cannonRotationVector3.x = cannonRotateVector.x;
            //cannon.transform.rotation = Quaternion.Euler(cannonRotationVector3);
            
            //MoveForward
            moveVector = transform.TransformDirection(Vector3.forward);

            controller.Move(moveVector * (moveDelta * 2f * targetTank.pawnData.Speed) + GravityValue);
        }
    }
}
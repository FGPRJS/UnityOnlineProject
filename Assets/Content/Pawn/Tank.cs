using System;
using Content.Pawn.Skill;
using Script;
using UnityEngine;

namespace Content.Pawn
{
    public class Tank : Pawn
    {
        public TankData data;

        public GameObject tower;
        public GameObject cannon;

        public Vector3 moveVector = Vector3.zero;
        public float moveDelta = 0.0f;
        public Vector3 rotationVector = Vector3.zero;
        public float rotationDelta = 0.0f;
        public Vector3 towerRotateVector = Vector3.zero;
        public float towerRotationDelta = 0.0f;
        public Vector3 cannonRotateVector = Vector3.zero;
        public float cannonRotationDelta = 0.0f;

        [Header("SKILL")]
        public FireNormalBullet fireSkill;


        protected void Update()
        {
            #region Gravity
            GravityValue +=
                Physics.gravity * Time.deltaTime;
            if (controller.isGrounded) GravityValue = Vector3.zero;
            #endregion
            
            //Rotate
            rotationVector.y += rotationDelta * data.RotateSpeed;
            
            transform.rotation = Quaternion.Euler(rotationVector);

            //Rotate Tower
            towerRotateVector.y += towerRotationDelta * data.TowerRotateSpeed;
            
            tower.transform.rotation = Quaternion.Euler(towerRotateVector);
            
            //Rotate Cannon
            cannonRotateVector.x += cannonRotationDelta * data.CannonRotateSpeed;
            
            if (cannonRotateVector.x > data.cannonRotationUPLimit)
            {
                cannonRotateVector.x = data.cannonRotationUPLimit;
            }
            else if(cannonRotateVector.x < data.cannonRotationDOWNLimit)
            {
                cannonRotateVector.x = data.cannonRotationDOWNLimit;
            }

            Vector3 cannonRotationVector3 = cannon.transform.rotation.eulerAngles;
            cannonRotationVector3.x = cannonRotateVector.x;
            cannon.transform.rotation = Quaternion.Euler(cannonRotationVector3);
            
            //MoveForward
            controller.Move(
                transform.TransformDirection(Vector3.forward * (moveDelta * data.Speed)
                    + GravityValue));

            velocity = controller.velocity;
        }
    }
}

using System;
using Content.Pawn.Skill;
using Script;
using UnityEngine;
using UnityEngine.Serialization;

namespace Content.Pawn
{
    public class Tank : Pawn
    {
        public TankData tankData;

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
            rotationVector.y += rotationDelta * pawnData.RotateSpeed;
            
            transform.rotation = Quaternion.Euler(rotationVector);

            //Rotate Tower
            towerRotateVector.y += towerRotationDelta * tankData.TowerRotateSpeed;
            
            tower.transform.rotation = Quaternion.Euler(towerRotateVector);
            
            //Rotate Cannon
            cannonRotateVector.x += cannonRotationDelta * tankData.CannonRotateSpeed;
            
            if (cannonRotateVector.x > tankData.cannonRotationUPLimit)
            {
                cannonRotateVector.x = tankData.cannonRotationUPLimit;
            }
            else if(cannonRotateVector.x < tankData.cannonRotationDOWNLimit)
            {
                cannonRotateVector.x = tankData.cannonRotationDOWNLimit;
            }

            Vector3 cannonRotationVector3 = cannon.transform.rotation.eulerAngles;
            cannonRotationVector3.x = cannonRotateVector.x;
            cannon.transform.rotation = Quaternion.Euler(cannonRotationVector3);
            
            //MoveForward
            moveVector = transform.TransformDirection(Vector3.forward);

            controller.Move(moveVector * (moveDelta * pawnData.Speed) + GravityValue);
        }
    }
}

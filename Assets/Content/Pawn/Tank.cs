using System;
using Script;
using UnityEngine;

namespace Content.Pawn
{
    public class Tank : Pawn
    {
        public TankData data;

        public GameObject tower;
        public GameObject cannon;

        private Vector3 _moveVector = Vector3.zero;
        private Vector3 _rotationVector = Vector3.zero;
        private Vector3 _towerRotateVector = Vector3.zero;
        private Vector3 _cannonRotateVector = Vector3.zero;

        public void RotateTank(float deltaValue)
        {
            _rotationVector.y += deltaValue * data.RotateSpeed;
        }

        public void RotateTower(float deltaValue)
        {
            _towerRotateVector.y += deltaValue * data.TowerRotateSpeed;
        }

        public void RotateCannon(float deltaValue)
        {
            _cannonRotateVector.x += deltaValue * data.CannonRotateSpeed;
            
            if (_cannonRotateVector.x > data.cannonRotationUPLimit)
            {
                _cannonRotateVector.x = data.cannonRotationUPLimit;
            }
            else if(_cannonRotateVector.x < data.cannonRotationDOWNLimit)
            {
                _cannonRotateVector.x = data.cannonRotationDOWNLimit;
            }
        }

        public void MoveForward(float deltaValue)
        {
            controller.Move(
                transform.TransformDirection(Vector3.forward * (deltaValue * data.Speed)));
        }
        
        protected override void Update()
        {
            base.Update();
            
            transform.rotation = Quaternion.Euler(_rotationVector);
            tower.transform.rotation = Quaternion.Euler(_towerRotateVector);
            
            Vector3 connonRotationVector3 = cannon.transform.rotation.eulerAngles;
            connonRotationVector3.x = -_cannonRotateVector.x;
            cannon.transform.rotation = Quaternion.Euler(connonRotationVector3);
        }
    }
}

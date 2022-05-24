using System;
using Content.Pawn.Bumper;
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

        
        [Header("Control")]
        [NonSerialized]
        public TankControlBumper bumper;
        public CharacterController controller;
        
        [Header("SKILL")]
        public FireNormalBullet fireSkill;

        private void Update()
        {
            //Succeeding ControlBumper
            var difference = bumper.transform.position - transform.position;
            var moveDirection = difference.normalized;
            if(difference.magnitude > 1)
                controller.Move(moveDirection * (pawnData.Speed * Time.deltaTime));

            transform.rotation = bumper.transform.rotation;
            

        }
    }
}

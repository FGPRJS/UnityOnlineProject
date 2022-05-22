using System;
using System.Collections.Generic;
using Content.Communication.Protocol;
using JetBrains.Annotations;
using UnityEngine;

namespace Content.Pawn.Skill
{
    public class ASkill : MonoBehaviour
    {
        public Pawn skillOwner;
        
        protected float _cooltime;
        public float Cooltime{
            set
            {
                if (value > 0)
                {
                    _cooltime = value;
                }
                else
                {
                    _cooltime = 0;
                }
            }

            get
            {
                return _cooltime;
            }
        }
        protected float _currentCooltime;

        public float CurrentCooltime
        {
            set
            {
                if (value > 0)
                {
                    _currentCooltime = value;
                }
                else
                {
                    _currentCooltime = 0;
                }
            }

            get
            {
                return _currentCooltime;
            }
        }

        [CanBeNull]
        public virtual void UseSkill()
        {
            return;
        }

        protected virtual void FixedUpdate()
        {
            CurrentCooltime -= Time.fixedDeltaTime;
        }
    }
}
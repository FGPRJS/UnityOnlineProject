using System;
using UnityEngine;

namespace Content.Pawn.Bullet
{
    public class NormalBullet : Bullet
    {
        public long damage = 100;
        
        protected override void BulletEntertoPawn(Pawn pawn)
        {
            
        }
    }
}
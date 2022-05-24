using UnityEngine;

namespace Content.Pawn.Bullet
{
    public class Bullet : MonoBehaviour
    {
        public Pawn pawnOwner;

        private void OnTriggerEnter(Collider other)
        {
            var targetPawn = other.gameObject.GetComponent<Pawn>();
            if (targetPawn)
            {
                BulletEntertoPawn(targetPawn);
            }
            
            Destroy(gameObject);
        }
        
        protected virtual void BulletEntertoPawn(Pawn pawn)
        {
            
        }
    }
}
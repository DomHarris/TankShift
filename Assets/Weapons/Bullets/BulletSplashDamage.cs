using Entity.Damage;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bullets
{
    public class BulletSplashDamage : BulletImpact
    {
        [SerializeField] private float range;

        [SerializeField] private LayerMask layersToHit;
        
        private Collider2D[] _results = new Collider2D[16];
        
        protected override void DoDamage(HitData damagePacket, IHitReceiver[] receivers)
        {
            var numHit = Physics2D.OverlapCircleNonAlloc(transform.position, range, _results, layersToHit);
            
            for (int i = 0; i < numHit; i++)
            {
                receivers = _results[i].GetComponentsInChildren<IHitReceiver>();
                foreach (var receiver in receivers)
                    receiver.ReceiveHit(damagePacket);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
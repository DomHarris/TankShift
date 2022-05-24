using UnityEngine;

namespace Entity.Damage
{
    [RequireComponent(typeof(PhysicsEntity))]
    public class EntityKnockbackOnHit : MonoBehaviour, IHitReceiver
    {
        [SerializeField] private float knockbackAmount;
        [SerializeField] private DamageType damageTypes;
        
        private PhysicsEntity _entity;

        private void Awake()
        {
            _entity = GetComponent<PhysicsEntity>();
        }

        public void ReceiveHit(HitData data)
        {
            if (data.DamageType == damageTypes)
                _entity.AddForce(data.IncomingDirection * knockbackAmount);
        }
    }
}
using Entity.Stats;
using UnityEngine;

namespace Entity.Damage
{
    [RequireComponent(typeof(PhysicsEntity))]
    public class EntityKnockbackOnHit : MonoBehaviour, IHitReceiver
    {
        [SerializeField, StatTypeWithParent]
        private StatType knockbackAmount;

        private float KnockbackAmount => _stats.GetStat(knockbackAmount);
        [SerializeField] private DamageType damageTypes;
        
        private PhysicsEntity _entity;
        private StatController _stats;

        private void Awake()
        {
            _entity = GetComponent<PhysicsEntity>();
            _stats = GetComponentInParent<StatController>();
        }

        public void ReceiveHit(HitData data)
        {
            if (data.DamageType == damageTypes)
                _entity.AddForce(data.IncomingDirection * KnockbackAmount);
        }
    }
}
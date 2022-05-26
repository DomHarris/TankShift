using Entity.Stats;
using UnityEngine;
using Utils;

namespace Entity.Damage
{
    /// <summary>
    /// Use the physics entity to move an enemy when it gets hit
    /// </summary>
    [RequireComponent(typeof(PhysicsEntity))]
    public class EntityKnockbackOnHit : MonoBehaviour, IHitReceiver
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, StatTypeWithParent]
        private StatType knockbackAmount;

        [SerializeField, EnumFlags] private DamageType damageTypes;
        #endregion

        // Properties - access functions 
        #region Properties
        // Get the knockback amount from the stat controller
        private float KnockbackAmount => _stats.GetStat(knockbackAmount);
        #endregion

        // Private fields - only used in this script
        #region PrivateFields
        // the physics entity that we're moving
        private PhysicsEntity _entity;
        // the stat controller that contains the knockback amount
        private StatController _stats;
        #endregion
        
        /// <summary>
        /// Called when the game loads
        /// Grab any references we need
        /// </summary>
        private void Awake()
        {
            _entity = GetComponent<PhysicsEntity>();
            _stats = GetComponentInParent<StatController>();
        }
        
        /// <summary>
        /// Called when the object gets hit, using the IHitReceiver interface
        /// </summary>
        /// <param name="data"></param>
        public void ReceiveHit(HitData data)
        {
            // if we get knockback from this damage type
            // used to filter what damage types cause knockback
            // e.g. lasers probably shouldn't cause knockback
            if (damageTypes.HasFlag(data.DamageType))
                _entity.AddForce(data.IncomingDirection * KnockbackAmount);
        }
    }
}
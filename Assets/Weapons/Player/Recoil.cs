using Entity;
using Entity.Stats;
using UnityEngine;

namespace Bullets.Player
{
    public class Recoil : BaseShootListener
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields  
        [SerializeField, Tooltip("The physics entity, used for recoil calculations")] 
        private PhysicsEntity entity;

        [SerializeField,
         Tooltip("How much should this object recoil when it fires, as a percentage of the shoot force")]
        private StatType recoilScale;
        private float RecoilScale => _stats.Stats.GetStat(recoilScale);
        
        [SerializeField, Tooltip("Where should the bullet shoot from?")] 
        private Transform shootPoint;
        #endregion

        private StatController _stats;

        protected override void Awake()
        {
            base.Awake();
            _stats = GetComponentInParent<StatController>();
        }

        protected override void OnShoot()
        {
            // shove the entity in the opposite direction
            entity.AddForce(-shootPoint.right * _shoot.GetShootForce() * RecoilScale );
        }
    }
}
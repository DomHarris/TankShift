using Entity;
using UnityEngine;

namespace Bullets.Player
{
    public class Recoil : BaseShootListener
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields  
        [SerializeField, Tooltip("The physics entity, used for recoil calculations")] 
        private PhysicsEntity entity;
        
        [SerializeField, Tooltip("How much should this object recoil when it fires, as a percentage of the shoot force")] 
        private float recoilScale = 0.5f;
        
        [SerializeField, Tooltip("Where should the bullet shoot from?")] 
        private Transform shootPoint;
        #endregion
        
        protected override void OnShoot()
        {
            // shove the entity in the opposite direction
            entity.AddForce(-shootPoint.right * _shoot.GetShootForce() * recoilScale );
        }
    }
}
using JetBrains.Annotations;
using Lean.Pool;
using UnityEngine;

namespace Bullets.Player
{
    /// <summary>
    /// Fire projectile bullets from a point
    /// </summary>
    public class ShootBullet : BaseShootListener
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("The bullet prefab to use. Make sure it has a Rigidbody2D component.")] 
        private Rigidbody2D bulletPrefab;
        
        [SerializeField, Tooltip("Where should the bullet shoot from?")] 
        private Transform shootPoint;
        
        [SerializeField, Tooltip("How quickly should the bullet move, as a multiple of normal speed")] 
        private float speedMultiplier = 2f;
        #endregion
      
       
        /// <summary>
        /// Called when the Shoot event is fired
        /// Fire a bullet
        /// </summary>
        protected override void OnShoot()
        {
            // spawn the bullet with LeanPool
            // LeanPool reuses objects so we don't have to call Instantiate and Destroy as often
            var bullet = LeanPool.Spawn(bulletPrefab, shootPoint.position, shootPoint.rotation);
            
            // a little bit of maths to make the bullets travel faster without changing trajectory 
            // set gravity scale to the speed multiplier squared, then multiply the initial force by the speed multiplier
            bullet.gravityScale = speedMultiplier * speedMultiplier;
            // then multiply the shoot force by the speed multiplier
            bullet.AddForce(_shoot.GetShootForce() * speedMultiplier, ForceMode2D.Impulse);
            
        }
    }
}
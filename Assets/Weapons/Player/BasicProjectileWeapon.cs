using Lean.Pool;
using UnityEngine;

namespace Weapons.Player
{
    [CreateAssetMenu(menuName = "Weapon/Basic Projectile")]
    public class BasicProjectileWeapon : WeaponBase
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("The bullet prefab to use. Make sure it has a Rigidbody2D component.")] 
        private Rigidbody2D bulletPrefab;
        
        [SerializeField, Tooltip("How quickly should the bullet move, as a multiple of normal speed")] 
        private float speedMultiplier = 2f;
        #endregion

        protected override void OnShoot()
        {
            // spawn the bullet with LeanPool
            // LeanPool reuses objects so we don't have to call Instantiate and Destroy as often
            var bullet = LeanPool.Spawn(bulletPrefab, _shootPoint.position, _shootPoint.rotation);
            
            // a little bit of maths to make the bullets travel faster without changing trajectory 
            // set gravity scale to the speed multiplier squared, then multiply the initial force by the speed multiplier
            bullet.gravityScale = speedMultiplier * speedMultiplier;
            // then multiply the shoot force by the speed multiplier
            bullet.AddForce(_shootPoint.right * _input.GetShootForce() * speedMultiplier, ForceMode2D.Impulse);
        }
    }
}
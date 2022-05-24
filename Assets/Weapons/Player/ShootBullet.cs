using DG.Tweening;
using Entity;
using JetBrains.Annotations;
using Lean.Pool;
using UnityEngine;

namespace Bullets.Player
{
    /// <summary>
    /// Fire projectile bullets from a point
    /// </summary>
    [RequireComponent(typeof(BaseShootInput))]
    public class ShootBullet : MonoBehaviour
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("The bullet prefab to use. Make sure it has a Rigidbody2D component.")] 
        private Rigidbody2D bulletPrefab;
        
        [SerializeField, Tooltip("Where should the bullet shoot from?")] 
        private Transform shootPoint;
        
        [SerializeField, Tooltip("How quickly should the bullet move, as a multiple of normal speed")] 
        private float speedMultiplier = 2f;
        
        [SerializeField, Tooltip("A sprite for the muzzle flash"), CanBeNull] 
        private SpriteRenderer muzzleFlash;
        
        [SerializeField, Tooltip("The physics entity, used for knockback calculations")] 
        private PhysicsEntity entity;
        
        [SerializeField, Tooltip("How much should this enemy be knocked back when it fires, as a percentage of the shoot force")] 
        private float knockbackScale = 0.5f;
        #endregion
        
        // Private fields - only used in this script
        #region PrivateFields
        private BaseShootInput _shoot; // the object we're listening to for shoot events
        #endregion
        
        /// <summary>
        /// Called when the game loads
        /// Grab any references we need, and pre-calculate anything we know won't change
        /// </summary>
        private void Awake()
        {
            _shoot = GetComponent<BaseShootInput>();
        }

        /// <summary>
        /// Called when the object is enabled
        /// Start listening to any events
        /// </summary>
        private void OnEnable()
        {
            _shoot.Shoot += OnShoot;
        }

        /// <summary>
        /// Called when the object is disabled
        /// Stop listening to any events
        /// </summary>
        private void OnDisable()
        {
            _shoot.Shoot -= OnShoot;
        }
    
        /// <summary>
        /// Called when the Shoot event is fired
        /// Fire a bullet, with a little muzzle flash
        /// TODO: maybe move the muzzle flash into a different class
        /// </summary>
        private void OnShoot()
        {
            
            // if there's a muzzle flash, do a little DOTween animation to make it black, then white, then fade out
            if (muzzleFlash != null)
            {
                muzzleFlash.color = Color.black;
                muzzleFlash.DOColor(Color.white, 0.1f).SetDelay(0.1f);
                muzzleFlash.DOColor(new Color(1, 1, 1, 0), 0.2f).SetDelay(0.2f);
            }

            // spawn the bullet with LeanPool
            // LeanPool reuses objects so we don't have to call Instantiate and Destroy as often
            var bullet = LeanPool.Spawn(bulletPrefab, shootPoint.position, shootPoint.rotation);
            
            // a little bit of maths to make the bullets travel faster without changing trajectory 
            // set gravity scale to the speed multiplier squared, then multiply the initial force by the speed multiplier
            bullet.gravityScale = speedMultiplier * speedMultiplier;
            // then multiply the shoot force by the speed multiplier
            bullet.AddForce(shootPoint.right * _shoot.GetShootForce() * speedMultiplier, ForceMode2D.Impulse);
            
            // shove the entity in the opposite direction
            entity.AddForce(-shootPoint.right * _shoot.GetShootForce() * knockbackScale );
        }
    }
}
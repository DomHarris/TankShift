using System;
using Entity;
using Entity.Damage;
using Lean.Pool;
using UnityEngine;

namespace Bullets
{
    /// <summary>
    /// When bullets impact something, do a thing!
    /// </summary>
    public class BulletImpact : MonoBehaviour
    {
        #region Events
        public event Action<HitData> Hit;
        #endregion
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("How much damage should this bullet do?")] 
        protected float damage;
        
        [SerializeField, Tooltip("How many times should this bullet bounce?")] 
        protected float numBounces = 1;

        #endregion
        
        // Private fields - only used in this script
        #region PrivateFields
        private Rigidbody2D _rb;
        private int _currentBounces;
        #endregion
        
        /// <summary>
        /// Called when the game loads
        /// Grab any references we need, and pre-calculate anything we know won't change
        /// </summary>
        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void OnEnable()
        {
            _currentBounces = 0;
        }

        /// <summary>
        /// Called every physics tick
        /// Rotate the bullet here so it is always the correct value
        /// Doing it in update causes a bit of stuttering due to the difference in timings between FixedUpdate and Update
        /// </summary>
        protected virtual void FixedUpdate()
        {
            // make the bullet point in the direction of the rigidbody's velocity
            transform.right = _rb.velocity;
        }

        protected void InvokeEvent(HitData damagePacket)
        {
            Hit?.Invoke(damagePacket);
        }

        protected HitData GenerateDamagePacket(Collision2D other)
        {
            // create a little object that holds all the relevant data 
            return new HitData
            {
                CollisionInfo = other,
                Damage = damage,
                DamageType = DamageType.Projectile,
                IncomingDirection = transform.right,
                IncomingObject = gameObject
            };
        }

        protected virtual void DoDamage(HitData damagePacket, IHitReceiver[] receivers)
        {
            // tell all the objects that were hit that they were hit, and send the data
            foreach (var hitReceiver in receivers)
                hitReceiver.ReceiveHit(damagePacket);
        }

        /// <summary>
        /// Called when this object hits something
        /// </summary>
        /// <param name="other">The object that was hit</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            // get all the scripts attached to the thing we hit that want to know about the hit
            var hitReceivers = other.gameObject.GetComponentsInChildren<IHitReceiver>();

            var damagePacket = GenerateDamagePacket(other);
            InvokeEvent(damagePacket);
            DoDamage(damagePacket, hitReceivers);

            ++_currentBounces;
            if(_currentBounces >= numBounces)
                // disable this object
                LeanPool.Despawn(_rb);
        }
    }
}
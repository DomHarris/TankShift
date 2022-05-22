using Lean.Pool;
using UnityEngine;

namespace Bullets
{
    /// <summary>
    /// When bullets impact something, do a thing!
    /// </summary>
    public class BulletImpact : MonoBehaviour
    {
        // Private fields - only used in this script
        #region PrivateFields
        private Rigidbody2D _rb;
        #endregion
        
        /// <summary>
        /// Called when the game loads
        /// Grab any references we need, and pre-calculate anything we know won't change
        /// </summary>
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Called every physics tick
        /// Rotate the bullet here so it is always the correct value
        /// Doing it in update causes a bit of stuttering due to the difference in timings between FixedUpdate and Update
        /// </summary>
        private void FixedUpdate()
        {
            // make the bullet point in the direction of the rigidbody's velocity
            transform.right = _rb.velocity;
        }

        /// <summary>
        /// Called when this object hits something
        /// </summary>
        /// <param name="other">The object that was hit</param>
        public void OnCollisionEnter2D(Collision2D other)
        {
            LeanPool.Despawn(_rb);
        }
    }
}
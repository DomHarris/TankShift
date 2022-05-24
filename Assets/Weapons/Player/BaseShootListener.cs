using UnityEngine;

namespace Bullets.Player
{
    public abstract class BaseShootListener : MonoBehaviour
    {
        // Private fields - only used in this script
        #region PrivateFields
        protected BaseShootInput _shoot; // the object we're listening to for shoot events
        #endregion
        
        /// Called when the game loads
        /// Grab any references we need, and pre-calculate anything we know won't change
        /// </summary>
        protected virtual void Awake()
        {
            _shoot = GetComponent<BaseShootInput>();
        }

        /// <summary>
        /// Called when the object is enabled
        /// Start listening to any events
        /// </summary>
        protected virtual void OnEnable()
        {
            _shoot.Shoot += OnShoot;
        }

        /// <summary>
        /// Called when the object is disabled
        /// Stop listening to any events
        /// </summary>
        protected virtual void OnDisable()
        {
            _shoot.Shoot -= OnShoot;
        }

        protected abstract void OnShoot();
    }
}
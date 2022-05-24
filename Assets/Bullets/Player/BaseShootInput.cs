using System;
using UnityEngine;

namespace Bullets.Player
{
    /// <summary>
    /// Tell an object when to fire a shoot event
    /// </summary>
    public abstract class BaseShootInput : MonoBehaviour
    {
        // Events - broadcast a message to any objects that are listening
        #region Events
        // the event to fire
        public event Action Shoot;
        #endregion
        
        /// <summary>
        /// Invoke the event - used only by base classes
        /// </summary>
        protected void InvokeShootEvent()
        {
            if (Shoot != null)
                Shoot.Invoke();
        }
        
        /// <summary>
        /// Returns how strong the current shot should be
        /// </summary>
        /// <returns>the force of the shot</returns>
        public abstract float GetShootForce();
    }
}
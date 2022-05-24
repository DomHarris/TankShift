using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Damage
{
    /// <summary>
    /// Keep track of health
    /// </summary>
    public class Health : MonoBehaviour, IHitReceiver
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("How much health should we start with?")]
        private float maxHealth;
        #endregion

        // Events - broadcast a message to any objects that are listening
        #region Events
        // the event to broadcast
        public event OnHitFunction OnHit;
        // the function that gets called on the other objects
        public delegate void OnHitFunction(float currentHealth, float previousHealth, float maxHealth,
            float healthPercentage);
        #endregion


        // Private fields - only used in this script
        #region PrivateFields
        private float _currentHealth;
        private float _previousHealth;
        private float _healthPercentage;
        #endregion

        /// <summary>
        /// Called when the object is enabled
        /// Reset the current health
        /// </summary>
        private void OnEnable()
        {
            _previousHealth = _currentHealth = maxHealth;
        }

        /// <summary>
        /// Receive a hit
        /// </summary>
        /// <param name="data"></param>
        public void ReceiveHit(HitData data)
        {
            // keep track of what the health used to be
            _previousHealth = _currentHealth;

            // keep track of what the health currently is 
            _currentHealth -= data.Damage;

            // keep track of the current health as a percentage
            // - doing it here means we only have to do the divide once, and division is a computationally expensive operation
            _healthPercentage = _currentHealth / maxHealth;

            // tell everything that cares "hey, I've been hit"
            OnHit?.Invoke(_currentHealth, _previousHealth, maxHealth, _healthPercentage);
        }
    }
}
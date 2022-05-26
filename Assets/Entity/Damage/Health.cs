using System;
using System.Collections;
using System.Collections.Generic;
using Entity.Stats;
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
        [SerializeField, Tooltip("Which stat type should we use to get the max health?"), StatTypeWithParent]
        private StatType maxHealthStat;
        #endregion
        
        public float MaxHealth => _stats.GetStat(maxHealthStat);

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
        // some private variables to track various health values
        private float _currentHealth;
        private float _previousHealth;
        private float _healthPercentage;
        // the stat controller that contains the max health amount
        private StatController _stats;
        #endregion

        
        /// <summary>
        /// Called when the game loads
        /// Grab any references we need
        /// </summary>
        private void Awake()
        {
            _stats = GetComponentInParent<StatController>();
        }

        /// <summary>
        /// Called when the object is enabled
        /// Reset the current health
        /// </summary>
        private void OnEnable()
        {
            _previousHealth = _currentHealth = MaxHealth;
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
            _healthPercentage = _currentHealth / MaxHealth;

            // tell everything that cares "hey, I've been hit"
            OnHit?.Invoke(_currentHealth, _previousHealth, MaxHealth, _healthPercentage);
        }
    }
}
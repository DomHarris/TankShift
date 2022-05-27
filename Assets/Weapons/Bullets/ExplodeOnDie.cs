using System;
using System.Collections.Generic;
using Entity.Damage;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons.Bullets
{
    /// <summary>
    /// When health drops below 0, explode and disable the object
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class ExplodeOnDie : MonoBehaviour
    {
        // we only ever want to instantiate one particle system, we can reuse that single instantiated one rather than creating a bunch
        // use the already-instantiated one instead of the prefab
        // ParticleSystem[] so we can use any child particle systems too
        private static Dictionary<ParticleSystem, ParticleSystem[]> _particleSystems = new Dictionary<ParticleSystem, ParticleSystem[]>();
        
        [SerializeField, Tooltip("A prefab for the explosion")] 
        private ParticleSystem explosionPrefab;

        [SerializeField, Tooltip("How many particles do we want to spawn for each particle system?")]
        private List<int> particleCount;
        
        [SerializeField, Tooltip("A prefab for the overlay to leave on the ground")] 
        private Transform debris;
        
        private Health _health;

        // <summary>
        /// Called when the game loads
        /// Grab any references we need, and instantiate the particle system we want to instantiate
        /// </summary>
        private void Awake()
        {
            _health = GetComponent<Health>();
            if (!_particleSystems.ContainsKey(explosionPrefab))
                _particleSystems.Add(explosionPrefab, Instantiate(explosionPrefab).GetComponentsInChildren<ParticleSystem>());
        }

        /// <summary>
        /// Called when something changes in the editor
        /// Set the particle count array to the length of the number of particle systems attached to the object
        /// </summary>
        private void OnValidate()
        {
            // get the number of particle systems in the prefab
            var numParticleSystems = explosionPrefab.GetComponentsInChildren<ParticleSystem>().Length;
            
            // loop through whichever is highest, the number of particle systems or the size of the particleCount list
            for (int i = 0; i < Mathf.Max(numParticleSystems, particleCount.Count); ++i)
            {
                // if we're at an index that is higher than the particle count
                // add a new item to the list
                if (i >= particleCount.Count)
                    particleCount.Add(1);
                
                // if we're at an index that is higher than the number of particle systems but lower than the length of the particleCount list
                // get rid of the rest of the items in the particleCount list
                if (i == numParticleSystems && i < particleCount.Count)
                {
                    particleCount.RemoveRange(numParticleSystems-1, particleCount.Count - numParticleSystems - 1);
                    break;
                }
            }
        }

        /// <summary>
        /// Called when the object is enabled
        /// Start listening to any events
        /// </summary>
        private void OnEnable()
        {
            _health.OnHit += OnHit;   
        }

        /// <summary>
        /// When the entity's health changes
        /// </summary>
        /// <param name="currentHealth">the entity's current health</param>
        /// <param name="previousHealth">the entity's health before being hit</param>
        /// <param name="maxHealth">the entity's max health</param>
        /// <param name="healthPercentage">the entity's current health as a percentage (0-1 range)</param>
        private void OnHit(float currentHealth, float previousHealth, float maxHealth, float healthPercentage)
        {
            // if we don't have any health left
            if (currentHealth <= 0)
            {
                // foreach particle system
                for (var i = 0; i < _particleSystems[explosionPrefab].Length; i++)
                {
                    // move the particle system into place
                    _particleSystems[explosionPrefab][i].transform.position = transform.position + (_particleSystems[explosionPrefab][i].transform.parent != null ? _particleSystems[explosionPrefab][i].transform.localPosition : Vector3.zero); 
                    // emit the particles
                    _particleSystems[explosionPrefab][i].Emit(particleCount[i]);
                }

                // spawn a debris object
                LeanPool.Spawn(debris, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360))).localScale = Vector3.one;
                
                // disable this entity
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Called when the object is disabled
        /// Stop listening to any events
        /// </summary>
        private void OnDisable()
        {
            _health.OnHit -= OnHit;
        }
    }
}
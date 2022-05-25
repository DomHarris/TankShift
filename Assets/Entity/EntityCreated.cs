using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    /// <summary>
    /// Enum to describe different 
    /// </summary>
    public enum EntityLabel { Player, Enemy }
    
    /// <summary>
    /// Helper class to reduce the amount of GetComponent calls and GameObject.Find 
    /// </summary>
    public class EntityCreated : MonoBehaviour
    {
        // Public Static events - not tied to a specific object
        public static event Action<EntityLabel, GameObject> EntitySpawn;
        public static event Action<EntityLabel, GameObject> EntityDespawn; 
        
        // Public Static variables - not tied to a specific object
        public static Dictionary<EntityLabel, List<GameObject>> Entities = new Dictionary<EntityLabel, List<GameObject>>();
        
        // Serialized Fields - set in Unity
        [SerializeField] private EntityLabel label;
        
        
        /// <summary>
        /// When the object is enabled
        /// Broadcast the event so all objects that are listening get notified
        /// Add the object to the current list of entities
        /// </summary>
        private void OnEnable()
        {
            if (!Entities.ContainsKey(label))
                Entities.Add(label, new List<GameObject>());
            
            Entities[label].Add(gameObject);
            if (EntitySpawn != null)
                EntitySpawn.Invoke(label, gameObject);
        }

        /// <summary>
        /// When the object is disabled
        /// Broadcast the event so all objects that are listening get notified
        /// Remove the object from the list of entities
        /// </summary>
        private void OnDisable()
        {
            Entities[label].Remove(gameObject);
            
            if (EntityDespawn != null)
                EntityDespawn.Invoke(label, gameObject);
        }
    }
}
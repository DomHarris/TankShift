using System;
using UnityEngine;
using Utils;

namespace Entity.Stats
{
    /// <summary>
    /// Scriptable Object container for stats
    /// Used to describe different values entities can have
    /// Can be updated at runtime or modified with modifiers in a StatController
    /// </summary>
    [CreateAssetMenu(menuName = "Stats/Stat")]
    public class Stat : ScriptableObject
    {
        [SerializeField, ScriptableObjectDropdown(typeof(StatType))] 
        private StatType type;
        public StatType Type => type;
        
        // the runtime value needs to not be serialized by Unity
        // fixes some bugs when testing in the Unity Editor
        [NonSerialized] private float _runtimeValue;
        
        // get the runtime value
        public float Value => _runtimeValue;

        // set an initial value for this object
        [SerializeField] private float initialValue;

        
        /// <summary>
        /// When the game stats, set the runtime value to the initial value 
        /// </summary>
        private void OnEnable()
        {
            _runtimeValue = initialValue;
        }

        /// <summary>
        /// Set the value of this stat
        /// </summary>
        /// <param name="val"></param>
        public void SetValue(float val)
        {
            _runtimeValue = val;
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Set the StatType - only used in the Editor when creating a new Stat from the StatCollection custom editor
        /// We don't want this to happen while the game is running!
        /// </summary>
        /// <param name="type"></param>
        public void SetType(StatType type)
        {
            this.type = type;
        }
        #endif
    }
    
}
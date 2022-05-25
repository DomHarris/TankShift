using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace Entity.Stats
{
    [CreateAssetMenu(menuName = "Stats/Stat")]
    public class Stat : ScriptableObject
    {
        [SerializeField, ScriptableObjectDropdown(typeof(StatType))] private StatType type;
        public StatType Type => type;
        
        [NonSerialized] private float _runtimeValue;
        public float Value => _runtimeValue;

        [SerializeField] private float initialValue;

        private void OnEnable()
        {
            _runtimeValue = initialValue;
        }

        public void SetValue(float val)
        {
            _runtimeValue = val;
        }
        
        #if UNITY_EDITOR
        public void SetType(StatType type)
        {
            this.type = type;
        }
        #endif
    }
    
    [AttributeUsage(AttributeTargets.Field,AllowMultiple=true)]
    public class StatTypeWithParentAttribute : ScriptableObjectDropdownAttribute
    {
        public StatTypeWithParentAttribute() : base(typeof(StatType))
        {
        }
    }
    
    [AttributeUsage(AttributeTargets.Field,AllowMultiple=true)]
    public class ScriptableObjectDropdownAttribute : PropertyAttribute
    {
        public readonly Type Type;

        public ScriptableObjectDropdownAttribute(Type type)
        {
            Type = type;
        }
    }
    
 
 
#if UNITY_EDITOR
    
#endif
}
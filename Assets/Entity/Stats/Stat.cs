using System;
using UnityEngine;

namespace Entity.Stats
{
    [CreateAssetMenu(menuName = "Stats/Stat")]
    public class Stat : ScriptableObject
    {
        [SerializeField] private StatType type;
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
            Debug.Log($"{name}'s value is now {_runtimeValue}");
        }
    }
}
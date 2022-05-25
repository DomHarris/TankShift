using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entity.Stats
{
    [Serializable]
    public class StatModifier
    {
        public StatType Type;
        public float Value;
    }
    
    public class StatController : MonoBehaviour
    {
        [SerializeField] private StatCollection stats;
        private List<StatModifier> _modifiers = new List<StatModifier>();

        public float GetStat(StatType type)
        {
            return stats.GetStat(type) * _modifiers.Where(mod => mod.Type == type).Aggregate(1f, (val, mod) => val * mod.Value);
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void RemoveModifier(StatModifier modifier)
        {
            _modifiers.Remove(modifier);
        }
        
        public void SetStat(StatType type, float newVal)
        {
            stats.SetStat(type, newVal);
        }
    }
}
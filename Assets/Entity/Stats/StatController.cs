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
        [SerializeField] private List<StatModifier> modifiers;

        public float GetStat(StatType type)
        {
            return stats.GetStat(type) * modifiers.Where(mod => mod.Type == type).Aggregate(1f, (val, mod) => val * mod.Value);
        }

        public void AddModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public void RemoveModifier(StatModifier modifier)
        {
            modifiers.Remove(modifier);
        }
        
        public void SetStat(StatType type, float newVal)
        {
            stats.SetStat(type, newVal);
        }
    }
}
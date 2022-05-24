using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entity.Stats
{
    [CreateAssetMenu(menuName = "Stats/Stat Collection")]
    public class StatCollection : ScriptableObject
    {
        [SerializeField] private List<Stat> stats;

        public float GetStat(StatType statType)
        {
            if (stats.Any(s => s.Type == statType))
            {
                var stat = stats.First(s => s.Type == statType);
                if (stat != null)
                    return stat.Value;
                throw new NullReferenceException($"Stat {stat.name} is null.");
            }

            throw new Exception($"No stats of type {statType?.name} in {name}");
        }

        public void SetStat(StatType statType, float newValue)
        {
            if (stats.Any(s => s.Type == statType))
            {
                var stat = stats.First(s => s.Type == statType);
                if (stat != null)
                    stat.SetValue(newValue);
                else
                    throw new NullReferenceException($"Stat {stat.name} is null.");
            } else
                throw new Exception($"No stats of type {statType?.name} in {name}");
        }
    }
}
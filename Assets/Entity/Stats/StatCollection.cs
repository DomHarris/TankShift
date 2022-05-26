using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entity.Stats
{
    /// <summary>
    /// Scriptable object for collections of stats
    /// Used to get and set stats based on the StatType 
    /// </summary>
    [CreateAssetMenu(menuName = "Stats/Stat Collection")]
    public class StatCollection : ScriptableObject
    {
        [SerializeField] private List<Stat> stats;

        /// <summary>
        /// Gets the **first** stat of type statType 
        /// </summary>
        /// <param name="statType">the statType we want to find</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Will throw a null reference exception if the stat of this stat type is null</exception>
        /// <exception cref="Exception">Will throw an exception if there are no stats of type {{statType}} in this collection</exception>
        public float GetStat(StatType statType)
        {
            // if there's a stat of this type
            if (stats.Any(s => s.Type == statType))
            {
                // get the first one
                var stat = stats.First(s => s.Type == statType);
                // if it's not null, return it. Otherwise throw an exception
                if (stat != null)
                    return stat.Value;
                throw new NullReferenceException($"Stat {stat.name} is null.");
            }
            // if there are no stats of this type, throw an exception. This should never happen, exceptions make debugging easier
            throw new Exception($"No stats of type {statType?.name} in {name}");
        }

        /// <summary>
        /// Sets the **first** stat of type statType 
        /// </summary>
        /// <param name="statType">the statType we want to find</param>
        /// <param name="newValue">the value we want to set the stat to</param>
        /// <exception cref="NullReferenceException">Will throw a null reference exception if the stat of this stat type is null</exception>
        /// <exception cref="Exception">Will throw an exception if there are no stats of type {{statType}} in this collection</exception>
        public void SetStat(StatType statType, float newValue)
        {
            // if there's a stat of this type
            if (stats.Any(s => s.Type == statType))
            {
                // get the first one
                var stat = stats.First(s => s.Type == statType);
                // if it's not null, set the value
                if (stat != null)
                    stat.SetValue(newValue);
                else // otherwise throw an exception
                    throw new NullReferenceException($"Stat {stat.name} is null.");
            } else// if there are no stats of this type, throw an exception. This should never happen, exceptions make debugging easier
                throw new Exception($"No stats of type {statType?.name} in {name}");
        }
    }
}
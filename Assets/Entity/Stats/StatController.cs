using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entity.Stats
{
    /// <summary>
    /// Attached to entities that have stats we want to modify during the game
    /// </summary>
    public class StatController : MonoBehaviour
    {
        // the entity's collection of stats
        [SerializeField] private StatCollection stats; 
        
        // a private list of modifiers used to temporarily change the stats
        private List<StatModifier> _modifiers = new List<StatModifier>();

        /// <summary>
        /// Get a stat, with any modifiers
        /// </summary>
        /// <param name="type">The Stat Type to get</param>
        /// <returns></returns>
        public float GetStat(StatType type)
        {
            return stats.GetStat(type) * _modifiers // our list of modifiers 
                                            // only the modifiers that affect this stat type
                                            .Where(mod => mod.Type == type)
                                            // multiply them all together
                                            // e.g. if we had a list of modifiers with values 0.5, 0.25, 1.5, we want to multiply the stat by 0.1875
                                            .Aggregate(1f, (val, mod) => val * mod.Multiplier);
        }

        /// <summary>
        /// Add a modifier to the list. This will automatically update the relevant stats.
        /// </summary>
        /// <param name="modifier"></param>
        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        /// <summary>
        /// Remove a modifier from the list. This will automatically update the relevant stats.
        /// </summary>
        /// <param name="modifier"></param>
        public void RemoveModifier(StatModifier modifier)
        {
            _modifiers.Remove(modifier);
        }
        
        /// <summary>
        /// Directly set the value of a stat.
        /// </summary>
        /// <param name="type">The stat type to modify</param>
        /// <param name="newVal">The new value to set the stat to</param>
        public void SetStat(StatType type, float newVal)
        {
            stats.SetStat(type, newVal);
        }
    }
}
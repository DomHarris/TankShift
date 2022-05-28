using System;
using Utils;

namespace Entity.Stats
{
    /// <summary>
    /// A serializable class use to modify stats of type Type
    /// </summary>
    [Serializable]
    public class StatModifier
    {
        // the StatType to modify
        [ScriptableObjectDropdown(typeof(StatType))]
        public StatType Type;
        // a multiplier for the stat type. 
        public float Multiplier;
    }
}
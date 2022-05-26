using System;
using Utils;

namespace Entity.Stats
{
    /// <summary>
    /// Attribute for stat types that definitely have a parent StatCollection
    /// Used to display and modify the value in the parent StatCollection inline
    /// </summary>
    [AttributeUsage(AttributeTargets.Field,AllowMultiple=true)]
    public class StatTypeWithParentAttribute : ScriptableObjectDropdownAttribute
    {
        /// <summary>
        /// Same as the ScriptableObjectDropdownAttribute, but always with type StatType
        /// </summary>
        public StatTypeWithParentAttribute() : base(typeof(StatType))
        {
        }
    }
}
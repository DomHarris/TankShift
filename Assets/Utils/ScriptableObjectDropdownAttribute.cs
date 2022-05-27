using System;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Attribute for drawing scriptable objects as a dropdown
    /// </summary>
    [AttributeUsage(AttributeTargets.Field,AllowMultiple=true)]
    public class ScriptableObjectDropdownAttribute : PropertyAttribute
    {
        public readonly Type Type;

        /// <summary>
        /// Requires the type of scriptable object to be passed through to the constructor
        /// Without this, we would only be able to find scriptable objects, rather than the specific object type
        /// </summary>
        /// <param name="type">The type to search for. Should be the same as the field, e.g. `[SerializeField, ScriptableObjectDropdown(typeof(Stat))] Stat maxHealth;`</param>
        public ScriptableObjectDropdownAttribute(Type type)
        {
            Type = type;
        }
    }
}
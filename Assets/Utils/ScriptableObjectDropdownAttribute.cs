using System;
using UnityEngine;

namespace Utils
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple=true)]
    public class ScriptableObjectDropdownAttribute : PropertyAttribute
    {
        public readonly Type Type;

        public ScriptableObjectDropdownAttribute(Type type)
        {
            Type = type;
        }
    }
}
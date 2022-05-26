using System;
using UnityEngine;

namespace Utils
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnumFlagsAttribute : PropertyAttribute
    {
        public EnumFlagsAttribute()
        {
        }
    }
}
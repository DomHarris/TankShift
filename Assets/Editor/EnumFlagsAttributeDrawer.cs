using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor
{
    /// <summary>
    /// Property drawer for enum flags
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Called the property is drawn in the Editor
        /// </summary>
        /// <param name="position">The position of the property</param>
        /// <param name="property">The property to modify</param>
        /// <param name="label">The label of the property</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // all we need to do is use a MaskField to draw the current property, rather than an EnumPopup 
            property.intValue = EditorGUI.MaskField( position, label, property.intValue, property.enumNames );
        }
    }
}
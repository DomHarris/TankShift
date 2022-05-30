using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UI
{
    [CustomPropertyDrawer(typeof(IntroCrawl.TextPage))]
    public class TextPageDrawer : PropertyDrawer
    {
        private float _height;
        private Dictionary<TextPageDrawer, bool> _expanded = new Dictionary<TextPageDrawer, bool>();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_expanded.ContainsKey(this)) _expanded.Add(this, false);
            
            _height = 0;
            position.height = EditorGUIUtility.singleLineHeight;

            _expanded[this] = EditorGUI.Foldout(position, _expanded[this], label);
            _height += EditorGUIUtility.singleLineHeight;

            if (_expanded[this])
            {

                var color = property.FindPropertyRelative("Color");
                EditorGUI.PropertyField(position, color);
                _height += EditorGUI.GetPropertyHeight(color);
                position.y += EditorGUI.GetPropertyHeight(color);

                var body = property.FindPropertyRelative("Body");
                EditorGUI.PropertyField(position, body);
                _height += EditorGUI.GetPropertyHeight(body);
                position.y += EditorGUI.GetPropertyHeight(body);

                var breakpoints = property.FindPropertyRelative("BreakPoints");
                EditorGUI.PropertyField(position, breakpoints);
                _height += EditorGUI.GetPropertyHeight(breakpoints);
                position.y += EditorGUI.GetPropertyHeight(breakpoints);

                EditorGUI.BeginDisabledGroup(true);

                int currentPos = 0;
                for (int i = 0; i < breakpoints.arraySize; ++i)
                {
                    var previousValue = i == 0 ? 0 : breakpoints.GetArrayElementAtIndex(i - 1).intValue;
                    currentPos += previousValue;
                    var thisText =
                        body.stringValue.Substring(currentPos, breakpoints.GetArrayElementAtIndex(i).intValue);
                    position.height = EditorGUIUtility.singleLineHeight * 2;
                    EditorGUI.TextArea(position, thisText);
                    _height += EditorGUIUtility.singleLineHeight * 3;
                    position.y += EditorGUIUtility.singleLineHeight * 3;
                }

                EditorGUI.EndDisabledGroup();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _height;
        }
    }
}
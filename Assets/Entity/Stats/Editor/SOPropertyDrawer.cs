using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entity.Stats.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableObjectDropdownAttribute))]
    public class SOPropertyDrawer : PropertyDrawer
    {
        private Type _t;
        private string[] _guids;
        private string[] Guids 
        {
            get 
            {
                if (_guids == null)
                {
                    var allObjects = AssetDatabase.FindAssets("t: " + _t.Name);

                    _guids = allObjects.Where(obj => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath((string)obj)) != null).ToArray();
                }
                return _guids;
            }
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _t = ((ScriptableObjectDropdownAttribute)attribute).Type;

            var list = new List<ScriptableObject>(Guids.Length + 1)
            {
                null
            };

            foreach (var guid in Guids)
            {
                var assetAtPath = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (!list.Contains(assetAtPath))
                    list.Add(assetAtPath);
            }

            var names = list.Select(scriptableObject => scriptableObject == null ? "null" : $"{scriptableObject.name}");

            var current = (ScriptableObject) property.objectReferenceValue;
            
            var currentIdx = list.FindIndex(scriptableObject => scriptableObject == current);

            var prevIdx = currentIdx;
            currentIdx = EditorGUI.Popup(position, label.text, currentIdx, names.ToArray());
            if (currentIdx >= list.Count || currentIdx == -1)
                currentIdx = 0;
            if (prevIdx != currentIdx)
                _guids = null;

            property.objectReferenceValue = list[currentIdx];
        }
    }
}
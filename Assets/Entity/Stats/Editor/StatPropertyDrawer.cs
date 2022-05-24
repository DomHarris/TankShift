using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entity.Stats.Editor
{
    [CustomPropertyDrawer(typeof(Stat), true)]
    public class StatPropertyDrawer : PropertyDrawer
    {
        private string[] _guids;
        private string[] Guids 
        {
            get 
            {
                if (_guids == null)
                {
                    var allObjects = AssetDatabase.FindAssets("t: " + nameof(Stat));

                    _guids = allObjects.Where(obj => AssetDatabase.LoadAssetAtPath<Stat>(AssetDatabase.GUIDToAssetPath((string)obj)) != null).ToArray();
                }
                return _guids;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var list = new List<Stat>(Guids.Length + 1)
            {
                null
            };

            foreach (var guid in Guids)
            {
                var assetAtPath = AssetDatabase.LoadAssetAtPath<Stat>(AssetDatabase.GUIDToAssetPath(guid));
                if (!list.Contains(assetAtPath))
                    list.Add(assetAtPath);
            }

            var names = list.Select(Stat => Stat == null ? "null" : $"{Stat.name}");
            var current = (Stat) property.objectReferenceValue;
            
            const float buttonSize = 60;
            const float spacing = 10;
            position.width -= buttonSize + spacing;
            
            var currentIdx = list.FindIndex(Stat => Stat == current);
            

            currentIdx = EditorGUI.Popup(position, label.text, currentIdx, names.ToArray());
            if (currentIdx >= list.Count || currentIdx == -1)
                currentIdx = 0;
            position.x += position.width + spacing;
            position.width = buttonSize;

            position.height -= 3f;
            
            var serialized = new SerializedObject(current);
            serialized.Update();
            serialized.FindProperty("initialValue").floatValue = EditorGUI.FloatField(position, serialized.FindProperty("initialValue").floatValue);
            serialized.ApplyModifiedProperties();

            
            property.objectReferenceValue = list[currentIdx];
        }
    }
}
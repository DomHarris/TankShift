using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entity.Stats.Editor
{
    [CustomPropertyDrawer(typeof(StatCollection))]
    public class StatCollectionPropertyDrawer : PropertyDrawer
    {
        private string[] _guids;
        private string[] Guids 
        {
            get 
            {
                if (_guids == null)
                {
                    var allObjects = AssetDatabase.FindAssets("t: " + nameof(StatCollection));

                    _guids = allObjects.Where(obj => AssetDatabase.LoadAssetAtPath<StatCollection>(AssetDatabase.GUIDToAssetPath((string)obj)) != null).ToArray();
                }
                return _guids;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var list = new List<StatCollection>(Guids.Length + 1)
            {
                null
            };

            foreach (var guid in Guids)
            {
                var assetAtPath = AssetDatabase.LoadAssetAtPath<StatCollection>(AssetDatabase.GUIDToAssetPath(guid));
                if (!list.Contains(assetAtPath))
                    list.Add(assetAtPath);
            }

            var names = list.Select(StatCollection => StatCollection == null ? "null" : $"{StatCollection.name}");
            var current = (StatCollection) property.objectReferenceValue;
            var currentIdx = list.FindIndex(StatCollection => StatCollection == current);

            const float buttonSize = 100;
            position.width -= buttonSize;
            currentIdx = EditorGUI.Popup(position, label.text, currentIdx, names.ToArray());
            position.x += position.width;
            position.width = buttonSize;
            if (GUI.Button(position, "Find Asset"))
                EditorGUIUtility.PingObject(list[currentIdx]);
            
            if (currentIdx >= list.Count || currentIdx == -1)
                currentIdx = 0;

            property.objectReferenceValue = list[currentIdx];
        }
    }
}
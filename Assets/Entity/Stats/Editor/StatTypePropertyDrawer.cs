using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entity.Stats.Editor
{
    [CustomPropertyDrawer(typeof(StatType))]
    public class StatTypePropertyDrawer : PropertyDrawer
    {
        private string[] _guids;
        private string[] Guids 
        {
            get 
            {
                if (_guids == null)
                {
                    var allObjects = AssetDatabase.FindAssets("t: " + nameof(StatType));

                    _guids = allObjects.Where(obj => AssetDatabase.LoadAssetAtPath<StatType>(AssetDatabase.GUIDToAssetPath(obj)) != null).ToArray();
                }
                return _guids;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var list = new List<StatType>(Guids.Length + 1)
            {
                null
            };

            foreach (var guid in Guids)
            {
                var assetAtPath = AssetDatabase.LoadAssetAtPath<StatType>(AssetDatabase.GUIDToAssetPath(guid));
                if (!list.Contains(assetAtPath))
                    list.Add(assetAtPath);
            }

            var names = list.Select(statType => statType == null ? "null" : $"{statType.name}");
            var current = (StatType) property.objectReferenceValue;
            var currentIdx = list.FindIndex(statType => statType == current);

            currentIdx = EditorGUI.Popup(position, label.text, currentIdx, names.ToArray());
            if (currentIdx >= list.Count || currentIdx == -1)
                currentIdx = 0;

            property.objectReferenceValue = list[currentIdx];
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entity.Stats.Editor
{
    [CustomEditor(typeof(StatCollection))]
    public class StatCollectionEditor : UnityEditor.Editor
    {
        private SerializedProperty _stats;
        private StatType _current = null;
        
        private string[] _guids;
        private string[] Guids 
        {
            get 
            {
                if (_guids == null)
                {
                    var allObjects = AssetDatabase.FindAssets("t: " + nameof(StatType));

                    _guids = allObjects.Where(obj => AssetDatabase.LoadAssetAtPath<StatType>(AssetDatabase.GUIDToAssetPath((string)obj)) != null).ToArray();
                }
                return _guids;
            }
        }

        private void OnEnable()
        {
            _stats = serializedObject.FindProperty("stats");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_stats);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add new stat"))
            {
                if (_current == null) return;
                var newStat = CreateInstance<Stat>();
                newStat.name = $"{target.name}.{_current.name}";
                newStat.SetType(_current);
                AssetDatabase.CreateAsset(newStat, AssetDatabase.GetAssetPath(target).Replace($"{target.name}.asset", $"{newStat.name}.asset"));
                AssetDatabase.Refresh();
                var lastItem = _stats.arraySize;
                _stats.InsertArrayElementAtIndex(lastItem);
                _stats.GetArrayElementAtIndex(lastItem).objectReferenceValue = newStat;
                serializedObject.ApplyModifiedProperties();
            }
            
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

            var currentIdx = list.FindIndex(statType => statType == _current);

            currentIdx = EditorGUILayout.Popup(currentIdx, names.ToArray());
            if (currentIdx >= list.Count || currentIdx == -1)
                currentIdx = 0;

            _current = list[currentIdx];
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
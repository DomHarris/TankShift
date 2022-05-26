using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entity.Stats.Editor
{
    /// <summary>
    /// Custom editor for stat collections
    /// Adds a button to create new stats
    /// </summary>
    [CustomEditor(typeof(StatCollection))]
    public class StatCollectionEditor : UnityEditor.Editor
    {
        // keep a reference to the stats serialized property, so we don't have to call FindProperty multiple times
        private SerializedProperty _stats;
        
        // keep a reference to the currently selected stat type, so we can use that to create a new Stat
        private StatType _current = null;
        
        // cache the GUIDs so we don't have to do multiple lookups every frame
        private string[] _guids;
        
        // get the GUIDs for every StatType scriptable object in the project
        private string[] Guids 
        {
            get 
            {
                // we only want to do this big lookup if we haven't done it already
                if (_guids == null)
                {
                    // find the assets with the correct type
                    var allObjects = AssetDatabase.FindAssets("t: " + nameof(StatType));

                    // filter out all the null StatType objects
                    _guids = allObjects.Where(obj => AssetDatabase.LoadAssetAtPath<StatType>(AssetDatabase.GUIDToAssetPath((string)obj)) != null).ToArray();
                }
                return _guids;
            }
        }
        
        /// <summary>
        /// Called when the domain reloads ("reloading assets" popup window in Unity)
        /// </summary>
        private void OnEnable()
        {
            // cache the reference to the "stats" property 
            _stats = serializedObject.FindProperty("stats");
        }

        /// <summary>
        /// Called every editor tick when we want to draw the editor 
        /// </summary>
        public override void OnInspectorGUI()
        {
            
            // draw the normal property drawer for the stats property
            // we dont' need to change this
            EditorGUILayout.PropertyField(_stats);

            // create a horizontal group, so the button and dropdown are side by side
            EditorGUILayout.BeginHorizontal();
            
            // draw a GUILayout button with some text, and run this if it is pressed 
            if (GUILayout.Button("Add new stat"))
            {
                // don't do anything if the current StatType is null, we don't want to create null stats
                if (_current == null)
                {
                    Debug.LogError("Can't create a stat with no stat type");
                    return;
                }
                
                // create a new Stat instance, and initialise it with the right name and type
                var newStat = CreateInstance<Stat>();
                newStat.name = $"{target.name}.{_current.name}";
                newStat.SetType(_current);
                
                // create an asset object using the current StatCollection object's path - this will put it in the same folder as this asset
                AssetDatabase.CreateAsset(newStat, AssetDatabase.GetAssetPath(target).Replace($"{target.name}.asset", $"{newStat.name}.asset"));
                // immediately refresh the asset database so Unity picks it up right away 
                AssetDatabase.Refresh();
                // insert it into the stats array
                var lastItem = _stats.arraySize;
                _stats.InsertArrayElementAtIndex(lastItem);
                _stats.GetArrayElementAtIndex(lastItem).objectReferenceValue = newStat;
                // update the object so changes are saved
                serializedObject.ApplyModifiedProperties();
            }
            
            // we always want the first in the list to say "null", otherwise we'll autoselect the first one
            // that's undesirable, because it won't throw any errors or warnings but will potentially cause undesirable behaviour
            // making the default option be "null" means if you forget to set this, you'll get a NullReferenceException
            var list = new List<StatType>(Guids.Length + 1)
            {
                null
            };

            // loop through the GUIDs and grab all the StatType assets at the path
            foreach (var guid in Guids)
            {
                var assetAtPath = AssetDatabase.LoadAssetAtPath<StatType>(AssetDatabase.GUIDToAssetPath(guid));
                if (!list.Contains(assetAtPath))
                    list.Add(assetAtPath);
            }
            
            // find all the names so we can use them in the popup
            // TODO: maybe don't use linq for this, it generates a lot of garbage
            var names = list.Select(statType => statType == null ? "null" : $"{statType.name}");

            // get the current stattype reference and find the index of it, so we can display the currently selected one properly
            var currentIdx = list.FindIndex(statType => statType == _current);

            // // display a dropdown list of stat types
            currentIdx = EditorGUILayout.Popup(currentIdx, names.ToArray());
            if (currentIdx >= list.Count || currentIdx == -1)
                currentIdx = 0;

            // store the current StatType object
            _current = list[currentIdx];
            
            EditorGUILayout.EndHorizontal();
            // update the object so changes are saved
            serializedObject.ApplyModifiedProperties();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entity.Stats.Editor
{
    /// <summary>
    /// Custom property drawer for stat collections
    /// Draws differently when used as a property on another object
    /// Displays all the StatCollection objects in a dropdown list, adds a button to find the StatCollection asset, and adds an option to create new StatCollection objects
    /// </summary>
    [CustomPropertyDrawer(typeof(StatCollection))]
    public class StatCollectionPropertyDrawer : PropertyDrawer
    {
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
                    var allObjects = AssetDatabase.FindAssets("t: " + nameof(StatCollection));

                    // filter out all the null StatType objects
                    _guids = allObjects.Where(obj => AssetDatabase.LoadAssetAtPath<StatCollection>(AssetDatabase.GUIDToAssetPath((string)obj)) != null).ToArray();
                }
                return _guids;
            }
        }

        
        /// <summary>
        /// Called every editor tick when we want to draw the property 
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // we always want the first in the list to say "null", otherwise we'll autoselect the first one
            // that's undesirable, because it won't throw any errors or warnings but will potentially cause undesirable behaviour
            // making the default option be "null" means if you forget to set this, you'll get a NullReferenceException
            var list = new List<StatCollection>(Guids.Length + 1);
            
            // loop through the GUIDs and grab all the StatCollection assets at the path
            foreach (var guid in Guids)
            {
                var assetAtPath = AssetDatabase.LoadAssetAtPath<StatCollection>(AssetDatabase.GUIDToAssetPath(guid));
                if (!list.Contains(assetAtPath))
                    list.Add(assetAtPath);
            }
            // add a null object to the end of the list - we'll use this to create a new StatCollection
            // added to the end of the list so we don't try to create a new StatCollection by default
            list.Add(null);
            
            // find all the names so we can use them in the popup
            // TODO: maybe don't use linq for this, it generates a lot of garbage
            var names = list.Select(StatCollection => StatCollection == null ? "Create New StatCollection" : $"{StatCollection.name}");
            var current = (StatCollection) property.objectReferenceValue;
            var currentIdx = list.FindIndex(StatCollection => StatCollection == current);

            // draw a button with width 100 at the same y position 
            // to do this we need to update the `position` variable
            const float buttonSize = 100;
            
            // first, reduce the width so the dropdown list doesn't overlap the button
            position.width -= buttonSize;
            
            // add a dropdown list for the stat collection scriptable objects
            currentIdx = EditorGUI.Popup(position, label.text, currentIdx, names.ToArray());
            
            // move the x position of the position variable, so we can draw the button next to it 
            position.x += position.width;
            // set the width to the button size
            position.width = buttonSize;
            
            // draw a button at `position` with some text
            if (GUI.Button(position, "Find Asset"))
                // when we press the button, ping the object so we can find it
                EditorGUIUtility.PingObject(list[currentIdx]);

            // if we've somehow picked an invalid option, set it back to 0
            if (currentIdx >= list.Count || currentIdx == -1)
                currentIdx = 0;

            property.objectReferenceValue = list[currentIdx];

            // if the current object isn't null, we don't need to do anything else. Return early
            if (list[currentIdx] != null)
                return;

            // if it _is_ null, we've pressed the "create new" option, so lets create a new stat collection
            var newCollection = ScriptableObject.CreateInstance<StatCollection>();
            
            // open the file picker window so the user can choose where to place the object
            var path = EditorUtility
                .SaveFilePanel("Set path for new stat collection", $"{Application.dataPath}{Path.DirectorySeparatorChar}Assets", "New Stat Collection.asset", "asset")
                // this returns an absolute path, e.g. (on macOS) /Users/user/Documents/Tankshift/Assets/NewStatCollection.asset
                // we need everything from "Assets", and need to ignore the first part
                // the first part is, conveniently, Application.dataPath
                .Replace(Application.dataPath, "Assets");

            // if the user clicked cancel, select the first option in the list
            // TODO: make this their previously selected option rather than just the first in the list
            if (path == "")
            {
                property.objectReferenceValue = list.First();
                return;
            }
            
            // create the asset and refresh the asset database
            newCollection.name = path.Split(Path.DirectorySeparatorChar).Last().Replace(".asset", "");
            AssetDatabase.CreateAsset(newCollection, path);
            AssetDatabase.Refresh(); 
            // clear the GUIDs and find them again next time, otherwise the new option won't be part of the dropdown list
            _guids = null;
            // set the property to the newly created object
            property.objectReferenceValue = newCollection;
        }
    }
}
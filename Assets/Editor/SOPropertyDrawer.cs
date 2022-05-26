using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor
{
    /// <summary>
    /// A property drawer that displays Scriptable Objects with the attribute [ScriptableObjectDropdown] as a dropdown
    /// </summary>
    [CustomPropertyDrawer(typeof(ScriptableObjectDropdownAttribute))]
    public class SOPropertyDrawer : PropertyDrawer
    {
        // cache the type from the property
        private Type _t;
        
        // cache the GUIDs so we don't have to do multiple lookups every frame
        private string[] _guids;
        
        // get the GUIDs for every scriptable object of type _t 
        private string[] Guids 
        {
            get 
            {
                // we only want to do this big lookup if we haven't done it already
                if (_guids == null)
                {
                    // find the assets with the correct type
                    var allObjects = AssetDatabase.FindAssets("t: " + _t.Name);

                    // filter out all the null scriptable objects
                    _guids = allObjects.Where(obj => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(obj)) != null).ToArray();
                }
                return _guids;
            }
        }


        /// <summary>
        /// Called the property is drawn in the Editor
        /// </summary>
        /// <param name="position">The position of the property</param>
        /// <param name="property">The property to modify</param>
        /// <param name="label">The label of the property</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // get the "Type" parameter attached to the current attribute
            // set in the constructor for the attribute
            _t = ((ScriptableObjectDropdownAttribute)attribute).Type;

            // we always want the first in the list to say "null", otherwise we'll autoselect the first one
            // that's undesirable, because it won't throw any errors or warnings but will potentially cause undesirable behaviour
            // making the default option be "null" means if you forget to set this, you'll get a NullReferenceException
            var list = new List<ScriptableObject>(Guids.Length + 1)
            {
                null
            };

            // loop through the GUIDs and grab all the ScriptableObject assets at the path
            foreach (var guid in Guids)
            {
                var assetAtPath = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (!list.Contains(assetAtPath))
                    list.Add(assetAtPath);
            }

            // find all the names so we can use them in the popup
            // TODO: maybe don't use linq for this, it generates a lot of garbage
            var names = list.Select(scriptableObject => scriptableObject == null ? "null" : $"{scriptableObject.name}");

            // get the current SO reference and find the index of it, so we can display the currently selected one properly
            var current = (ScriptableObject) property.objectReferenceValue;
            var currentIdx = list.FindIndex(scriptableObject => scriptableObject == current);

            // keep track of what it used to be - if it changes, we should clear the GUIDs
            // this gives us a way to recalculate the GUIDs if a scriptable object was deleted
            var prevIdx = currentIdx;
            
            // display a dropdown list for the scriptable objects 
            currentIdx = EditorGUI.Popup(position, label.text, currentIdx, names.ToArray());
            
            // if we've somehow selected something invalid, set it back to null
            if (currentIdx >= list.Count || currentIdx == -1)
                currentIdx = 0;
            
            // reset the GUIDs if this changes, as described on line 77
            if (prevIdx != currentIdx)
                _guids = null;

            // set the property's value to this object
            property.objectReferenceValue = list[currentIdx];
        }
    }
}
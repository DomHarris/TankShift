using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entity.Stats.Editor
{
    /// <summary>
    /// A property drawer that displays stats as a dropdown, with an editable value
    /// </summary>
    [CustomPropertyDrawer(typeof(Stat), true)]
    public class StatPropertyDrawer : PropertyDrawer
    {
        // cache the GUIDs so we don't have to do multiple lookups every frame
        private string[] _guids;
        
        // get the GUIDs for every Stat 
        private string[] Guids 
        {
            get 
            {
                // we only want to do this big lookup if we haven't done it already
                if (_guids == null)
                {
                    // find the assets with the correct type
                    var allObjects = AssetDatabase.FindAssets("t: " + nameof(Stat));

                    // filter out all the null stats
                    _guids = allObjects.Where(obj => AssetDatabase.LoadAssetAtPath<Stat>(AssetDatabase.GUIDToAssetPath((string)obj)) != null).ToArray();
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
            // we always want the first in the list to say "null", otherwise we'll autoselect the first one
            // that's undesirable, because it won't throw any errors or warnings but will potentially cause undesirable behaviour
            // making the default option be "null" means if you forget to set this, you'll get a NullReferenceException
            var list = new List<Stat>(Guids.Length + 1)
            {
                null
            };

            // loop through the GUIDs and grab all the Stat assets at the path
            foreach (var guid in Guids)
            {
                var assetAtPath = AssetDatabase.LoadAssetAtPath<Stat>(AssetDatabase.GUIDToAssetPath(guid));
                if (!list.Contains(assetAtPath))
                    list.Add(assetAtPath);
            }

            // find all the names so we can use them in the popup
            // TODO: maybe don't use linq for this, it generates a lot of garbage
            var names = list.Select(Stat => Stat == null ? "null" : $"{Stat.name}");
            
            // get the current SO reference and find the index of it, so we can display the currently selected one properly
            var current = (Stat) property.objectReferenceValue;
            var currentIdx = list.FindIndex(Stat => Stat == current);
            
            // add an extra input field to the right of the dropdown with a specific size and spacing 
            const float buttonSize = 60;
            const float spacing = 10;
            
            // we have to manually update the position in OnGUI methods
            position.width -= buttonSize + spacing;

            // draw a popup for the scriptable object
            currentIdx = EditorGUI.Popup(position, label.text, currentIdx, names.ToArray());
            if (currentIdx >= list.Count || currentIdx == -1)
                currentIdx = 0;
            
            // manually update the position
            position.x += position.width + spacing;
            position.width = buttonSize;
            position.height -= 3f;

            if (list[currentIdx] == null)
                return;
            // create a new serialized object for the current property
            // this way, we can find the `initialValue` property more easily
            var serialized = new SerializedObject(list[currentIdx]);
            
            // update the serialized object so we can see the most recent value
            serialized.Update();
            
            // update the the initial value with a FloatField
            serialized.FindProperty("initialValue").floatValue = EditorGUI.FloatField(position, serialized.FindProperty("initialValue").floatValue);
            serialized.ApplyModifiedProperties();
            
            // write the changes to this property
            property.objectReferenceValue = list[currentIdx];
        }
    }
}
using System.Collections;
using System.Reflection;
using Editor;
using UnityEditor;
using UnityEngine;

namespace Entity.Stats.Editor
{
    [CustomPropertyDrawer(typeof(StatTypeWithParentAttribute))]
    public class StatWithParentPropertyDrawer : SOPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = property.serializedObject.targetObject;
 
            const float buttonSize = 60;
            const float spacing = 10;
            position.width -= buttonSize + spacing;
            base.OnGUI(position, property, label);
            
            position.x += position.width + spacing;
            position.width = buttonSize;

            var statController = ((MonoBehaviour)target).GetComponentInParent<StatController>();
            var statControllerSO = new SerializedObject(statController);
            if (statControllerSO.FindProperty("stats").objectReferenceValue == null) return;
            
            var statList = new SerializedObject(statControllerSO.FindProperty("stats").objectReferenceValue).FindProperty("stats");

            SerializedProperty foundStat = null;
            for (int i = 0; i < statList.arraySize; ++i)
            {
                if (((Stat)statList.GetArrayElementAtIndex(i).objectReferenceValue).Type == (StatType)property.objectReferenceValue)
                {
                    foundStat = statList.GetArrayElementAtIndex(i);
                    break;
                }
            }

            if (foundStat == null) return;
            var serialized = new SerializedObject(foundStat.objectReferenceValue);
            serialized.Update();
            serialized.FindProperty("initialValue").floatValue = EditorGUI.FloatField(position, serialized.FindProperty("initialValue").floatValue);
            serialized.ApplyModifiedProperties();
        }
 
    }
}
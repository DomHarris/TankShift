using UnityEditor;
using UnityEditor.UI;

namespace UI
{
    [CustomEditor(typeof(WeaponSlot)), CanEditMultipleObjects]
    public class WeaponSlotEditor : ButtonEditor
    {
        private SerializedProperty _image;
        private SerializedProperty _defaultSprite;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _image = serializedObject.FindProperty("image");
            _defaultSprite = serializedObject.FindProperty("defaultSprite");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_image);
            EditorGUILayout.PropertyField(_defaultSprite);
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
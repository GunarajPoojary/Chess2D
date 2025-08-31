using Chess2D.UI;
using UnityEditor;
using UnityEditor.UI;

namespace Chess2D.Editor
{
    [CustomEditor(typeof(TweenUIElement))]
    public class TweenButtonEditor : ButtonEditor
    {
        private SerializedProperty _buttonTweenConfigProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _buttonTweenConfigProperty = serializedObject.FindProperty("_buttonTweenConfig");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Custom Button Tween Animation", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_buttonTweenConfigProperty, new UnityEngine.GUIContent("Button Tween Config"));

            // Apply changes
            serializedObject.ApplyModifiedProperties();
        }
    }
}
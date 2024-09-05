using UnityEditor;
using UnityEditor.UI;


[CustomEditor(typeof(CustomToggle), true)]
public class CustomToggleEditor : ToggleEditor
{
    SerializedProperty _EventToggleOnProperty;
    SerializedProperty _EventToggleOffProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        _EventToggleOnProperty = serializedObject.FindProperty("EventToggleOn");
        _EventToggleOffProperty = serializedObject.FindProperty("EventToggleOff");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(_EventToggleOnProperty);
        EditorGUILayout.PropertyField(_EventToggleOffProperty);

        serializedObject.ApplyModifiedProperties();
    }
}

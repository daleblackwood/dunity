using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Entity), true)]
public class EntityEditor : Editor
{
	public override void OnInspectorGUI()
    {
        var entity = (Entity)target;

        DrawDefaultInspector();

        GUI.enabled = false;
        EditorGUILayout.LabelField("Entity Key", entity.GetKey());
        GUI.enabled = true;
    }
}

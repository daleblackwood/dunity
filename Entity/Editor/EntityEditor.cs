using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Entity), true)]
public class EntityEditor : Editor
{
	public override void OnInspectorGUI()
    {
        var entity = (Entity)target;

        if (entity.saveKey != null)
        {
            EditorGUILayout.LabelField("Entity Key: " + entity.saveKey);
        }
        else
        {
            EditorGUILayout.LabelField("No entity key yet.");
        }

        DrawDefaultInspector();

        //GUI.enabled = false;
        //GUI.enabled = true;
    }
}

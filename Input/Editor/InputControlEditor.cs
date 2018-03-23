using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InputControl))]
public class InputControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var ic = (InputControl)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Debug Assign"))
        {
            ic.DebugAssign();
        }
    }
}

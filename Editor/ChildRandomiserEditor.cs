using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChildRandomiser))]
public class ChildRandomiserEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cr = (ChildRandomiser)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Randomise"))
        {
            cr.Randomise();
        }
    }
}

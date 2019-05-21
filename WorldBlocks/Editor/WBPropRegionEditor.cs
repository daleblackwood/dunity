using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WBPropRegion))]
public class WBPropRegionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUI.changed)
        {
            ((WBPropRegion)target).SetDirty();
        }
    }
}
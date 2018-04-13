using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using WorldBlocks;

[CustomEditor(typeof(WorldBlockInverted))]
public class WorldBlockInvertedEditor : WorldBlockEditor 
{
    [MenuItem("WorldBlocks/Create/Inverted")]
    [MenuItem("GameObject/WorldBlocks/Inverted", false, 1)]
    [MenuItem("Heirachy/Create/WorldBlocks/Inverted")]
    static void MenuCreate()
    {
        var wb = WorldBlockInverted.CreateInverted();
        WBEditor.PostCreate(wb);
    }

    public override void OnInspectorGUI()
    {
        var wp = (WorldBlockInverted)target;

        if (WBEditorLayout.InspectorDrawer("WorldBlockInverted"))
        {
            wp.m_borderSize = EditorGUILayout.Slider("Border", wp.m_borderSize, 1.0f, 50.0f);
        }
        WBEditorLayout.EndDrawer();

        base.OnInspectorGUI();
    }
}

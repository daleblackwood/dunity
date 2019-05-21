using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using WorldBlocks;

[CustomEditor(typeof(WorldPath))]
public class WorldPathEditor : WorldBlockEditor 
{
    [MenuItem("WorldBlocks/Create/Path")]
    [MenuItem("GameObject/WorldBlocks/Path", false, 1)]
    [MenuItem("Heirachy/Create/WorldBlocks/Path")]
    static void MenuCreatePath()
    {
        var wb = WorldPath.CreatePath();
        WBEditor.PostCreate(wb);
    }

    public override void OnInspectorGUI()
    {
        var wp = (WorldPath)target;

        if (WBEditorLayout.InspectorDrawer("WorldPath"))
        {
            wp.m_pathWidth = EditorGUILayout.Slider("Width", wp.m_pathWidth, 0.5f, 4.0f);
        }
        WBEditorLayout.EndDrawer();

        base.OnInspectorGUI();
    }

    /*
    protected override void InspectPoint(WorldBlock wb, int index)
    {
        var wp = (WorldPath)wb;

        if (WBEditorLayout.InspectorDrawer("Point"))
        {
            index = Mathf.Clamp(index, 0, wb.m_edgeLoop.m_points.Count - 1);
            var p = wb.m_edgeLoop.m_points[index];
            p = EditorGUILayout.Vector2Field("Position", p);
            wb.m_edgeLoop.m_points[index] = p;

            float pointWidth = 1.0f;
            if (wp.m_pointWidths.ContainsKey(index))
            {
                pointWidth = wp.m_pointWidths[index];
            }
            float newPointWidth = EditorGUILayout.Slider("Segment Width", pointWidth, 0.3f, 3.0f);
            if (Mathf.Abs(pointWidth - newPointWidth) > float.Epsilon)
            {
                wp.m_pointWidths[index] = newPointWidth;
            }
        }
        WBEditorLayout.EndDrawer();
        GUILayout.Space(5);
    }
     */

}

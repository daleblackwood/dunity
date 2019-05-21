using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Game), true)]
public class GameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var game = (Game)target;

        EditorGUILayout.LabelField("Mode", game.gameMode.GetType().ToString());
        EditorGUILayout.LabelField("Save Slot", SaveData.main.slot.ToString());

        DrawDefaultInspector();
    }
}
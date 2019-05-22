using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Game), true)]
public class GameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var game = (Game)target;

        var modeName = game.gameMode == null ? "(none)" : game.gameMode.GetType().Name;
        var saveSlot = SaveData.main == null ? "(none)" : SaveData.main.slot.ToString();
        EditorGUILayout.LabelField("Mode", modeName);
        EditorGUILayout.LabelField("Save Slot", saveSlot);

        DrawDefaultInspector();
    }
}
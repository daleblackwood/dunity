using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(GameStage), true)]
public class GameStageEditor : Editor
{
    bool showSaveData = false;

	public override void OnInspectorGUI()
    {
        var stage = (GameStage)target;

        stage.saveKey = EditorSceneManager.GetActiveScene().name;

        showSaveData = EditorGUILayout.Foldout(showSaveData, stage.saveKey + ": Save Data");
        if (showSaveData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var stageData = stage.saveData;
            if (stageData != null && stageData.entities.Count > 0)
            {
                EditorGUIUtility.labelWidth = 150;
                foreach (var pair in stageData.entities)
                {
                    EditorGUILayout.LabelField(pair.Key, pair.Value.ToString());
                }
                EditorGUIUtility.labelWidth = 0;
            }
            else
            {
                EditorGUILayout.HelpBox("No stage data yet exists. Save to write.", MessageType.Warning);
            }

            if (GUILayout.Button("Save Now"))
            {
                GameStageProcessor.SaveScenes();
            }

            EditorGUILayout.EndVertical();
        }


        DrawDefaultInspector();
    }
}

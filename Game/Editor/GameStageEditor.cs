using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using SimpleJSON;

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
            if (stageData == null || stageData.data.Count < 1)
            {
                EditorGUIUtility.labelWidth = 0;
                EditorGUIUtility.labelWidth = 150;
                foreach (KeyValuePair<string, JSONNode> pair in stageData.data)
                {
                    EditorGUILayout.LabelField(pair.Key, pair.Value.ToString());
                }
                EditorGUIUtility.labelWidth = 0;
            }
            else
            {
                EditorGUILayout.HelpBox("No stage data yet exists.", MessageType.Warning);
            }

            if (GUILayout.Button("Update Now"))
            {
                GameStageProcessor.SaveScenes();
            }

            EditorGUILayout.EndVertical();
        }


        DrawDefaultInspector();
    }
}

using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class GameStageProcessor : UnityEditor.AssetModificationProcessor
{
    static string[] OnWillSaveAssets(string[] paths)
    {
        bool shouldSaveScenes = false;
        foreach (string path in paths)
        {
            if (path.ToLower().EndsWith(".unity"))
            {
                shouldSaveScenes = true;
            }
        }
        if (shouldSaveScenes)
        {
            SaveScenes();
        }
        return paths;
    }

    static public void SaveScenes()
    {
        for (int i=0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            SaveScene(scene);
        }
    }

    static void SaveScene(Scene scene)
    {
        GameStage stage = null;
        var entities = new List<Entity>();
        foreach (var go in scene.GetRootGameObjects())
        {
            if (stage == null)
            {
                stage = go.GetComponent<GameStage>();
            }
            CollectEntities(go.transform, entities);
        }

        if (stage == null)
        {
            Debug.LogWarning("No stage on scene " + scene.name);
            return;
        }

        var stageSo = new SerializedObject(stage);
        var stageSaveKey = stageSo.FindProperty("saveKey");
        stageSaveKey.stringValue = scene.name;
        Debug.Log("Writing stage " + stageSaveKey.stringValue + "...");

        var writtenKeys = new Dictionary<string, bool>();

        foreach (var entity in entities)
        {
            var entitySo = new SerializedObject(entity);
            var entitySaveKey = entitySo.FindProperty("saveKey");
            int index = 0;
            do 
            {
                entitySaveKey.stringValue = EvalUtils.ParseToken(entity.name);
                if (index++ > 0)
                {
                    entitySaveKey.stringValue += "_" + index.ToString();
                }
            }
            while (writtenKeys.ContainsKey(entitySaveKey.stringValue));
            writtenKeys[entitySaveKey.stringValue] = true;
            entitySo.ApplyModifiedPropertiesWithoutUndo();
        }

        stageSo.ApplyModifiedPropertiesWithoutUndo();

        AssetDatabase.Refresh();

        Debug.Log("Stage " + stageSaveKey.stringValue + " written.");
    }

    static void CollectEntities(Transform parent, List<Entity> entities)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            CollectEntities(child, entities);

            var entity = child.GetComponent<Entity>();
            if (entity == null)
                continue;

            entities.Add(entity);
        }
    }
}
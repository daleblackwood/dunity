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

        stage.saveKey = scene.name;
        Debug.Log("Writing stage " + stage.saveKey + "...");

        stage.saveData = new SaveData.StageData();
        foreach (var entity in entities)
        {
            int index = 0;
            do 
            {
                entity.saveKey = SaveData.ParseSaveKey(entity.name);
                if (index++ > 0)
                {
                    entity.saveKey += "_" + index.ToString();
                }
            }
            while (stage.saveData.entities.ContainsKey(entity.saveKey));
            stage.saveData.SetEntity(entity);
        }

        var path = SaveData.GetStageDefaultDataPath(stage);
        var json = stage.saveData.Serialise();
        var str = json.ToString();
        Debug.Log("Save: " + path + "\n" + str);
        SaveData.TextWrite(path, str);
        AssetDatabase.Refresh();
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
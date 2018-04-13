using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorTools 
{
    [MenuItem("GameObject/Create New Parent", false, 1)]
    static void CreateParent()
    {
        if (Selection.gameObjects.Length < 1)
        {
            Debug.LogError("Nothing to parent.");
            return;
        }

        var first = Selection.gameObjects[0];

        var name = Selection.gameObjects[0].name + "Parent";
        var go = new GameObject(name);
        go.transform.SetParent(first.transform.parent);
        go.transform.position = first.transform.position;

        var children = new Transform[Selection.gameObjects.Length];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = Selection.gameObjects[i].transform;
        }

        AddToParent(go.transform, children);

        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Add To Parent", false, 1)]
    static void AddToParent()
    {
        if (Selection.gameObjects.Length < 2)
        {
            Debug.LogError("Not enough to parent.");
            return;
        }

        var children = new Transform[Selection.gameObjects.Length - 1];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = Selection.gameObjects[i + 1].transform;
        }

        AddToParent(Selection.gameObjects[0].transform, children);
    }

    [MenuItem("GameObject/Create New Adjacent", false, 1)]
    static void CreateAdjacent()
    {
        if (Selection.gameObjects.Length < 1)
        {
            Debug.LogError("Nothing for adjacent.");
            return;
        }

        var selected = Selection.gameObjects[0];
        var go = new GameObject();
        go.transform.SetParent(selected.transform.parent, false);
        go.transform.position = selected.transform.position;
        go.transform.SetSiblingIndex(selected.transform.GetSiblingIndex() + 1);
        Selection.activeObject = go;
    }

    [MenuItem("Tools/Run Eval Tests")]
    static void RunEvalTests()
    {
        EvalUtils.RunTests();
    }

    static void AddToParent(Transform parent, Transform[] newChildren)
    {
        foreach (var child in newChildren)
        {
            child.SetParent(parent, true);
        }
    }

    [MenuItem("Tools/Erase Saves")]
    static void EraseSaves()
    {
        for (int i = 0; i < 10; i++)
        {
            SaveData.SaveGameData(i, new SaveData.GameData());
        }
    }
}

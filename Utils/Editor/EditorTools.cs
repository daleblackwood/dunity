using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorTools 
{
    [MenuItem("Tools/Create New Parent")]
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

    [MenuItem("Tools/Add To Parent")]
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

    static void AddToParent(Transform parent, Transform[] newChildren)
    {
        foreach (var child in newChildren)
        {
            child.SetParent(parent, true);
        }
    }
}

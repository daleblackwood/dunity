using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

public class Utils 
{
    static public T Require<T>(GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }
        return (T)comp;
    }

    static public Transform GenerateChild(Transform parent, string name = "GenChild")
    {
        if (parent != null)
        {
            var removing = parent.Find(name);
            if (removing != null)
            {
                Remove(removing.gameObject);
            }
        }
        var child = new GameObject(name).transform;
        child.SetParent(parent, false);
        return child;
    }

    static public GameObject CloneInPlace(GameObject original)
    {
        var clone = GameObject.Instantiate(original, 
            original.transform.position, 
            original.transform.rotation, 
            original.transform.parent
        );
        return (GameObject)clone;
    }

    static public T CloneInPlace<T>(T original) where T : Component
    {
        GameObject clone = (GameObject)GameObject.Instantiate(original.gameObject, 
            original.transform.position, 
            original.transform.rotation, 
            original.transform.parent
        );
        return clone.GetComponent<T>();
    }

    public static T NewComponentObject<T>(Transform parent, string name = null) where T : Component
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        return go.AddComponent<T>();
    }

    public static GameObject InstantiateInto(GameObject prefab, Transform parent, string name = null)
    {
        var instance = GameObject.Instantiate(prefab.gameObject);
        instance.transform.SetParent(parent, false);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        if (string.IsNullOrEmpty(name) == false)
        {
            instance.name = name;
        }
        else
        {
            instance.name = prefab.name + "Instance";
        }

        instance.SetActive(true);

        return instance;
    }

    public static T InstantiateInto<T>(T prefab, Transform parent, string name = null) where T : Component
    {
        return InstantiateInto(prefab.gameObject, parent, name).GetComponent<T>();
    }

    static public void Remove(GameObject gameObject, bool immediate = false)
    {
        gameObject.name += "_destroy";
        if (Application.isPlaying && immediate == false)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            GameObject.DestroyImmediate(gameObject);
        }
    }

    static public void Remove(Component component, bool immediate = false)
    {
        Remove(component.gameObject, immediate);
    }

    static public void RemoveComponent<T>(GameObject gameObject) where T : Component
    {
        var comp = gameObject.GetComponent<T>();
        if (comp != null)
        {
            GameObject.Destroy(comp);
        }
    }

    static public void Replace(GameObject original, GameObject target, bool remove)
    {
        target.transform.parent = original.transform.parent;
        target.transform.localPosition = original.transform.localPosition;
        target.transform.localRotation = original.transform.localRotation;
        target.transform.localScale = original.transform.localScale;
        target.transform.SetSiblingIndex(original.transform.GetSiblingIndex());

        if (remove)
        {
            Remove(original, false);
        }
        else
        {
            original.SetActive(false);
        }
    }

    static public string GetPath(GameObject gameObject)
    {
        string result = gameObject.name;

        var parent = gameObject.transform.parent;
        while (parent != null)
        {
            result = parent.name + "." + result;
            parent = parent.parent;
        }

        return result;
    }

    static public bool IsAncestor(Transform parent, Transform child)
    {
        if (parent == null || child == null)
            return false;

        var t = child;
        while (t != null)
        {
            if (t == parent)
                return true;
            
            t = t.parent;
        }

        return false;
    }

    static public T[] GetChildComponents<T>(Transform parent) where T : Component
    {
        var list = new List<T>();
        for (int i=0; i<parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            var childComp = child.GetComponent<T>();
            if (childComp != null)
            {
                list.Add(childComp);
            }
        }
        return list.ToArray();
    }

    static public T GetClosest<T>(T[] components, Vector3 position) where T : Component
    {
        if (components == null || components.Length < 1)
            return null;

        T closest = null;
        float distSq = float.PositiveInfinity;

        foreach (var comp in components)
        {
            if (comp == null)
                continue;

            float dsq = (position - comp.transform.position).sqrMagnitude;
            if (dsq > distSq)
                continue;

            closest = comp;
            distSq = dsq;
        }

        return closest;
    }

    static public T GetClosestOf<T>(T a, T b, Vector3 position) where T : Component
    {
        return (position - a.transform.position).sqrMagnitude > (position - b.transform.position).sqrMagnitude ? b : a;
    }

    static public Vector3 GetClosestOf(Vector3 a, Vector3 b, Vector3 position)
    {
        return (position - a).sqrMagnitude > (position - b).sqrMagnitude ? b : a;
    }

    static public bool WithinDistance(Vector3 a, Vector3 b, float distance)
    {
        return (b - a).sqrMagnitude < distance * distance;
    }

    static public void SetLayer(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            SetLayer(child.gameObject, layer);
        }
    }
}

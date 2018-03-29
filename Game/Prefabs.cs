using UnityEngine;
using System.Collections.Generic;

public class Prefabs : MonoBehaviour
{
    static public Prefabs main = null;

    public GameObject[] prefabs;
    Dictionary<string, GameObject> prefabRefs = new Dictionary<string, UnityEngine.GameObject>();

    void OnEnable()
    {
        main = this;
        prefabRefs.Clear();
        for (int i=0; i<prefabs.Length; i++)
        {
            var prefab = prefabs[i];
            prefabRefs[GetKey(prefab.name)] = prefab;
        }
    }

    public T GetPrefab<T>(string name) where T : Component
    {
        var key = GetKey(name);
        if (prefabRefs.ContainsKey(key) == false)
            return null;

        var go = prefabRefs[key];
        return go.GetComponent<T>();
    }

    public T CreateInstance<T>(string name, bool disable = false) where T : Component
    {
        var prefab = GetPrefab<T>(name);
        if (prefab == null)
            return null;

        var instance = Instantiate(prefab);
        if (disable)
        {
            instance.gameObject.SetActive(false);
        }

        return instance;
    }

    string GetKey(string key)
    {
        return key.Trim().ToLower();
    }
}

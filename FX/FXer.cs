using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FXType
{
    None, 
    Explosion,
    LandingCloudBig,
    DustCloud,
    SmokePuff,
    Foam,
    //Dust,
    //Coin,
    //GoldDrop,
    //Bullet,
    //ExplosionFloor,
    //BigDust,
    EnemyExplode,
    HitSmack,
    MAX
}

[System.Serializable]
public class FXEntry
{
    public FXType fxType;
    public FXInstance prefab;
    public int limit = 1;
    public int next = 0;
}

public class FXer : MonoBehaviour 
{
    static public FXer main { get; private set; }

    public FXEntry[] entries;

    Dictionary<FXType, FXInstance[]> instances = new Dictionary<FXType, FXInstance[]>();
    public Transform parent { get; private set; }

    void Awake()
    {
        main = this;

        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        Clear();
    }

    public void Clear()
    {
        parent = Utils.GenerateChild(null, "FXParent");
    }

    public FXInstance Fire(FXType fxType, Vector3 positon, float scale = 1.0f)
    {
        var instance = NextInstance(fxType);
        instance.transform.SetParent(parent);
        instance.transform.position = positon;
        instance.transform.localScale = Vector3.one * scale;
        instance.transform.localRotation = Quaternion.identity;
        instance.gameObject.SetActive(true);
        instance.CallFire();
        return instance;
    }

    FXInstance NextInstance(FXType fxType)
    {
        var entry = GetEntry(fxType);
        if (entry == null || entry.limit < 1)
            return null;

        if (instances.ContainsKey(fxType) == false)
        {
            instances[fxType] = new FXInstance[entry.limit];
        }

        var collection = instances[fxType];
        if (entry.next < 0)
            entry.next = 0;

        int index = entry.next % entry.limit;
        if (collection[index] == null)
        {
            collection[index] = MakeInstance(fxType);
        }

        entry.next++;
        return collection[index];
    }

    FXInstance MakeInstance(FXType fxType)
    {
        var entry = GetEntry(fxType);
        if (entry == null)
            return null;

        var instance = GameObject.Instantiate(entry.prefab);
        return instance;
    }

    public FXEntry GetEntry(FXType fxType)
    {
        foreach (var entry in entries)
        {
            if (entry.fxType == fxType)
                return entry;
        }
        return null;
    }
}
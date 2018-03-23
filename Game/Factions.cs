using UnityEngine;
using System.Collections.Generic;

public class Factions : MonoBehaviour
{
    static private Factions _main;
    static public Factions main { 
        get {
            if (_main == null)
            {
                _main = GameObject.FindObjectOfType<Factions>();
                if (_main == null)
                {
                    var mainGo = new GameObject("Factions");
                    _main = mainGo.AddComponent<Factions>();
                }
            }
            return _main;
        }
    }

    public Faction[] factions;


    void Awake()
    {
        _main = this;
        if (factions == null || factions.Length < 1)
        {
            
        }
        GameObject.DontDestroyOnLoad(gameObject);
    }

    public Faction GetFaction(int index)
    {
        if (index < 0 || index >= factions.Length)
            return null;

        return factions[index];
    }

    public int GetFactionIndex(Faction faction)
    {
        for (int i=0; i<factions.Length; i++)
        {
            if (factions[i].name == faction.name)
                return i;
        }
        return -1;
    }

    public Faction[] GetOpposing(int index)
    {
        var result = new List<Faction>();
        for (int i=0; i<factions.Length; i++)
        {
            if (i == index)
                continue;

            result.Add(factions[i]);
        }
        return result.ToArray();
    }
}

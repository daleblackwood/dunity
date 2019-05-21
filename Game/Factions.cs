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
        GameObject.DontDestroyOnLoad(gameObject);
    }

    public Faction GetFaction(int index)
    {
        if (factions == null)
            return null;

        if (index < 0 || index >= factions.Length)
            return null;

        return factions[index];
    }
    
    public Faction GetFaction(string factionName)
    {
        var index = GetFactionIndex(factionName);
        return GetFaction(index);
    }

    public int GetFactionIndex(string factionName)
    {
        for (int i=0; i<factions.Length; i++)
        {
            if (factions[i].Compare(factionName))
                return i;
        }
        return -1;
    }
    
    public int GetFactionIndex(Faction faction)
    {
        return GetFactionIndex(faction.name);
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

    public Faction[] GetOpposing(string factionName)
    {
        var index = GetFactionIndex(factionName);
        return GetOpposing(index);
    }

    public Faction[] GetOpposing(Faction faction)
    {
        var index = GetFactionIndex(faction);
        return GetOpposing(index);
    }
}

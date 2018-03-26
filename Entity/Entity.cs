using UnityEngine;
using System.Collections;

public class Entity : GameBehaviour
{
    public bool isTargetable = false;
    string key = null;

    public string GetKey()
    {
        if (key == null)
        {
            key = (GetType().Name + "-" + name).ToLower();
        }
        return key;
    }
}

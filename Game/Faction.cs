using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Faction", menuName = "Dwil/Create Faction")]
public class Faction : ScriptableObject
{
    public Color color = new Color(0.3f, 0.6f, 1);

    public float hue {
        get {
            var hsv = Vector3.zero;
            Color.RGBToHSV(color, out hsv.x, out hsv.y, out hsv.z);
            return hsv.x;
        }
    }

    public override int GetHashCode()
    {
        return GetKey().GetHashCode();
    }

    public string GetKey()
    {
        return ToKey(name);
    }

    public bool Compare(string otherName)
    {
        return ToKey(name) == ToKey(otherName);
    }

    string ToKey(string s)
    {
        if (string.IsNullOrEmpty(s))
            return "";

        return s.ToLower().Trim();
    }
}
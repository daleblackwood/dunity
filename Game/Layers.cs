using UnityEngine;

public static class Layers
{
    static public int GetCastMask()
    {
        return ~(
            1 << LayerMask.NameToLayer("Water") &
            1 << LayerMask.NameToLayer("ActorIgnore")
        );
    }

    static public readonly int CastMask = GetCastMask();
    static public readonly int OneWay = LayerMask.NameToLayer("OneWay");
    static public readonly int Water = LayerMask.NameToLayer("Water");
    static public readonly int Actor = LayerMask.NameToLayer("Actor");
    static public readonly int Hurts = LayerMask.NameToLayer("Hurts");
    static public readonly int Kills = LayerMask.NameToLayer("Kills");
}
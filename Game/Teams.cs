using UnityEngine;
using System.Collections;

public enum Team
{
    A,
    B,
    MAX
}

public class Teams
{
    static public Team Opposite(Team team)
    {
        return team == Team.A ? Team.B : Team.A;
    }

    static public Color GetColor(Team team)
    {
        return Color.HSVToRGB(GetHue(team), 0.7f, 0.7f);
    }

    static public float GetHue(Team team)
    {
        return team == Team.A ? 0.5f : 0.0f;
    }

    static public float GetClothesValue(Team team)
    {
        return team == Team.A ? 0.0f : 0.5f;
    }

    static public string GetName(Team team)
    {
        return team == Team.A ? "Blues": "Reds";
    }

    static public Team GetTeamForPlayer(int index)
    {
        return index % 2 == 0 ? Team.A : Team.B;
    }
}

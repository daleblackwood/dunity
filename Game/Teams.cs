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
        return GetFactionForTeam(team).color;
    }

    static public float GetHue(Team team)
    {
        return GetFactionForTeam(team).hue;
    }

    static public string GetName(Team team)
    {
        return GetFactionForTeam(team).name;
    }

    static public Team GetTeamForPlayer(int index)
    {
        return index % 2 == 0 ? Team.A : Team.B;
    }

    static public Faction GetFactionForTeam(Team team)
    {
        return Factions.main.GetFaction((int)team);
    }

    static public Faction GetFactionForPlayer(int index)
    {
        return GetFactionForTeam(GetTeamForPlayer(index));
    }
}

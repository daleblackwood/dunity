using UnityEngine;
using System.Collections.Generic;

public enum ActionType
{
    None = 0,
    Jump,
    Attack,
    Action3,
    Action4,
    Confirm = 10,
    Escape,
    MAX
}

[System.Serializable]
public class Actions
{
    public Vector2 move;

    public bool primary { 
        get { return IsSet(ActionType.Jump); } 
        set { Set(ActionType.Jump, value); } 
    }

    public bool secondary { 
        get { return IsSet(ActionType.Attack); } 
        set { Set(ActionType.Attack, value); } 
    }

    bool[] active = new bool[(int)ActionType.MAX];

    public Actions()
    {
        Reset();
    }

    public void Reset()
    {
        move = Vector2.zero;
        for (int i = 0; i < active.Length; i++)
        {
            active[i] = false;
        }
    }

    public bool IsSet(ActionType actionType)
    {
        return active[(int)actionType] == true;
    }

    public void Set(ActionType actionType, bool value)
    {
        active[(int)actionType] = actionType > ActionType.None && actionType < ActionType.MAX ? value : false;
    }
}
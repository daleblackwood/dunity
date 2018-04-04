using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContextAction
{
    None,

    // strike
    Use,
    Open,
    Talk,
    Read
}

public class ContextActionTrigger : BaseTrigger 
{
    public ContextAction action = ContextAction.Use;

    public bool IsStrike()
    {
        return action >= ContextAction.Use && action <= ContextAction.Read;
    }

    public override void Trigger()
    {
        triggerable.FireTrigger();
    }
}

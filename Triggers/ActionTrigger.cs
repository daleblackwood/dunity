using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerAction
{
    None,

    // strike
    Use,
    Open,
    Talk,
    Read
}

public class ActionTrigger : BaseTrigger 
{
    public TriggerAction action = TriggerAction.Use;

    public bool IsStrike()
    {
        return action >= TriggerAction.Use && action <= TriggerAction.Read;
    }

    public override void Trigger()
    {
        triggerable.FireTrigger();
    }
}

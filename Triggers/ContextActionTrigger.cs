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
    Read,
    Inspect,
    Pick_Up
}

public class ContextActionTrigger : BaseTrigger 
{
    public ContextAction action = ContextAction.Use;

	void Start()
	{
        if (triggerable == null)
        {
            triggerable = GetComponent<Triggerable>();
        }
	}

	public bool IsStrike()
    {
        return action >= ContextAction.Use && action <= ContextAction.Read;
    }

    public override void Trigger()
    {
        if (triggerable == null)
        {
            Debug.LogWarningFormat("{0} has null triggerable", name);
            return;
        }
        if (triggerable is TalkTrigger)
        {
            var tt = triggerable as TalkTrigger;
            if (tt.host == null)
            {
                tt.host = GetComponent<Actor>();
            }
        }
        triggerable.FireTrigger();
    }
}

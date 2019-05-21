using System.Collections.Generic;
using UnityEngine;

public enum TriggerChildMode
{
    All,
    First,
    Random,
    Last
}

public class TriggerChildren : Triggerable
{
    public TriggerChildMode triggerMode;

    private int nextIndex = 0;

    protected override void OnTrigger()
    {
        var firstIndex = 0;
        var itDir = 1;

        var triggers = Utils.GetChildComponents<Triggerable>(transform);

        switch (triggerMode)
        {
            case TriggerChildMode.Random:
                firstIndex = Random.Range(0, triggers.Length);
                itDir = Random.Range(0, 2) == 0 ? 1 : -1;
                break;
            case TriggerChildMode.Last:
                firstIndex = triggers.Length - 1;
                itDir = -1;
                break;
        }

        for (int i=0; i<triggers.Length; i++)
        {
            var index = itDir > 0 ? i : triggers.Length - i - 1;
            var trigger = triggers[index];

            if (trigger.ConditionPasses())
            {
                trigger.Trigger(isTriggered);
                if (triggerMode != TriggerChildMode.All)
                {
                    return;
                }
            }
        }
    }
}
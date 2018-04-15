using UnityEngine;

public class MultiTrigger : Triggerable
{
    public Triggerable[] triggerables;

    protected override void OnTrigger()
    {
        foreach (var triggerable in triggerables)
        {
            triggerable.Trigger(isTriggered);
        }
    }
}
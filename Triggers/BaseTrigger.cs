using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTrigger : MonoBehaviour 
{
    public Triggerable triggerable;
    public bool once = false;
    bool triggeredOnce = false;

    protected virtual void OnEnable()
    {
        triggeredOnce = false;
        if (triggerable == null)
        {
            triggerable = GetComponent<Triggerable>();
        }
    }

    public virtual void Trigger()
    {
        if (once && triggeredOnce)
            return;

        triggerable.Trigger(true);
        triggeredOnce = true;
    }
}

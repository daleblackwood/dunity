using UnityEngine;
using System.Collections;
using Behave;

public abstract class BehaveAI : BaseAI
{
    public Behave.BDriver behave { get; private set; }

    public override void Reset()
    {
        base.Reset();
        if (behave != null)
        {
            behave.Restart();
        }
    }

    public override void OnReady()
    {
        BehaveStart();
    }

    protected virtual BNode BehaveCreateDefault()
    {
        return BAction.FAIL;
    }

    protected virtual void BehaveStart()
    {
        if (behave == null) 
        {
            behave = new BDriver(name, BehaveCreateDefault());
        }
        behave.Restart();
    }

    protected virtual void BehaveUpdate()
    {
        if (behave != null)
        {
            behave.CallTick(Time.deltaTime);
        }
    }

    protected override void UpdateActions()
    {
        BehaveUpdate();
        base.UpdateActions();
    }
}

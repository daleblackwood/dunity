using UnityEngine;
using System.Collections;
using Behave;

public abstract class StateAI : BaseAI, IStateHost
{
    public StateMachine state { get; private set; }

    protected override void Awake()
    {
        state = new StateMachine(this);
        base.Awake();
    }

    public override void OnReady()
    {
        PickState();
    }

    public virtual void PickState() {}
}

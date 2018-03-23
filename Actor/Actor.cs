using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : GameBehaviour 
{
    public Faction faction = null;

    public Actions actions = new Actions();

    public ActorMover mover { get; private set; }
    public ActorAnimator animator { get; private set; }

    protected BaseControl control = null;
    bool movementEnabled = false;

    protected override void OnInit()
    {
        mover = GetComponentInChildren<ActorMover>();
        control = GetComponent<BaseControl>();
        EnableMovement(false);
        base.OnInit();
    }

    protected override void OnEnable()
    {
        EnableMovement(true);
        base.OnEnable();
    }

    protected override void OnReady()
    {
        base.OnReady();
    }

    protected virtual void Start()
    {
        EnableMovement(false);
        EnableMovement(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (control != null)
        {
            control.Reset();
        }
        EnableMovement(false);
    }

    void Update()
    {
        if (isReady == false) return;
        if (movementEnabled)
        {
            if (control != null)
            {
                control.Update();
            }
            mover.UpdateActions(actions);
        }
    }

    public void EnableMovement(bool enable)
    {
        if (enable == false)
        {
            actions.Reset();
        }
        movementEnabled = enable;
    }
}

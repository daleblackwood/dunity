using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plavatar;

public class Player : GameBehaviour 
{
    public Team team = Team.A;
    public int index = -1;
    public Actions actions = new Actions();

    public ActorMover actorMover { get; private set; }
    public Plavatar.Plavatar plavatar { get; private set; }

    BaseControl control = null;
    bool movementEnabled = false;

    protected override void OnInit()
    {
        actorMover = GetComponentInChildren<ActorMover>();
        //plavatar = GetComponentInChildren<Plavatar.Plavatar>();
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

    void Start()
    {
        control = ControlManager.main.GetControlForPlayer(this);
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
            actorMover.UpdateActions(actions);
        }
    }

    public void EnableMovement(bool enable)
    {
        //plavatar.enabled = enable;
        if (enable == false)
        {
            actions.Reset();
        }
        movementEnabled = enable;
    }
}

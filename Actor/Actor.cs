using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : GameBehaviour 
{
    public Faction faction = null;

    public Actions actions = new Actions();

    public ActorMover mover { get; private set; }
    public ActorAnimator animator { get; private set; }

    public Player player { get; private set; }
    public int playerIndex { get { return player == null ? -1 : player.index; } }

    protected BaseControl control = null;
    bool movementEnabled = false;

    protected override void OnInit()
    {
        mover = GetComponentInChildren<ActorMover>();
        control = GetComponent<BaseControl>();
        player = GetComponentInParent<Player>();
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
        GameStage.main.Register(this);
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

    public void SetControl(BaseControl control)
    {
        this.control = control;
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

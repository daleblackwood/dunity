using UnityEngine;
using System.Collections.Generic;

public class Player : GameBehaviour
{
    public int index = -1;
    int lastIndex = -2;
    public Actor actor { get; private set; }
    public Actor targetContext { get; private set; }
    public ContextActionTrigger[] contexActions = null;
    float waterTimeRemaining = 2.0f;
    public float maxWaterTime = 10.0f;
    UIProgressMeterer meterer = null;

    override protected void OnInit()
    {
        actor = GetComponentInChildren<Actor>();
        actor.onKill = OnKill;
        actor.onReset = OnReset;
    }

    protected override void OnFirstReady()
    {
        meterer = UIManager.main.GetComponentInChildren<UIProgressMeterer>(true);
    }

    void Update()
    {
        if (index != lastIndex)
        {
            var control = ControlManager.main.GetControlForPlayer(this);
            if (control != null)
            {
                actor.SetControl(control);
            }
            lastIndex = index; 
        }

        if (actor.targetContext != targetContext)
        {
            targetContext = actor.targetContext;
            contexActions = targetContext == null ? null : targetContext.GetComponents<ContextActionTrigger>();
        }

        if (actor.mover.inWater && actor.isAlive)
        {
            float waterPC = Mathf.Clamp01(actor.mover.timeInWater / maxWaterTime);

            if (meterer != null)
            {
                meterer.ShowProgress(transform, actor.faction.color, Mathf.Clamp01(1.0f - waterPC * 1.1f));
            }

            if (waterPC >= 1.0f)
            {
                //actor.Kill();
                actor.TakeHit(null);
                if (actor.isAlive)
                {
                    actor.transform.position = actor.mover.lastGroundPos;
                }
            }
        }
    }

    void OnReset()
    {
    }

    void OnKill()
    {
        GameMode.main.Respawn();
    }

    public bool IsActionPressed()
    {
        if (actor == null || actor.actions == null)
            return false;

        return actor.actions.primary || actor.actions.secondary;
    }
}

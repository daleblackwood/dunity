using UnityEngine;
using System.Collections;

public abstract class BaseActioner : MonoBehaviour
{
    public ActionType actionType = ActionType.None;
    public bool requested = false;
    public bool activated = false;
    public float coolDownTime = 0.1f;
    protected float lastActivateTime = 0.0f;
    protected Actions lastActions;
    protected ActorMover mover = null;

    protected virtual void OnEnable()
    {
        activated = false;
        mover = GetComponent<ActorMover>();
    }

    protected virtual void OnDisable()
    {
        if (lastActions != null)
        {
            lastActions.Set(actionType, false);
        }
        activated = false;
    }

    public void UpdateAction(Actions actions)
    {
        lastActions = actions;

        bool wasRequested = requested;
        requested = actions.IsSet(actionType);

        bool inCoolDown = Time.time - lastActivateTime < coolDownTime;

        if (requested != wasRequested)
        {
            if (requested)
            {
                if (inCoolDown == false)
                {
                    activated = true;
                    lastActivateTime = Time.time;
                    inCoolDown = true;
                    OnActivate();
                }
            }
            else
            {
                OnDeactivate();
            }
        }

        if (activated && inCoolDown == false)
        {
            activated = false;
            OnCooldown();
        }
    }

    protected abstract void OnActivate();

    protected virtual void OnCooldown(){}

    protected virtual void OnDeactivate(){}
}

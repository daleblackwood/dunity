using UnityEngine;
using System.Collections.Generic;

public class Player : GameBehaviour
{
    public int index = -1;
    int lastIndex = -2;
    public Actor actor { get; private set; }
    public Actor targetContext { get; private set; }
    public ContextActionTrigger[] contexActions = null;

    override protected void OnInit()
    {
        actor = GetComponentInChildren<Actor>();
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
    }
}

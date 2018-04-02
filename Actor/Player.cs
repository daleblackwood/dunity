using UnityEngine;
using System.Collections.Generic;

public class Player : GameBehaviour
{
    public enum ContextAction
    {
        None    = 0,
        Talk    = 1,
        Attack  = 2,
        Stay    = 4
    }

    public int index = -1;
    int lastIndex = -2;
    public Actor actor { get; private set; }
    public Actor target { get; private set; }
    public ContextAction[] contexActions = null;

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

        if (actor.targetActor != target)
        {
            target = actor.targetActor;

            var contextActionList = new List<ContextAction>();
            if (target != null)
            {
                if (target.faction == actor.faction)
                {
                    var talker = target.GetComponent<NPCTalker>();
                    if (talker != null)
                    {
                        contextActionList.Add(ContextAction.Talk);
                    }
                }
                else
                {
                    contextActionList.Add(ContextAction.Attack);
                }
            }

            contexActions = contextActionList.ToArray();
        }
    }
}

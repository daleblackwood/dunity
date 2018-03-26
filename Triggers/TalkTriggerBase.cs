using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TalkTriggerBase : Triggerable 
{
    public TalkConversation conversation = null;
    public bool isTalking { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        isTargetable = false;
        //faction = Faction.None;
    }

    protected override void OnTrigger()
    {
        if (isTriggered)
        {
            Talk();
        }
    }

    public void Talk()
    {
        /*
        var actorA = Game.Instance.player;
        var entityB = Game.Instance.player.targetFinder.target;
        if (entityB == null)
        {
            entityB = Game.Instance.player.target;
        }
        if (entityB == null)
        {
            entityB = actorA.FindTarget(20.0f);
        }
        var actorB = entityB.castActor;
        Talk(actorA, actorB);
        */
    }

    public void Talk(Actor actorA, Actor actorB)
    {
        if (actorA == null)
            throw new UnityException("Actor A is null.");

        if (actorB == null)
            throw new UnityException("Actor B is null.");

        var newConvo = new TalkConversation();
        
        newConvo.actorA = actorA;
        newConvo.actorB = actorB;
        Talk(newConvo);
    }

    public void Talk(TalkConversation newConvo)
    {
        if (isTalking)
            return;
        
        this.conversation = newConvo;
        OnTalk();
    }

    protected abstract void OnTalk();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkTrigger : Triggerable 
{
    public bool isTalking { get; protected set; }
    public Actor host;
    [HideInInspector] public List<TalkNode> talkNodes = new List<TalkNode>();
    public System.Action onComplete = null;
    public Conversation conversation { get; private set; }

    protected override void OnTrigger()
    {
        if (isTriggered)
        {
            Talk();
        }
    }

    public void Talk()
    {
        OnTalk();
    }

    public void Talk(Actor actorA, Actor actorB)
    {
        var newConvo = new Conversation();
        newConvo.host = host;
        Talk(newConvo);
    }

    public void Talk(Conversation newConvo)
    {
        if (isTalking)
            return;
        
        this.conversation = newConvo;
        OnTalk();
    }

    protected virtual void OnTalk()
    {
        if (isTalking)
            return;

        if (talkNodes.Count < 1)
            return;

        var nodes = new List<TalkNode>();
        foreach (var talkNode in talkNodes)
        {
            nodes.Add(talkNode);
        }

        if (nodes.Count < 1)
            return;

        conversation = new Conversation();
        conversation.host = host;
        conversation.talkNodes = nodes.ToArray();

        isTalking = true;

        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        SequenceManager.main.Talk(conversation, TalkComplete);
    }

    protected void TalkComplete()
    {
        isTalking = false;
        conversation = null;
        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            Talk();
        }
    }
}

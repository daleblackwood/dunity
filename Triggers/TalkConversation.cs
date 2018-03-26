using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TalkNode
{
    public Actor actor;
    public string speech;
    public Triggerable trigger;
}

[System.Serializable]
public class TalkConversation
{
    public Actor actorA;
    public Actor actorB;
    public TalkNode[] talkNodes = new TalkNode[0];
    public bool isTalking = false;
}

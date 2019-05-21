using System.Collections.Generic;
using UnityEngine;

public enum Talker
{
    Host,
    Player,
    Sidekick1,
    Sidekick2,
    Sidekick3,
    Custom
}

[System.Serializable]
public class TalkNode
{
    public Talker talker = Talker.Host;
    public Actor actor;
    public string speech;
    public Triggerable trigger;
}

[System.Serializable]
public class Conversation
{
    static public Conversation FromString(string speech)
    {
        var node = new TalkNode();
        node.speech = speech;
        var convo = new Conversation();
        convo.talkNodes = new TalkNode[]{ node };
        return convo;
    }

    public Actor host;
    public TalkNode[] talkNodes = new TalkNode[0];
    public bool isTalking = false;
}

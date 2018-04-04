using System.Collections.Generic;
using UnityEngine;

public enum Talker
{
    Host,
    Player,
    Sidekick,
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
    public Actor host;
    public TalkNode[] talkNodes = new TalkNode[0];
    public bool isTalking = false;
}

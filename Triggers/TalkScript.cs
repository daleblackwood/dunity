using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkScript : TalkTriggerBase
{
    public string condition;
    public List<TalkNode> talkNodes = new List<TalkNode>();
    public System.Action onComplete = null;

    protected override void OnTalk()
    {
        if (conversation == null || isTalking)
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
    
    public bool ConditionPasses(string condition)
    {
        //if (string.IsNullOrEmpty(condition))
            return true;

        //return Game.Instance.saver.saveGame.GetBool(condition);
    }

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            var talkPicker = GetComponentInParent<TalkPicker>();
            if (talkPicker)
            {
                talkPicker.Talk();
            }
            else
            {
                Talk();
            }
        }
    }
}
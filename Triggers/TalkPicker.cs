using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkPicker : TalkTrigger
{
    TalkScript selected = null;

    protected override void OnTalk()
    {
        isTalking = true;

        for (var i = 0; i < transform.childCount; i++)
        {
            var script = transform.GetChild(i).GetComponent<TalkScript>();
            if (script == null)
                continue;
            
            bool passes = script.ConditionPasses(script.condition);
            if (passes == false)
                continue;

            selected = script;
            script.onComplete = TalkComplete;
            script.Talk(conversation);
            break;
        }

        Debug.LogWarning(name + ": no talkscript found.");
    }

    void TalkComplete()
    {
        isTalking = false;
        selected.onComplete = null;
        selected = null;
    }

}
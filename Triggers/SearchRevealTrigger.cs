using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchRevealTrigger : Triggerable 
{
    public string searchTarget;

    override protected void OnTrigger()
    {
        var target = GameObject.Find(searchTarget);
        if (target != null)
        {
            SequenceManager.main.RevealTarget(target.transform);
        }
    }
}

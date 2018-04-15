using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStateObject : Triggerable 
{
    public GameObject objectUntriggered;
    public GameObject objectTriggered;
    public string condition;

    public override void Reset()
    {
        base.Reset();
        Evaluate();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Evaluate();
    }

    void Evaluate()
    {
        if (GameStage.main == null || GameStage.main.saveData == null)
            return;
        
        bool triggered = GameStage.main.saveData.IsTrue(saveKey);
        if (objectUntriggered != null)
        {
            objectUntriggered.SetActive(triggered == false);
        }
        if (objectTriggered != null)
        {
            objectTriggered.SetActive(triggered);
        }
    }

    protected override void OnTrigger()
    {
        GameStage.main.saveData.SetValue(saveKey, true);
        Evaluate();
    }
}

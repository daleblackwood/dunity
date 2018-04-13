using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStateObject : GameBehaviour 
{
    public GameObject objectUntriggered;
    public GameObject objectTriggered;
    public string condition;

    public override void Reset()
    {
        base.Reset();
        bool triggered = GameStage.main.saveData.IsTrue(condition);
        //objectUntriggered.SetActive(triggered == false);
        //objectTriggered.SetActive(triggered);
    }
}

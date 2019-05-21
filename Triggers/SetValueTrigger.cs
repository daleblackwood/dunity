using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetValueTrigger : Triggerable
{
    public string key;

    override protected void OnTrigger()
    {
        SaveData.main.game.SetValue(key, isTriggered);
    }
}

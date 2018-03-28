using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollectTrigger : MonoBehaviour 
{
    public Triggerable trigger;

    public string[] itemKeys;
    public GameObject[] avatars;
    public bool announce = true;

    int itemCount = 0;
    int prevItemCount = -1;
	
	void Update() 
    {
        if (Game.isReady == false)
            return;
        
        itemCount = 0;
        int targetCount = itemKeys.Length;
        foreach (var key in itemKeys)
        {
            bool isCollected = trigger.saveState == SaveData.EntitySaveState.Triggered;
            if (isCollected == false)
                continue;

            itemCount++;
        }

        if (itemCount != prevItemCount)
        {
            prevItemCount = itemCount;
            for (int i=0; i<avatars.Length; i++)
            {
                var avatar = avatars[i];
                avatar.SetActive(i < itemCount);
            }

            if (itemCount != 0)
            {
                if (announce && itemCount < targetCount)
                {
                    Announce();
                }
                else if (trigger != null && trigger.isTriggered == false)
                {
                    trigger.Trigger(true);
                }   
            }
        }
	}

    void Announce()
    {
        int targetCount = itemKeys.Length;
        int remaining = targetCount - itemCount;
        var t = transform;
        var cam = GetComponentInChildren<Camera>(true);
        if (cam != null)
        {
            t = cam.transform;
        }
        var message = remaining.ToString() + " to go.";
        //Game.Instance.sequence.RevealSign(t, message);
        //SoundPlayer.main.Play("adventure_solve_big");
    }
}

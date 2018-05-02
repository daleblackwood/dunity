using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Triggerable : Entity, ITriggerable 
{
    public bool isTriggered { get; private set; }
    public bool doReveal = false;
    public bool oneTime { get; private set; }

    public bool hasTriggered { get; private set; }

    override protected void Awake()
    {
        base.Awake();
    }

    override protected void OnReady()
    {
        base.OnReady();

        //if (Game.Instance.saver.saveGame.IsCollected(this))
        {
            OnTriggerComplete();
        }
    }

    public virtual void Trigger(bool nowTriggered)
    {
        if (nowTriggered != isTriggered)
        {
            if (doReveal && nowTriggered)
            {
                //if (SequenceManager.main.isPlaying == false)
                {
                    SequenceManager.main.RevealTarget(transform, () =>
                    {
                        SetTrigger(nowTriggered);
                    });
                }
            }
            else
            {
                SetTrigger(nowTriggered);
            }
        }
    }

    public void FireTrigger()
    {
        SetTrigger(true);
        isTriggered = false;
    }

    protected void SetTrigger(bool nowTriggered)
    {
        if (hasTriggered && oneTime)
            return;
        
        if (nowTriggered != isTriggered)
        {
            //Game.Instance.saver.saveGame.SetCollected(this);
            isTriggered = nowTriggered;
            OnTrigger();
        }

        if (nowTriggered)
        {
            hasTriggered = true;
        }
    }

    protected abstract void OnTrigger();

    protected virtual void OnTriggerComplete() {}
}

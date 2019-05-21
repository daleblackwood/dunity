using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Persistance
{
    None,
    Stage,
    Game
}

public abstract class Triggerable : Entity, ITriggerable 
{
    public string condition;

    public bool isTriggered { get; protected set; }
    public bool oneTime { get; private set; }

    public bool hasTriggered { get; private set; }

    public Persistance persistance = Persistance.None;

    public Triggerable nextTrigger;

    override protected void Awake()
    {
        base.Awake();
    }

    override protected void OnReady()
    {
        base.OnReady();

        isTriggered = IsSaveTriggered();
        if (isTriggered)
        {
            OnTriggerComplete();
        }
    }

    public bool IsSaveTriggered()
    {
        if (Game.isReady == false)
        {
            return false;
        }
        if (persistance != Persistance.None)
        {
            if (persistance == Persistance.Game && SaveData.main.game.IsTrue(saveKey))
            {
                return true;
            }
            else if (persistance == Persistance.Stage)
            {
                return GameStage.main.saveData.IsTriggered(this);
            }
        }
        return false;
    }

    public virtual void Trigger(bool nowTriggered)
    {
        if (ConditionPasses() == false)
        {
            Debug.LogWarningFormat("Trigger condition {0} didn't pass", condition);
            return;
        }

        if (nowTriggered != isTriggered)
        {
            /*
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
             */
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
            isTriggered = nowTriggered;
            
            OnTrigger();
            SaveTrigger();

            if (isTriggered && nextTrigger != null)
            {
                nextTrigger.Trigger(true);
            }
        }

        if (nowTriggered)
        {
            hasTriggered = true;
        }
    }

    protected void SaveTrigger()
    {
        if (persistance == Persistance.Stage)
        {
            GameStage.main.saveData.SetTriggered(this, isTriggered);
        }
        else if (persistance == Persistance.Game)
        {
            SaveData.main.game.SetValue(saveKey, isTriggered);
        }
    }

    protected abstract void OnTrigger();

    protected virtual void OnTriggerComplete() {}

    public bool ConditionPasses()
    {
        if (oneTime && hasTriggered)
            return false;
            
        if (string.IsNullOrEmpty(condition) || string.IsNullOrEmpty(condition + ""))
            return true;

        return EvalUtils.ParseCondition(condition, SaveData.main.LookupValue);
    }
}

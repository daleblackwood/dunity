using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsInactiveTrigger : BaseTrigger 
{
    public GameObject[] objects;
    [Range(0.0f, 2.0f)] public float delay = 0.5f;

    public bool complete = false;
    float triggerTime = 0.0f;
    float completeTime = 0.0f;

    protected override void OnEnable()
    {
        completeTime = 0.0f;
        base.OnEnable();
    }

    void Update()
    {
        if (complete == false)
        {
            UpdateIncomplete();
        }
    }

    void UpdateIncomplete()
    {
        var active = GetActiveCount();

        if (active == 0 && complete == false)
        {
            completeTime += Time.unscaledDeltaTime;
            if (completeTime > delay)
            {
                complete = true;
                Trigger();
                gameObject.SetActive(false);
            }
        }
    }

    GameObject GetActiveObject()
    {
        GameObject closest = null;
        float dist2 = float.PositiveInfinity;

        foreach (var obj in objects)
        {
            if (obj == null || obj.activeInHierarchy == false)
                continue;

            float newDist2 = (obj.transform.position - transform.position).sqrMagnitude;
            if (newDist2 < dist2)
            {
                dist2 = newDist2;
                closest = obj;
            }
        }

        return closest;
    }

    int GetActiveCount()
    {
        int count = 0;
        foreach (var obj in objects)
        {
            if (obj == null || obj.activeInHierarchy == false)
                continue;

            count++;
        }
        return count;
    }

    void OnCollisionEnter(Collision collision)
    {
        /*
        var sword = collision.collider.GetComponentInParent<Sword>();
        if (sword != null && sword.actor != null)
        {
            sword.Cancel();

            if (sword.actor.isFighting == false)
            {
                if (Time.time - triggerTime > 1.0f)
                {
                    triggerTime = Time.time;
                    var active = GetActiveCount();
                    if (active != 0)
                    {
                        string remains = active.ToString() + " remain";
                        if (active == 1)
                        {
                            remains += "s";
                        }
                        SequenceManager.main.RevealSign(transform, remains);
                        //Game.Instance.sequence.RevealTarget(active.transform);
                    }
                }
            }
        }
        */
    }
}

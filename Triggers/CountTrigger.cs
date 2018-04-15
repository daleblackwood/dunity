using UnityEngine;

public class CountTrigger : Triggerable
{
    public Triggerable triggerable;
    public int requireCount = 2;
    public int currentCount = 0;

    public override void Trigger(bool nowTriggered)
    {
        if (nowTriggered)
        {
            currentCount++;
        }
        else
        {
            currentCount--;
            if (currentCount < 0)
            {
                currentCount = 0;
            }
        }

        bool allTriggered = currentCount == requireCount;
        if (triggerable.isTriggered != allTriggered)
        {
            if (allTriggered)
            {
                triggerable.Trigger(true);
            }
            else
            {
                triggerable.Trigger(false);
            }
        }
    }

    protected override void OnTrigger()
    {
    }
}
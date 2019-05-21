using UnityEngine;

public class DependantTrigger : MonoBehaviour
{
    public Triggerable[] dependants;
    public Triggerable triggerable;

    bool wasTriggered = false;

    void Update()
    {
        bool nowTriggered = true;
        foreach (var dependant in dependants)
        {
            if (dependant.IsSaveTriggered() == false || dependant.isTriggered == false)
            {
                nowTriggered = false;
                break;
            }
        }

        if (nowTriggered != wasTriggered)
        {
            wasTriggered = nowTriggered;
            if (nowTriggered)
            {
                triggerable.Trigger(true);
            }
        }
    }
}
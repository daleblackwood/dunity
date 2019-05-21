using UnityEngine;

public class ActivateObjectTrigger : Triggerable
{
    public GameObject activateTarget;
    public bool activateOff;
    public FXType fxType;
    public float fxShake = 0.0f;
    public float fxScale = 1.0f;

    protected override void OnTrigger()
    {
        var target = activateTarget == null ? gameObject : activateTarget;

        ActivateObject(isTriggered);

        if (fxType != FXType.None)
        {
            FXer.main.Fire(fxType, target.transform.position, fxScale);
            if (fxShake > float.Epsilon)
            {
                BaseCam.main.Shake(1.0f);
            }
        }
    }

    protected override void OnTriggerComplete()
    {
        ActivateObject(isTriggered);
    }

    void ActivateObject(bool activated)
    { 
        bool shouldActivate = isTriggered;
        if (activateOff)
        {
            shouldActivate = shouldActivate == false;
        }

        var target = activateTarget == null ? gameObject : activateTarget;
        target.SetActive(shouldActivate);
    }
}
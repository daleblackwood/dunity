using UnityEngine;
using System.Collections;

public class Entity : GameBehaviour
{
    [HideInInspector] public string saveKey = null;

    public bool isTargetable = false;

    public virtual void TakeHit(EntityHit hit = null) {}

    public virtual void OnSave() {}

    public void Activate()
    {
        enabled = true;
        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }
    }

    public void Deactivate()
    {
        enabled = false;
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}

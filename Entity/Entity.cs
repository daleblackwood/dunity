using UnityEngine;
using System.Collections;

public class Entity : GameBehaviour
{
    [HideInInspector] public string saveKey = null;

    public bool isTargetable = false;

    public virtual void TakeHit(EntityHit hit = null) {}

    public virtual void OnSave() {}
}

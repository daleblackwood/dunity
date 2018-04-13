using UnityEngine;
using System.Collections;

public class Entity : GameBehaviour
{
    [HideInInspector] public string saveKey = null;

    public bool isTargetable = false;

    public virtual void TakeHit(Entity other, int damage = 1) {}
}

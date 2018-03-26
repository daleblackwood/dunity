using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireable : Entity 
{
    public void Fire()
    {
        OnFire();
    }

    protected virtual void OnFire(){}
}

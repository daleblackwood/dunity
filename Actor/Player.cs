using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plavatar;

public class Player : Actor 
{
    public int index = -1;

    protected override void Start()
    {
        control = ControlManager.main.GetControlForPlayer(this);
        base.Start();
    }
}

using UnityEngine;
using System.Collections;

public class JumpActioner : BaseActioner
{
    override protected void OnActivate()
    {
        mover.Jump();
        mover.animator.TriggerBool("Jump");
    }

    override protected void OnDeactivate()
    {
        mover.JumpStop();
    }
}

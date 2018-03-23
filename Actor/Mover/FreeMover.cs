using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeMover : Mover 
{
    public float speed = 8.0f;
    public float turnSpeed = 4.0f;

    protected override void OnMove()
    {
        var moveDir = GetMoveCamera(actions.move);
        var newWish = moveDir * speed;

        wishMove = newWish;

        if (wishMove.sqrMagnitude > 0.1f)
        {
            wishRot = Quaternion.LookRotation(wishMove.normalized, Vector3.up);
        }
    }

    protected void LateUpdate()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, wishRot, Time.deltaTime * 300.0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : BaseControl
{
    InputDevice device = null;
    //PartyActionSet actionSet = null;
    //TouchInput touch = null;

    public void DebugAssign()
    {
        ControlManager.main.AddDefaultDevice();
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        int index = actor is Actor ? ((Actor)actor).playerIndex : -1;
        device = ControlManager.main.GetDevice(index);
        //actionSet = new PartyActionSet();
        //actionSet.Device = device;
    }

    protected override void UpdateActions()
    {
        actions.Reset();

        if (device != null)
        {
            device.UpdateActions(actions);
        }

        /*
        int index = actor is Actor ? ((Actor)actor).playerIndex : -1;
        if (index == 0)
        {
            if (touch == null)
            {
                touch = new TouchInput();
            }

            if (Application.isMobilePlatform || Input.GetMouseButton(0))
            {
                touch.UpdateActions(actions);
            }

            var keyMove = Vector2.zero;

            // Arrows for lefties
            // WASD for 1337ies
            // IJKL because of narrow seating on flight
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.I))
            {
                keyMove.y += 1.0f;
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.K))
            {
                keyMove.y -= 1.0f;
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.J))
            {
                keyMove.x -= 1.0f;
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.L))
            {
                keyMove.x += 1.0f;
            }
            actions.move += keyMove;

            actions.primary |= Input.GetKey(KeyCode.Space);
            actions.secondary |= Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Z);
        }
         */
    }
}

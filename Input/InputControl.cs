using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputControl : BaseControl
{
    InputDevice device = null;
    PartyActionSet actionSet = null;

    public void DebugAssign()
    {
        ControlManager.main.AddDefaultDevice();
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        device = ControlManager.main.GetDevice(player.index);
        actionSet = new PartyActionSet();
        actionSet.Device = device;
    }
    protected override void UpdateActions()
    {
        actions.Reset();

        if (device != null)
        {
            actionSet.UpdateActions(actions);
        }

        if (player.index == 0)
        {
            var keyMove = Vector2.zero;
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                keyMove.y += 1.0f;
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                keyMove.y -= 1.0f;
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                keyMove.x -= 1.0f;
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                keyMove.x += 1.0f;
            }
            actions.move += keyMove;

            actions.primary |= Input.GetKey(KeyCode.Space);
            actions.secondary |= Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Z);
        }
    }
}

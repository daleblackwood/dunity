using InControl;
using UnityEngine;

public class PartyActionSet : PlayerActionSet
{
    public PlayerAction left;
    public PlayerAction right;
    public PlayerAction up;
    public PlayerAction down;
    public PlayerTwoAxisAction move;

    public PlayerAction primary;
    public PlayerAction secondary;

    public PlayerAction pause;
    public PlayerAction submit;

    Actions actions = new Actions();
    int lastFrame = -1;

    public PartyActionSet()
    {
        left = CreatePlayerAction("Left");
        right = CreatePlayerAction("Right");
        up = CreatePlayerAction("Up");
        down = CreatePlayerAction("Down");
        move = CreateTwoAxisPlayerAction(left, right, down, up);

        primary = CreatePlayerAction("Primary");
        secondary = CreatePlayerAction("Secondary");

        pause = CreatePlayerAction("Pause");
        submit = CreatePlayerAction("Submit");

        SetDefault();
    }

    public void SetDefault()
    {
        left.AddDefaultBinding(Key.LeftArrow);
        right.AddDefaultBinding(Key.RightArrow);
        up.AddDefaultBinding(Key.UpArrow);
        down.AddDefaultBinding(Key.DownArrow);

        left.AddDefaultBinding(Key.A);
        right.AddDefaultBinding(Key.D);
        up.AddDefaultBinding(Key.W);
        down.AddDefaultBinding(Key.S);

        pause.AddDefaultBinding(Key.Escape);

        primary.AddDefaultBinding(Key.Space);
        secondary.AddDefaultBinding(Key.Control);

        left.AddDefaultBinding(InputControlType.LeftStickLeft);
        right.AddDefaultBinding(InputControlType.LeftStickRight);
        up.AddDefaultBinding(InputControlType.LeftStickUp);
        down.AddDefaultBinding(InputControlType.LeftStickDown);

        primary.AddDefaultBinding(InputControlType.Action1);
        primary.AddDefaultBinding(InputControlType.Action4);
        secondary.AddDefaultBinding(InputControlType.Action2);
        secondary.AddDefaultBinding(InputControlType.Action3);

        pause.AddDefaultBinding(InputControlType.Start);
    }

    public Actions UpdateActions(Actions actions)
    {
        if (lastFrame != Time.frameCount)
        {
            actions.move = move.Vector;
            actions.primary = primary.IsPressed;
            actions.secondary = secondary.IsPressed;
            lastFrame = Time.frameCount;
        }
        return actions;
    }
}


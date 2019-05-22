using UnityEngine;

[System.Serializable]
public abstract class InputDevice
{   
    public enum ButtonType
    {
        Down,
        Right,
        Left,
        Up,
        Stick,
        Shoulder,
        MAX
    }

    public bool isAttached = false;
    public bool anyButtonIsPressed = false;
    public bool anyButtonNowPressed = false;
    public bool anyButtonNowReleased = false;
    public Vector2 moveStick;
    public Quaternion gyro = Quaternion.identity;
    public Vector3 accel = Vector3.zero;
    public Vector3 gyroMulti = Vector3.one;

    public bool[] pressed = new bool[(int)ButtonType.MAX];
    bool[] prevPressed = new bool[(int)ButtonType.MAX];

    public virtual void Init() {}

    public virtual void UpdateActions(Actions actions)
    {
        for (int i=0; i<pressed.Length; i++)
        {
            prevPressed[i] = pressed[i];
        }

        UpdateDevice();
        UpdateButtonStates();

        var wasPressed = anyButtonIsPressed;
        anyButtonIsPressed = false;
        for (int i=0; i<pressed.Length; i++)
        {
            if (pressed[i])
            {
                anyButtonIsPressed = true;
            }
        }
        anyButtonNowPressed = wasPressed == false && anyButtonIsPressed;
        anyButtonNowReleased = wasPressed && anyButtonIsPressed == false;

        UpdateSticks();

        UpdateMotion();
    }

    protected virtual void UpdateDevice(){}

    protected virtual void UpdateButtonStates(){}

    protected virtual void UpdateSticks(){}

    protected virtual void UpdateMotion(){}

    public bool GetButton(ButtonType button)
    {
        return pressed[(int)button];
    }

    public bool GetButtonNowPressed(ButtonType button)
    {
        return pressed[(int)button] && prevPressed[(int)button] == false;
    }

    public bool GetButtonNowReleased(ButtonType button)
    {
        return prevPressed[(int)button] && pressed[(int)button] == false;
    }
}
using UnityEngine;

[System.Serializable]
public class JoyconInputDevice : InputDevice
{
    public int joyconIndex = 0;
    JoyconDeviceProxy joycon;
    
    protected override void UpdateDevice()
    {
        joycon = JoyconDeviceProxy.GetDevice(joyconIndex);
        joycon.Update();
    }

    protected override void UpdateButtonStates()
    {
        pressed[(int)ButtonType.Down] = joycon.GetButton(Joycon.Button.DPAD_DOWN);
        pressed[(int)ButtonType.Right] = joycon.GetButton(Joycon.Button.DPAD_RIGHT);
        pressed[(int)ButtonType.Left] = joycon.GetButton(Joycon.Button.DPAD_LEFT);
        pressed[(int)ButtonType.Up] = joycon.GetButton(Joycon.Button.DPAD_UP);
        pressed[(int)ButtonType.Stick] = joycon.GetButton(Joycon.Button.STICK);
        pressed[(int)ButtonType.Shoulder] = joycon.GetButton(Joycon.Button.SHOULDER_1);
    }

    protected override void UpdateSticks()
    {
        moveStick = joycon.stick;
    }

    protected override void UpdateMotion()
    {
        gyro = joycon.gyro;
        accel = joycon.accel;
    }
}
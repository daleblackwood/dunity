using UnityEngine;

[System.Serializable]
public class JoyconInputDevice : InputDevice
{
    public int joyconIndex = 0;
    Joycon joycon;

    public override void Init() 
    {
        joycon = JoyconManager.Instance.j[0];
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
        var stickAxes = joycon.GetStick();
        moveStick.x = stickAxes[0];
        moveStick.y = stickAxes[1];
    }

    protected override void UpdateMotion()
    {
        // Gyro values: x, y, z axis values (in radians per second)
        var gv = joycon.GetGyro();
        gyro = Quaternion.Euler(gv.x * gyroMulti.x, gv.y * gyroMulti.y, gv.z * gyroMulti.z);

        // Accel values:  x, y, z axis values (in Gs)
        accel = joycon.GetAccel();
    }
}
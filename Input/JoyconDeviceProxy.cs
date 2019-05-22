using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconDeviceProxy
{
    static List<JoyconDeviceProxy> devices = new List<JoyconDeviceProxy>();
    static public JoyconDeviceProxy GetDevice(int index)
    {
        if (index >= 8 || index < 0)
        {
            throw new UnityException("Invalid joycon index " + index);
        }
        while (devices.Count <= index)
        {
            devices.Add(new JoyconDeviceProxy(devices.Count));
        }
        return devices[index];
    }

    public int index = 0;
    Joycon joycon;
    public Vector2 stick { get; private set; }
    public Quaternion gyro { get; private set; }
    public Vector3 accel { get; private set; }
    public Vector3 down { get; private set; }

    // values for calculating gyro
    Vector3 downRaw = Vector3.down;
    float noDownTime = 0.0f;
    Quaternion gyroRaw = Quaternion.identity;
    public float downRawMag = 0.0f;
    public float stopTime = 0.0f;
    public bool moving = false;
    Vector3 accelRaw = Vector3.zero;
    int lastUpdateFrame = -1;

    private JoyconDeviceProxy(int index)
    {
        this.index = index;
    }

    public void Update()
    {
        if (lastUpdateFrame == Time.frameCount)
            return;

        lastUpdateFrame = Time.frameCount;

        if (JoyconManager.Instance == null)
        {
            var go = new GameObject("JoyconManager");
            go.AddComponent<JoyconManager>();
        }

        if (JoyconManager.Instance.j.Count <= index)
        {
            joycon = null;
            return;
        }

        joycon = JoyconManager.Instance.j[index];

        UpdateStick();
        UpdateGyro();
        UpdateAccel();
    }

    public bool GetButton(Joycon.Button button)
    {
        if (joycon == null)
            return false;

        return joycon.GetButton(button);
    }

    void UpdateStick()
    {
        var stickRaw = joycon.GetStick();
        stick = new Vector2(stickRaw[0], stickRaw[1]);
    }

    void UpdateGyro()
    {
        // reorient accel to find gravity
        var jAccel = joycon.GetAccel();
        downRaw.x = -jAccel.y;
        downRaw.y = jAccel.z;
        downRaw.z = -jAccel.x;

        // detect changes in motion
        downRawMag = downRaw.magnitude;
        var nearStop = downRawMag > 0.95f && downRawMag < 1.05f;
        if (nearStop)
        {
            stopTime += Time.deltaTime;
        }
        else
        {
            stopTime = 0.0f;
        }

        moving = stopTime < 0.1f;

        // when not swiping, ease slower, else faster
        float downLerpEase = moving ? 0.5f : 0.1f;

        // initialise or lerp down, by the ease
        down = down == default(Vector3) ? downRaw : Vector3.Lerp(down, downRaw, DeltaEase(downLerpEase));;

        var gyroDelta = joycon.GetGyro();
        gyroRaw *= Quaternion.Euler(-gyroDelta.y, gyroDelta.z, -gyroDelta.x);
        gyro = gyroRaw;

        // use look rotations to normalise down
        var forward = gyroRaw * Vector3.forward;
        var gyroRotTo = Quaternion.LookRotation(forward, -down);
        var gyroRotEase = moving ? 0.1f : 0.05f;
        gyro = Quaternion.Lerp(gyro, gyroRotTo, gyroRotEase);
    }

    public void Recenter()
    {
        var right = Vector3.Cross(Vector3.forward, -down);
        var newForward = Vector3.Cross(-down, right);
        var newRot = Quaternion.LookRotation(newForward, -down);
        gyroRaw = Quaternion.Lerp(gyroRaw, newRot, DeltaEase(0.3f));
    }

    public void DrawDebug(Vector3 pos)
    {
        Debug.DrawLine(pos, pos + down, Color.gray);
        Debug.DrawLine(pos, pos + accel, Color.blue);
    }

    void UpdateAccel()
    {
        accelRaw = downRaw - down;
        accel = Vector3.Lerp(accel, accelRaw, DeltaEase(0.2f));
    }

    float DeltaEase(float sharpness)
    {
        return Mathf.Clamp01(1.0f - Mathf.Pow(1.0f - sharpness, Time.deltaTime * 60.0f));
    }
}

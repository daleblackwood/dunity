using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ControlManager : GameBehaviour 
{
    public const int DISCOVER_TIME = 10;

    static public ControlManager main { get; private set; }

    public enum Options
    {
        Auto,
        AI_Only
    }
    public Options options = Options.Auto;

    Dictionary<Player, BaseControl> controls = new Dictionary<Player, BaseControl>();

    public List<InputDevice> devices { get; private set; }
    public bool discovering { get; private set; }

    protected override void OnInit()
    {
        main = this;
        devices = new List<InputDevice>();
    }

    public BaseControl GetControlForPlayer(Player player)
    {
        if (controls.ContainsKey(player))
        {
            return controls[player];
        }

        return AddControlForPlayer(player);
    }

    BaseControl AddControlForPlayer(Player player)
    {
        BaseControl result = null;

        bool isHuman = false;
        if (options != Options.AI_Only)
        {
            if (devices.Count < 1 || Game.isDevelopment)
            {
                isHuman = player.index == 0;
            }
            else
            {
                isHuman = player.index < devices.Count;
            }
        }

        if (isHuman)
        {
            result = player.gameObject.AddComponent<InputControl>();
        }
        else
        {
            result = player.GetComponentInChildren<BaseAI>();
        }

        if (result == null)
        {
            Debug.Log("No control found for player.");
            return null;
        }

        controls[player] = result;

        var playerControls = player.GetComponentsInChildren<BaseControl>();
        foreach (var pc in playerControls) 
        {
            pc.enabled = false;
        }

        result.Init(player);
        return result;
    }

    void Update()
    {
        if (discovering)
        {
            UpdateDiscovery();
        }
    }

    public void StartDiscovery()
    {
        devices.Clear();
        discovering = true;
    }

    void UpdateDiscovery()
    {
        foreach (var device in InputManager.Devices)
        {
            if (device.IsAttached == false)
                continue;

            if (devices.Contains(device))
                continue;

            if (device.AnyButtonIsPressed)
            {
                devices.Add(device);
                if (devices.Count >= 4)
                {
                    StopDiscovery();
                    return;
                }
            }
        }
    }

    public void AddDefaultDevice()
    {
        foreach (var device in InputManager.Devices)
        {
            if (device.IsAttached == false)
                continue;

            if (devices.Contains(device))
                continue;

            devices.Add(device);
            return;
        }
    }

    public void StopDiscovery()
    {
        discovering = false;
    }

    public int GetDeviceIndex(InputDevice device)
    {
        return devices.IndexOf(device);
    }

    public InputDevice GetDevice(int index)
    {
        if (index < 0 || index >= devices.Count)
            return null;

        var device = devices[index];
        if (device == null || device.IsAttached == false)
            return null;

        return device;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class InputManager
{
    public static InputManager main { get; private set; }

    public List<InputDevice> devices = new List<InputDevice>();

    void Awake()
    {
        main = this;
    }
}
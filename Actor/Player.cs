using UnityEngine;
using System.Collections;

public class Player : GameBehaviour
{
    public int index = -1;
    int lastIndex = -2;
    public Actor actor { get; private set; }

    override protected void OnInit()
    {
        actor = GetComponentInChildren<Actor>();
    }

    void Update()
    {
        if (index != lastIndex)
        {
            var control = ControlManager.main.GetControlForPlayer(this);
            if (control != null)
            {
                actor.SetControl(control);
            }
            lastIndex = index; 
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseControl : MonoBehaviour
{
    public Player player { get; protected set; }
    public Actor actor { get; protected set; } 
    public Actions actions { get { PlayerCheck(); return actor.actions; } }

    int lastUpdateFrame = 0;

    protected virtual void Awake()
    {
        player = GetComponent<Player>();
        actor = GetComponent<Actor>();
    }

    public void Init(Player player)
    {
        this.player = player;
        this.actor = player.actor;
        enabled = true;
        Reset();
    }

    public virtual void Reset() {}

    public void Update()
    {
        if (Time.frameCount == lastUpdateFrame)
            return;
        
        UpdateActions();
        lastUpdateFrame = Time.frameCount;
    }

    protected abstract void UpdateActions();

    void PlayerCheck()
    {
        if (actor == null)
        {
            throw new UnityException("Player isn't initialised.");
        }
    }
}

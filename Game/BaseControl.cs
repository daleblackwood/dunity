using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseControl : MonoBehaviour
{
    public Player player { get; protected set; }
    public Actions actions { get { PlayerCheck(); return player.actions; } }

    int lastUpdateFrame = 0;

    protected virtual void Awake()
    {
        player = GetComponent<Player>();
    }

    public void Init(Player player)
    {
        this.player = player;
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
        if (player == null)
        {
            throw new UnityException("Player isn't initialised.");
        }
    }
}

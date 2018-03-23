using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plavatar;

public class GameStage : GameBehaviour, IStateHost
{
    static public GameStage main { get; private set; }

    public PlayerSpawner[] spawnPoints;
    public bool isPlaying { get; protected set; }

    protected StateMachine state;

    Dictionary<Transform, Vector3> initPositions = new Dictionary<Transform, Vector3>();
    Dictionary<Transform, Quaternion> initRotations = new Dictionary<Transform, Quaternion>();
    protected List<Player> players = new List<Player>();

    protected override void Awake()
    {
        main = this;
        state = new StateMachine(this);
        base.Awake();
    }

    protected override void OnInit()
    {
        Game.MakeReady();
        base.OnInit();
    }

    protected virtual void Spawn()
    {
    }

    protected override void OnReady()
    {
        Spawn();

        base.OnReady();
        StartStage();
    }

    public virtual void StartStage()
    {
        UIManager.main.ToPlay();
    }

    public virtual void RestartStage()
    {
        StartStage();
    }

    protected virtual void Update()
    {
    }

    public virtual void PickState()
    {
        state.SetState("Null", null);
    }

    protected void SetPlaying(bool enable)
    {
        if (enable)
        {
            EnablePlayers(true);
            isPlaying = true;
        }
        else
        {
            isPlaying = false;
            EnablePlayers(false);
        }
    }

    public void EnablePlayers(bool enable)
    {
        foreach (var player in players)
        {
            player.enabled = false;
            if (enable)
            {
                player.enabled = enable;
            }
        }
    }

    protected void ResetPositions()
    {
        foreach (var pair in initPositions)
        {
            pair.Key.position = pair.Value;
        }
        foreach (var pair in initRotations)
        {
            pair.Key.rotation = pair.Value;
        }
    }

    protected void SetInitPosition(Transform t)
    {
        initPositions[t] = t.position;
        initRotations[t] = t.rotation;
    }

    public Player[] GetPlayers()
    {
        return players.ToArray();
    }
}

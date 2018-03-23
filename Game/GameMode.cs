using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public abstract class GameMode : GameBehaviour, IStateHost
{
    new static public GameMode main { get; private set; }

    public StateMachine state { get; private set; }

    protected override void OnInit()
    {
        main = this;
        state = new StateMachine(this);
    }

    public void PickState()
    {
        state.SetState(null, null);
    }

    protected IEnumerator CoDiscoverPlayers()
    {
        UIManager.main.ToPickSides();

        yield return new WaitForSeconds(0.5f);

        ControlManager.main.StartDiscovery();

        yield return new WaitForSeconds(ControlManager.DISCOVER_TIME);

        ControlManager.main.StopDiscovery();
    }

    public abstract void StartMode();

    public virtual void NextGame() {}

    public virtual void StopMode() {}
}

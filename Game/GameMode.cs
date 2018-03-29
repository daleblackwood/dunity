using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public abstract class GameMode : GameBehaviour, IStateHost
{
    new static public GameMode main { get; private set; }

    public StateMachine state { get; private set; }

    bool switching = false;

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

    public virtual void SwitchStage(string stageName)
    {
        if (switching)
            return;

        if (SaveData.main.game == null)
        {
            SaveData.main.Load();
        }

        SaveData.main.game.SetStageData(GameStage.main);
        SaveData.main.game.SetStage(stageName);
        SaveData.main.Save();
        Game.main.LoadScene(stageName, true, SwitchStageComplete);
    }

    protected virtual void SwitchStageComplete()
    {
        //SaveData.main.Load();
        switching = false;
    }
}

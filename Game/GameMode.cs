using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public abstract class GameMode : GameBehaviour, IStateHost
{
    static public GameMode main { get; private set; }
  
    public StateMachine state { get; private set; }

    public UIScreen playerDiscoveryScreen;
    public UIScreen playScreen;
    public UIScreen loadScreen;

    public string firstScene = null;
    public string stageName = null;
    public string doorName = null;

    bool switching = false;

    protected override void OnInit()
    {
        main = this;
        state = new StateMachine(this);
    }

    protected override void OnFirstReady()
    {
        stageName = Game.main.sceneName;
    }

    public void PickState()
    {
        state.SetState(null, null);
    }

    protected IEnumerator CoDiscoverPlayers()
    {
        UIManager.main.SwitchTo(playerDiscoveryScreen);

        yield return new WaitForSeconds(0.5f);

        ControlManager.main.StartDiscovery();

        yield return new WaitForSeconds(ControlManager.DISCOVER_TIME);

        ControlManager.main.StopDiscovery();
    }

    public void StartMode()
    {
        Debug.LogFormat("{0}: start game mode", GetKey());
        OnStartMode();
    }

    protected virtual void OnStartMode() {}

    public void NextGame() 
    {
        Debug.LogFormat("{0}: next game", GetKey());
        OnNextGame();
    }

    protected virtual void OnNextGame() {}

    public void StopMode() 
    {
        Debug.LogFormat("{0}: stop game mode", GetKey());
        OnStopMode();
    }

    protected virtual void OnStopMode() {}

    public virtual void SwitchStage(string stageName, string doorName = null)
    {
        if (switching)
            return;

        StartCoroutine(CoSwitchStage(stageName, doorName));
    }

    public void Reload()
    {
        SwitchStage(stageName, doorName);
    }

    IEnumerator CoSwitchStage(string stageName, string doorName)
    {
        if (SaveData.main.game == null)
        {
            SaveData.main.Load();
        }

        if (string.IsNullOrEmpty(stageName))
        {
            stageName = this.stageName;
        }

        bool isSameStage = stageName == this.stageName;

        this.stageName = stageName;
        this.doorName = doorName;

        UIManager.main.SwitchTo(loadScreen);

        if (isSameStage == false)
        {
            SaveData.main.game.SetStageData(GameStage.main);
            SaveData.main.game.SetStage(stageName);
        }

        SaveData.main.Save();

        yield return new WaitForSeconds(0.3f);

        //if (isSameStage == false)
        {
            Game.main.LoadScene(stageName, false);
        }

        while (Game.isReady == false)
        {
            yield return null;
        }

        yield return null;

        GameStage.main.Spawn();

        yield return new WaitForSeconds(0.3f);

        UIManager.main.SwitchTo(playScreen);

    }

    protected string GetKey()
    {
        return "Mode:" + GetType().Name;
    }
}

using UnityEngine;

public class DevMode : GameMode
{
    public override void StartMode()
    {
    }

    public override void NextGame()
    {
        GameStage.main.StartStage();
    }
}
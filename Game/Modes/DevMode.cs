using UnityEngine;

public class DevMode : GameMode
{
    protected override void OnNextGame()
    {
        GameStage.main.StartStage();
    }
}
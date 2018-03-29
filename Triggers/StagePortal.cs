using UnityEngine;
using System.Collections;

public class StagePortal : MonoBehaviour
{
    public string stageName = null;

    float enableTime = 0.0f;

    void OnEnable()
    {
        enableTime = Time.time;
    }

    void OnTriggerEnter(Collider other)
    {
        if (Time.time - enableTime < 2.0f)
            return;

        var player = other.GetComponentInParent<Player>();
        if (player == null)
            return;

        var loadStage = stageName;
        if (string.IsNullOrEmpty(loadStage) && SaveData.main.game != null)
        {
            loadStage = SaveData.main.game.previousStage;
        }

        GameStage.main.saveData.lastDoor = name;
        GameMode.main.SwitchStage(loadStage);
    }
}

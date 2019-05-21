using UnityEngine;
using System.Collections;

public class StagePortal : MonoBehaviour
{
    public string stageName = null;
    public string doorName = null;

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

        var playerVel = player.actor.mover.velocity;
        playerVel.y = 0.0f;
        if (playerVel.sqrMagnitude < 0.1f)
            return;

        var dot = Vector3.Dot(playerVel.normalized, transform.forward);
        if (dot > -0.5f)
            return;

        var loadStage = stageName;
        if (string.IsNullOrEmpty(loadStage) && string.IsNullOrEmpty(doorName) && SaveData.main.game != null)
        {
            loadStage = SaveData.main.game.previousStage;
        }

        GameStage.main.saveData.spawnPoint = name;
        GameMode.main.SwitchStage(loadStage, doorName);
    }
}

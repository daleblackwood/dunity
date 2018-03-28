using UnityEngine;
using System.Collections;

public class StagePortal : MonoBehaviour
{
    public string stageName = null;

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<Player>();
        if (player == null)
            return;

        GameStage.main.SwitchStage(stageName);
    }
}

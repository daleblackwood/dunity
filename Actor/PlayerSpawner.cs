using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour 
{
    public Player playerPrefab;
    public System.Action<Player> onSpawn = null;

    Player player = null;

    public Player Spawn()
    {
        if (player == null)
        {
            player = GameObject.Instantiate(playerPrefab);
        }
        if (onSpawn != null)
        {
            onSpawn(player);
        }
        return player;
    }
}

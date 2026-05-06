using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;

    private int index = 0;

    public void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log("PLAYER JOINED: " + player.name);

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn point kosong!");
            return;
        }

        Debug.Log("Spawn ke: " + spawnPoints[index].position);

        player.transform.position = spawnPoints[index].position;
    }
}
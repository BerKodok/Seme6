using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStartManager : MonoBehaviour
{
    public int requiredPlayers = 3;
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    private List<InputDevice> readyDevices = new List<InputDevice>();

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        InputDevice device = playerInput.devices[0];

        // hindari double
        if (readyDevices.Contains(device)) return;

        readyDevices.Add(device);

        Debug.Log("Player Ready: " + readyDevices.Count);

        // 🔥 kalau sudah cukup → spawn semua
        if (readyDevices.Count >= requiredPlayers)
        {
            SpawnAllPlayers();
        }

        // ❌ HAPUS player sementara (biar gak muncul dulu)
        Destroy(playerInput.gameObject);
    }

    void SpawnAllPlayers()
    {
        Debug.Log("SPAWN SEMUA PLAYER");

        for (int i = 0; i < readyDevices.Count; i++)
        {
            var player = PlayerInput.Instantiate(
                playerPrefab,
                controlScheme: "Gamepad",
                pairWithDevice: readyDevices[i]
            );

            // pindahkan ke spawn point
            player.transform.position = spawnPoints[i].position;
        }
    }
}
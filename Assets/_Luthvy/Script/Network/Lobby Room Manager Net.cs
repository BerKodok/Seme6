using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LobbyRoomManager : NetworkRoomManager
{
    private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName == GameplayScene)
        {
            spawnPoints.Clear();

            // Find all SpawnPoint components in Gameplay scene
            SpawnPoint[] foundSpawns = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

            foreach (var sp in foundSpawns)
            {
                spawnPoints.Add(sp);
            }

            if (spawnPoints.Count == 0)
            {
                Debug.LogError("No SpawnPoint objects found in Gameplay scene!");
            }
        }
    }

    public override GameObject OnRoomServerCreateGamePlayer(
        NetworkConnectionToClient conn,
        GameObject roomPlayer)
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("SpawnPoints list is empty!");
            return null;
        }

        int index = roomPlayer.GetComponent<NetworkRoomPlayer>().index;

        SpawnPoint spawn = spawnPoints[index % spawnPoints.Count];

        GameObject player = Instantiate(
            playerPrefab,
            spawn.transform.position,
            spawn.transform.rotation
        );

        return player;
    }

    public override void OnRoomClientEnter()
    {
        base.OnRoomClientEnter();

        if (LobbyMenuNet.Instance != null)
            LobbyMenuNet.Instance.Refresh();
    }

    public override void OnRoomClientExit()
    {
        base.OnRoomClientExit();

        if (LobbyMenuNet.Instance != null)
            LobbyMenuNet.Instance.Refresh();
    }
}
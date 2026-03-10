using Mirror;
using TMPro;
using UnityEngine;

public class LobbyMenuNet : MonoBehaviour
{
    public static LobbyMenuNet Instance;
    public Transform playerListParent;
    public GameObject playerListProfilePrefab;
    public GameObject startButton;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Update()
    {
        startButton.SetActive(NetworkServer.active);
    }

    public void Refresh()
    {
        if (playerListParent == null) return;
        if (!gameObject.scene.isLoaded) return;

        foreach (Transform child in playerListParent)
            Destroy (child.gameObject);

        var roomManager = NetworkRoomManager.singleton as NetworkRoomManager;
        if (roomManager == null) return;

        foreach (var player in roomManager.roomSlots)
        {
            if (player == null) continue;
            var profile = Instantiate(playerListProfilePrefab, playerListParent);
            profile.GetComponent<TMP_Text>().text = 
            $"Player {player.index}"; // - Ready: {player.readyToBegin}
        }
    }

    public void StartGame()
    {
        if(!NetworkServer.active) return;
        NetworkManager.singleton.ServerChangeScene((NetworkManager.singleton as NetworkRoomManager).GameplayScene);
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
    }
}

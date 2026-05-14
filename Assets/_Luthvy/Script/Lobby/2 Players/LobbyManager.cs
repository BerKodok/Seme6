using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public Transform[] mainPortraitWorldPoints;
    public Transform[] sidePortraitWorldPoints;
    public TMPro.TextMeshProUGUI[] statusBox;
    private int playerCount = 0;
    public static int FinalPlayerCount;

    public void OnPlayerJoined(PlayerInput player)
    {
        int index = player.playerIndex;

        var selector = player.GetComponent<CharacterSelectManager>();

        selector.mainPreviewPoint = mainPortraitWorldPoints[index];
        selector.sidePreviewPoint = sidePortraitWorldPoints[index];
        selector.readyString = statusBox[index];

        playerCount = Mathf.Max(playerCount, index + 1);
        FinalPlayerCount = playerCount;
    }

    public int GetPlayerCount()
    {
        return playerCount;
    }
}
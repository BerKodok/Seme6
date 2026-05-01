using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public Image[] portraitSlots;
    public TMPro.TextMeshProUGUI[] readyBox;
    private int playerCount = 0;
    public static int FinalPlayerCount;

    public void OnPlayerJoined(PlayerInput player)
    {
        int index = player.playerIndex;

        var selector = player.GetComponent<CharacterSelectManager>();

        selector.characterPortrait = portraitSlots[index];
        selector.readyString = readyBox[index];

        playerCount++;
        FinalPlayerCount = playerCount;
    }

    public int GetPlayerCount()
    {
        return playerCount;
    }
}
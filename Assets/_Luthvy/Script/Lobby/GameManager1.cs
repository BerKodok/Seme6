using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class GameManager1 : MonoBehaviour
{
    public GameObject[] CharacterPrefabs;
    public Transform[] spawnPoints;

    void Start()
    {
        int playerCount = LobbyManager1.FinalPlayerCount;

        for (int i = 0; i < playerCount; i++)
        {
            int characterID =
                CharacterSelectManager1.SelectedCharacter[i];

            if (characterID < 0 ||
                characterID >= CharacterPrefabs.Length)
            {
                Debug.LogError(
                    "Invalid Character ID: " + characterID);

                continue;
            }

            GameObject playerObj = Instantiate(
                CharacterPrefabs[characterID],
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            PlayerInput playerInput =
                playerObj.GetComponentInChildren<PlayerInput>();

            InputDevice device =
                CharacterSelectManager1.PlayerDevices[i];

            if (playerInput == null || device == null)
            {
                Debug.LogError(
                    "Missing PlayerInput or Device");

                continue;
            }

            playerInput.user.UnpairDevices();

            InputUser.PerformPairingWithDevice(
                device,
                playerInput.user
            );

            if (device is Gamepad)
            {
                playerInput.SwitchCurrentControlScheme(
                    "Gamepad",
                    Gamepad.current
                );
            }
            else
            {
                playerInput.SwitchCurrentControlScheme(
                    "Mouse Keyboard", // "Mouse Keyboard"
                    Keyboard.current,
                    Mouse.current
                );
            }
        }
    }
}
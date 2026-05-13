using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class GameManager : MonoBehaviour
{
    public GameObject[] MainCharacterPrefabs;
    public GameObject[] SideCharacterPrefabs;
    public Transform[] spawnPoints;


    void Start()
    {
        
        int playerCount = LobbyManager.FinalPlayerCount;

        for (int i = 0; i < playerCount; i++)
        {
            GameObject mainObj = null;
            GameObject sideObj = null;

            // MAIN character
            int mainID = CharacterSelectManager.SelectedCharacter[i, 0];

            // SIDE character
            int sideID = CharacterSelectManager.SelectedCharacter[i, 1];

            int mainSpawnIndex = i * 2;
            int sideSpawnIndex = i * 2 + 1;

            // Spawn MAIN
            if (mainID >= 0 && mainID < MainCharacterPrefabs.Length)
            {
                mainObj = Instantiate(
                    MainCharacterPrefabs[mainID],
                    spawnPoints[mainSpawnIndex].position,
                    spawnPoints[mainSpawnIndex].rotation
                );

                PlayerInput mainInput = mainObj.GetComponentInChildren<PlayerInput>();

                if (mainInput == null)
                {
                    mainInput.user.UnpairDevices();

                    InputUser.PerformPairingWithDevice(
                        CharacterSelectManager1.PlayerDevices[i], mainInput.user);

                    mainInput.SwitchCurrentControlScheme(
                        "Mouse Keyboard",Keyboard.current, Mouse.current);
                }
                else if (CharacterSelectManager1.PlayerDevices[i] is Gamepad)
                {
                    mainInput.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
                }
            }

            // Spawn SIDE
            if (sideID >= 0 && sideID < SideCharacterPrefabs.Length)
            {
                sideObj = Instantiate(
                    SideCharacterPrefabs[sideID],
                    spawnPoints[sideSpawnIndex].position,
                    spawnPoints[sideSpawnIndex].rotation
                );
            }

            if (mainObj != null && sideObj != null)
            {
                SideMovements sideMovement =
                    sideObj.GetComponentInChildren<SideMovements>();

                if (sideMovement == null)
                {
                    Debug.LogError("SideMovements missing!");
                    continue;
                }

                sideMovement.assignedInput =
                    mainObj.GetComponentInChildren<PlayerInput>();

                sideMovement.InitializeInput();

                Canvas sideCanvas =
                    sideObj.GetComponentInChildren<Canvas>();

                Camera mainCam =
                    mainObj.GetComponentInChildren<Camera>();

                if (sideCanvas != null && mainCam != null)
                {
                    sideCanvas.worldCamera = mainCam;
                }
            }
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelectManager1 : MonoBehaviour
{
/////////////////////////////////////////////////////////////////////////////////
/// MANAGER PROPERTIES

    [Header("Manager Settings")]

    public static int[] SelectedCharacter = new int[3];

    public static InputDevice[] PlayerDevices = new InputDevice[3];

    private static bool[] isReady = new bool[3];

    public int CurrentIndex { get; set; } = 0;

    private int playerIndex;

    private float inputCooldown = 0.2f;

    private float lastMoveTime;

    private bool isLockedIn = false;

    private GameObject currentPreview;

/////////////////////////////////////////////////////////////////////////////////
/// UI PROPERTIES

    [Header("Visual Settings")]

    public GameObject[] characterDisplayPrefabs;

    public Transform mainPreviewPoint;

    public TMPro.TextMeshProUGUI readyString;
/////////////////////////////////////////////////////////////////////////////////
/// UI PROPERTIES

    [Header("Scene String Setting")]
    public string sceneStringTo = "SceneLegacy 1";

/////////////////////////////////////////////////////////////////////////////////
/// AWAKE

    void Awake()
    {
        isLockedIn = false;

        PlayerInput playerInput =
            GetComponent<PlayerInput>();

        playerIndex = playerInput.playerIndex;

        PlayerDevices[playerIndex] =
            playerInput.devices[0];

        if (playerIndex == 0)
        {
            for (int i = 0; i < isReady.Length; i++)
            {
                isReady[i] = false;

                SelectedCharacter[i] = 0;
            }
        }
    }

/////////////////////////////////////////////////////////////////////////////////
/// START

    void Start()
    {
        UpdateUI();

        UpdateReadyUI();
    }

/////////////////////////////////////////////////////////////////////////////////
/// NAVIGATION

    public void OnCS_Navigate(InputValue value)
    {
        if (isLockedIn)
            return;

        Vector2 input = value.Get<Vector2>();

        if (Time.time - lastMoveTime < inputCooldown)
            return;

        if (input.x > 0.5f)
        {
            CurrentIndex++;

            lastMoveTime = Time.time;
        }
        else if (input.x < -0.5f)
        {
            CurrentIndex--;

            lastMoveTime = Time.time;
        }

        int count = characterDisplayPrefabs.Length;

        if (count == 0)
            return;

        CurrentIndex =
            (CurrentIndex + count) % count;

        UpdateUI();
    }

/////////////////////////////////////////////////////////////////////////////////
/// SUBMIT

    public void OnCS_Submit()
    {
        if (isLockedIn)
            return;

        isLockedIn = true;

        SelectedCharacter[playerIndex] =
            CurrentIndex;

        isReady[playerIndex] = true;

        UpdateReadyUI();

        int readyCount = 0;

        for (int i = 0; i < isReady.Length; i++)
        {
            if (isReady[i])
            {
                readyCount++;
            }
        }

        int playerCount =
            FindFirstObjectByType<LobbyManager1>()
            .GetPlayerCount();

        if (playerCount >= 2 &&
            readyCount == playerCount)
        {
            SceneManager.LoadScene(sceneStringTo);
        }
    }

/////////////////////////////////////////////////////////////////////////////////
/// UI

    void UpdateUI()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }

        currentPreview = Instantiate(
            characterDisplayPrefabs[CurrentIndex],
            mainPreviewPoint.position,
            mainPreviewPoint.rotation
        );

        currentPreview.transform.SetParent(
            mainPreviewPoint
        );
    }

    void UpdateReadyUI()
    {
        if (isLockedIn)
        {
            readyString.text = "Ready";

            readyString.color = Color.green;
        }
        else
        {
            readyString.text = "Selecting...";

            readyString.color = Color.white;
        }
    }
}
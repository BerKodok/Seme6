using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
/////////////////////////////////////////////////////////////////////////////////
/// MANAGER PROPERTIES

    [Header("Manager Settings")]
    public static int[,] SelectedCharacter = new int[3, 2];
    public static InputDevice[] PlayerDevices = new InputDevice[3];
    private static bool[] isReady = new bool[3];
    public int CurrentIndex {get; set;} = 0;
    private int playerIndex;
    private float inputCooldown = 0.2f;
    private float lastMoveTime;
    private int currentSelectionSlot = 0;
    private bool isLockedIn = false;
    private GameObject currentMainPreview;
    private GameObject currentSidePreview;
    //private int justJoinIN = 0;
/////////////////////////////////////////////////////////////////////////////////
/// UI PROPERTIES 

    [Header("Visual Settings")]
    public GameObject[] characterDisplayPrefabs;

    public Transform mainPreviewPoint;
    public Transform sidePreviewPoint;
    public TMPro.TextMeshProUGUI readyString;

/////////////////////////////////////////////////////////////////////////////////
/// AWAKE
    
    void Awake()
    {
        //justJoinIN = 0;
        isLockedIn = false;
        //justJoinIN = false;
        playerIndex = GetComponent<PlayerInput>().playerIndex;
        PlayerDevices[playerIndex] = GetComponent<PlayerInput>().devices[0];

        if (playerIndex == 0)
        {
            isReady[0] = false;
            isReady[1] = false;
            isReady[2] = false;

            SelectedCharacter[0,0] = 0;
            SelectedCharacter[0,1] = 0;

            SelectedCharacter[1,0] = 0;
            SelectedCharacter[1,1] = 0;

            SelectedCharacter[2,0] = 0;
            SelectedCharacter[2,1] = 0;
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
/// NAVIGATION VOID
    public void OnCS_Navigate(InputValue value)
    {
        //Debug.Log(CurrentIndex);
        if (isLockedIn) return;

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
        if (count == 0) return;

        CurrentIndex = (CurrentIndex + count) % count;

        UpdateUI();
    }

/////////////////////////////////////////////////////////////////////////////////
/// CUSTOM VOIDS
    public void OnCS_Submit()
    {
        if (isLockedIn)
        {
            UpdateUI();
            return;
        }

        // Save current selection
        SelectedCharacter[playerIndex, currentSelectionSlot] = CurrentIndex;

        // Move to next selection slot
        currentSelectionSlot++;

        // Finished MAIN selection
        if (currentSelectionSlot == 1)
        {
            UpdateUI();
            UpdateReadyUI();
            return;
        }

        // Finished SIDE selection
        isLockedIn = true;

        isReady[playerIndex] = true;

        UpdateReadyUI();

        int readyCount = 0;

        for (int i = 0; i < isReady.Length; i++)
        {
            if (isReady[i]) readyCount++;
        }

        int playerCount = FindFirstObjectByType<LobbyManager>().GetPlayerCount();

        if (playerCount >= 2 && readyCount == playerCount)
        {
            SceneManager.LoadScene("SceneLegacy"); //Scence Change
        }
    }
/////////////////////////////////////////////////////////////////////////////////
/// UI BITS
    void UpdateUI()
    {
        // MAIN selection
        if (currentSelectionSlot == 0)
        {
            if (currentMainPreview != null)
            {
                Destroy(currentMainPreview);
            }

            currentMainPreview = Instantiate(
                characterDisplayPrefabs[CurrentIndex],
                mainPreviewPoint.position,
                mainPreviewPoint.rotation
            );

            currentMainPreview.transform.SetParent(mainPreviewPoint);
        }

        // SIDE selection
        else
        {
            if (currentSidePreview != null)
            {
                Destroy(currentSidePreview);
            }

            currentSidePreview = Instantiate(
                characterDisplayPrefabs[CurrentIndex],
                sidePreviewPoint.position,
                sidePreviewPoint.rotation
            );

            currentSidePreview.transform.SetParent(sidePreviewPoint);
        }
    }

    void UpdateReadyUI()
    {
        if (isLockedIn)
        {
            readyString.text = "Ready";
            readyString.color = Color.green;
        }
        else if (currentSelectionSlot == 0)
        {
            readyString.text = "Selecting Main...";
            readyString.color = Color.white;
        }
        else
        {
            readyString.text = "Selecting Side...";
            readyString.color = Color.yellow;
        }
    }
}
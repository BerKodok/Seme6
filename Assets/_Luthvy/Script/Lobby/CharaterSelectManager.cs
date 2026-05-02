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
    public static int[] SelectedCharacter = new int[4];
    private static bool[] isReady = new bool[4];
    public int CurrentIndex {get; set;} = 0;
    private int playerIndex;
    private float inputCooldown = 0.2f;
    private float lastMoveTime;
    
    private bool isLockedIn = false;
    //private int justJoinIN = 0;
/////////////////////////////////////////////////////////////////////////////////
/// UI PROPERTIES 

    [Header("Visual Settings")]
    public Sprite[] characterSprites;
    public Image characterPortrait;
    public TMPro.TextMeshProUGUI readyString;

/////////////////////////////////////////////////////////////////////////////////
/// AWAKE
    
    void Awake()
    {
        //justJoinIN = 0;
        isLockedIn = false;
        //justJoinIN = false;
        playerIndex = GetComponent<PlayerInput>().playerIndex;

        if (playerIndex == 0)
        {
            isReady[0] = false;
            isReady[1] = false;

            SelectedCharacter[0] = 0;
            SelectedCharacter[1] = 0;
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

        int count = characterSprites.Length;
        if (count == 0) return;

        CurrentIndex = (CurrentIndex + count) % count;

        UpdateUI();
    }

/////////////////////////////////////////////////////////////////////////////////
/// CUSTOM VOIDS
    public void OnCS_Submit()
    {
        // LOCK CHECK
        if (isLockedIn) {UpdateUI(); return;}
        isLockedIn = true;
        //justJoinIN++;
        //Debug.Log(justJoinIN);
        //

        SelectedCharacter[playerIndex] = CurrentIndex;
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
            SceneManager.LoadScene("SceneLegacy");
        }
    }
/////////////////////////////////////////////////////////////////////////////////
/// UI BITS
    void UpdateUI()
    {
        if (characterPortrait != null && characterSprites.Length != 0)
        {
            characterPortrait.sprite = characterSprites[CurrentIndex];
        }
    }

    void UpdateReadyUI()
    {
        if (isLockedIn) {readyString.text = "Ready";readyString.color = Color.limeGreen;}
        else {readyString.text = "Selecting..."; readyString.color = Color.white;}
    }
}
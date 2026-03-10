using Mirror;
using TMPro;
using UnityEngine;

public class MainMenuNet: MonoBehaviour
{

    [SerializeField] private TMP_InputField ipInput ;
    //public GameObject mainMenuUIs;

    public void MakeLobbyRoom()
    {
        NetworkManager.singleton.StartHost();
        Debug.Log("LOBBY");
    }

    public void JoinGame()
    {
        NetworkManager.singleton.networkAddress = ipInput.text;
        NetworkManager.singleton.StartClient();
    }

    public void StopGame()
    {
        NetworkManager.singleton.StopHost();
    }

    public void LeaveGame()
    {
        NetworkManager.singleton.StopClient();
    }

    void Awake()
    {
        ipInput.text = "localhost";
        //mainMenuUIs.SetActive(true);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // ?? Camera dan UI
    public CinemachineCamera MenuCam;
    public CinemachineCamera CharacterSelectCam;
    public GameObject MenuPanel;
    public GameObject OptionPanel;
    public GameObject[] HowToPlayPanel;
    public GameObject CharSelectCanvas;

    private int currentHowToPlayIndex = 0;


    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuBGM();
        }

        HideAllHowToPlayPanel();
    }


    // ?? fungsi klik
    void PlayClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }

    public void SwitchToMenu()
    {
        PlayClick();

        CameraManager.SwitchCamera(MenuCam);
        MenuPanel.SetActive(true);
        OptionPanel.SetActive(false);
        HideAllHowToPlayPanel();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuBGM();
        }
    }

    public void SwitchToOption()
    {
        PlayClick();

        OptionPanel.SetActive(true);
        MenuPanel.SetActive(false);
        HideAllHowToPlayPanel();

    }

    public void SwitchToHowToPlay()
    {
        PlayClick();

        MenuPanel.SetActive(false);
        currentHowToPlayIndex = 0;
        ShowHowToPlay(currentHowToPlayIndex);
    }

    public void NextHowToPlay()
    {
        PlayClick();
        currentHowToPlayIndex++;

        if (currentHowToPlayIndex >= HowToPlayPanel.Length)
            currentHowToPlayIndex = 0;

        ShowHowToPlay(currentHowToPlayIndex);
    }

    public void PreviousHowToPlay()
    {
        PlayClick();
        currentHowToPlayIndex--;
        if (currentHowToPlayIndex < 0)
            currentHowToPlayIndex = HowToPlayPanel.Length - 1;
        ShowHowToPlay(currentHowToPlayIndex);
    }

    void ShowHowToPlay(int index)
    {
        HideAllHowToPlayPanel();
        HowToPlayPanel[index].SetActive(true);
    }

    void HideAllHowToPlayPanel()
    {
        foreach (var panel in HowToPlayPanel)
        {
            panel.SetActive(false);
        }
    }

    public void PlayGame()
    {
        PlayClick();

        CameraManager.SwitchCamera(CharacterSelectCam);
        CharSelectCanvas.SetActive(true);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCharacterSelectBGM();
        }
    }

    public void BackMenu()
    {
        PlayClick();

        CameraManager.SwitchCamera(MenuCam);
        MenuPanel.SetActive(true);
        OptionPanel.SetActive(false);
        CharSelectCanvas.SetActive(false);
        HideAllHowToPlayPanel();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuBGM();
        }
    }

    public void QuitGame()
    {
        PlayClick();
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
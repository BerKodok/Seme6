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

    // ?? BGM
    public AudioSource bgmSource;
    public AudioClip mainMenuBGM;
    public AudioClip characterBGM;

    private Coroutine musicCoroutine;
    public float fadeDuration = 0.5f;

    // ?? SFX
    public AudioSource sfxSource;
    public AudioClip clickSFX;

    void Start()
    {
        bgmSource.clip = mainMenuBGM;
        bgmSource.volume = 1f;
        bgmSource.Play();

        HideAllHowToPlayPanel();
    }

    public void SwitchMusic(AudioClip newClip)
    {
        // Kalau clip sama, jangan ulang
        if (bgmSource.clip == newClip)
            return;

        // Stop coroutine sebelumnya
        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
        }

        // Jalankan coroutine baru
        musicCoroutine = StartCoroutine(FadeMusic(newClip));
    }

    IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = 1f;

        // Fade Out
        while (bgmSource.volume > 0)
        {
            bgmSource.volume -= Time.deltaTime / fadeDuration;

            if (bgmSource.volume < 0)
                bgmSource.volume = 0;

            yield return null;
        }

        // Ganti musik
        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();

        // Reset volume
        bgmSource.volume = 0;

        // Fade In
        while (bgmSource.volume < startVolume)
        {
            bgmSource.volume += Time.deltaTime / fadeDuration;

            if (bgmSource.volume > startVolume)
                bgmSource.volume = startVolume;

            yield return null;
        }
    }

    // ?? fungsi klik
    void PlayClick()
    {
        sfxSource.PlayOneShot(clickSFX);
    }

    public void SwitchToMenu()
    {
        PlayClick();

        CameraManager.SwitchCamera(MenuCam);
        MenuPanel.SetActive(true);
        OptionPanel.SetActive(false);
        HideAllHowToPlayPanel();
        SwitchMusic(mainMenuBGM);
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
        SwitchMusic(characterBGM);
    }

    public void BackMenu()
    {
        PlayClick();

        CameraManager.SwitchCamera(MenuCam);
        MenuPanel.SetActive(true);
        OptionPanel.SetActive(false);
        CharSelectCanvas.SetActive(false);
        HideAllHowToPlayPanel();

        SwitchMusic(mainMenuBGM);
    }

    public void QuitGame()
    {
        PlayClick();
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
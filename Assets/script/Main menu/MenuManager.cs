using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public CinemachineCamera MenuCam;
    public CinemachineCamera CharacterSelectCam;

    // ?? BGM
    public AudioSource bgmSource;
    public AudioClip mainMenuBGM;
    public AudioClip characterBGM;

    public float fadeDuration = 0.5f;

    // ?? SFX
    public AudioSource sfxSource;
    public AudioClip clickSFX;

    void Start()
    {
        bgmSource.clip = mainMenuBGM;
        bgmSource.volume = 1f;
        bgmSource.Play();
    }

    public void SwitchMusic(AudioClip newClip)
    {
        StartCoroutine(FadeMusic(newClip));
    }

    IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = bgmSource.volume;

        while (bgmSource.volume > 0)
        {
            bgmSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();

        while (bgmSource.volume < startVolume)
        {
            bgmSource.volume += startVolume * Time.deltaTime / fadeDuration;
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
        SwitchMusic(mainMenuBGM);
    }

    public void SwitchToOption()
    {
        PlayClick();

        CameraManager.SwitchCamera(CharacterSelectCam);
        SwitchMusic(characterBGM);
    }

    public void PlayGame()
    {
        PlayClick();
        SceneManager.LoadScene("InGame");
    }

    public void QuitGame()
    {
        PlayClick();
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioClip mainMenuBGM;
    public AudioClip characterBGM;

    void Start()
    {
        PlayMainMenu();
    }

    public void PlayMainMenu()
    {
        bgmSource.clip = mainMenuBGM;
        bgmSource.Play();
    }

    public void PlayCharacter()
    {
        bgmSource.Stop();
        bgmSource.clip = characterBGM;
        bgmSource.Play();
    }
}
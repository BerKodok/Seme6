using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public Sound[] musicSound, sfxPlayer;
    public AudioSource MainBGMSource, sfxPlayerr;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSound, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            MainBGMSource.clip = s.clip;
            SetMusicVolume(name, s.volume);
            MainBGMSource.Play();
        }
    }

    public void SetMusicVolume(string name, float volume)
    {
        Sound s = Array.Find(musicSound, x => x.name == name);

        if (s != null)
        {
            s.volume = Mathf.Clamp(volume, 0f, 1f);
            if (MainBGMSource.clip == s.clip)
            {
                MainBGMSource.volume = s.volume;
            }
        }
    }


    public void SFXPlayer(string name)
    {
        Sound s = Array.Find(sfxPlayer, x => x.name == name);

        if (s != null)
            sfxPlayerr.PlayOneShot(s.clip, s.volume);
    }

}

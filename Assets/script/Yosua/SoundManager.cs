using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public Sound[] musicSound, sfxPlayer;
    public AudioSource MainMenu, sfxPlayerr, CharacterSelection, Gameplay, Boost;

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
            MainMenu.clip = s.clip;
            SetMusicVolume(name, s.volume);
            MainMenu.Play();
        }
    }

    public void SetMusicVolume(string name, float volume)
    {
        Sound s = Array.Find(musicSound, x => x.name == name);

        if (s != null)
        {
            s.volume = Mathf.Clamp(volume, 0f, 1f);
            if (MainMenu.clip == s.clip)
            {
                MainMenu.volume = s.volume;
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

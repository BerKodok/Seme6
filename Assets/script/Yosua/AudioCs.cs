using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource uiSource;
    public AudioSource playerSFXSource;
    public AudioSource footstepSource;
    public AudioSource environmentSource;

    [Header("BGM")]
    public AudioClip mainMenuBGM;
    public AudioClip characterSelectBGM;
    public AudioClip gameplayBGM;

    [Header("UI SFX")]
    public AudioClip countdownSFX;
    public AudioClip buttonClickSFX;

    [Header("Player SFX")]
    public AudioClip sprintFootstepSFX;
    public AudioClip boostSFX;
    public AudioClip fallDownSFX;

    [Header("Environment SFX")]
    public AudioClip crowdSFX;

    [Header("Volume")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float environmentVolume = 1f;

    [Header("Fade")]
    public float fadeDuration = 0.5f;

    private Coroutine musicCoroutine;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        DebugAudioSetup();
        PlayAudioByScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayAudioByScene(scene.name);
    }

    private void PlayAudioByScene(string sceneName)
    {
        Debug.Log("Scene aktif sekarang: " + sceneName);

        if (sceneName == "Main Menu")
        {
            Debug.Log("AudioManager: Play Main Menu BGM");
            StopCrowd();
            PlayMainMenuBGM();
        }
        else if (sceneName == "Gameplay")
        {
            Debug.Log("AudioManager: Play Gameplay BGM + Crowd");
            PlayGameplayBGM();
            PlayCrowd();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
        }
        else
        {
            Debug.LogWarning("AudioManager duplicate dihancurkan: " + gameObject.name);
            Destroy(gameObject);
        }
    }


    private void SetupAudioSources()
    {
        SetupSource(musicSource, true, musicVolume);
        SetupSource(uiSource, false, sfxVolume);
        SetupSource(playerSFXSource, false, sfxVolume);
        SetupSource(footstepSource, true, sfxVolume);
        SetupSource(environmentSource, true, environmentVolume);
    }

    private void SetupSource(AudioSource source, bool loop, float volume)
    {
        if (source == null) return;

        source.playOnAwake = false;
        source.loop = loop;
        source.volume = volume;
        source.spatialBlend = 0f;
        source.mute = false;
    }

    private void DebugAudioSetup()
    {
        Debug.Log("=== AUDIO MANAGER CHECK ===");

        if (musicSource == null) Debug.LogError("Music Source belum di-assign di AudioManager!");
        if (uiSource == null) Debug.LogError("UI Source belum di-assign di AudioManager!");
        if (playerSFXSource == null) Debug.LogError("Player SFX Source belum di-assign di AudioManager!");
        if (footstepSource == null) Debug.LogError("Footstep Source belum di-assign di AudioManager!");
        if (environmentSource == null) Debug.LogError("Environment Source belum di-assign di AudioManager!");

        if (mainMenuBGM == null) Debug.LogWarning("Main Menu BGM kosong.");
        if (characterSelectBGM == null) Debug.LogWarning("Character Select BGM kosong.");
        if (gameplayBGM == null) Debug.LogWarning("Gameplay BGM kosong.");
        if (countdownSFX == null) Debug.LogWarning("Countdown SFX kosong.");
        if (buttonClickSFX == null) Debug.LogWarning("Button Click SFX kosong.");
        if (sprintFootstepSFX == null) Debug.LogWarning("Sprint Footstep SFX kosong.");
        if (boostSFX == null) Debug.LogWarning("Boost SFX kosong.");
        if (fallDownSFX == null) Debug.LogWarning("Fall Down SFX kosong.");
        if (crowdSFX == null) Debug.LogWarning("Crowd SFX kosong.");
    }

    // =========================
    // BGM
    // =========================

    public void PlayMainMenuBGM()
    {
        PlayMusic(mainMenuBGM, "Main Menu BGM");
    }

    public void PlayCharacterSelectBGM()
    {
        PlayMusic(characterSelectBGM, "Character Select BGM");
    }

    public void PlayGameplayBGM()
    {
        PlayMusic(gameplayBGM, "Gameplay BGM");
    }

    public void PlayMusic(AudioClip newClip, string clipName = "Music")
    {
        if (musicSource == null)
        {
            Debug.LogError("Gagal play " + clipName + ": musicSource NULL!");
            return;
        }

        if (newClip == null)
        {
            Debug.LogError("Gagal play " + clipName + ": clip NULL!");
            return;
        }

        if (musicSource.clip == newClip && musicSource.isPlaying)
        {
            Debug.Log(clipName + " sudah sedang play.");
            return;
        }

        Debug.Log("Play Music: " + clipName);

        if (musicCoroutine != null)
            StopCoroutine(musicCoroutine);

        if (fadeDuration <= 0f)
        {
            musicSource.Stop();
            musicSource.clip = newClip;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
            return;
        }

        musicCoroutine = StartCoroutine(FadeToMusic(newClip));
    }

    private IEnumerator FadeToMusic(AudioClip newClip)
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0f)
        {
            musicSource.volume -= startVolume * Time.unscaledDeltaTime / fadeDuration;

            if (musicSource.volume < 0f)
                musicSource.volume = 0f;

            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        while (musicSource.volume < musicVolume)
        {
            musicSource.volume += musicVolume * Time.unscaledDeltaTime / fadeDuration;

            if (musicSource.volume > musicVolume)
                musicSource.volume = musicVolume;

            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
    }

    // =========================
    // UI SFX
    // =========================

    public void PlayButtonClick()
    {
        PlayUISFX(buttonClickSFX, "Button Click SFX");
    }

    public void PlayCountdown()
    {
        PlayUISFX(countdownSFX, "Countdown SFX");
    }

    public void PlayUISFX(AudioClip clip, string clipName = "UI SFX")
    {
        if (uiSource == null)
        {
            Debug.LogError("Gagal play " + clipName + ": uiSource NULL!");
            return;
        }

        if (clip == null)
        {
            Debug.LogError("Gagal play " + clipName + ": clip NULL!");
            return;
        }

        Debug.Log("Play UI SFX: " + clipName);
        uiSource.PlayOneShot(clip, sfxVolume);
    }

    // =========================
    // PLAYER SFX
    // =========================

    public void PlayBoost(float volume = 1f)
    {
        if (playerSFXSource == null)
        {
            Debug.LogError("Gagal play Boost: playerSFXSource NULL!");
            return;
        }

        if (boostSFX == null)
        {
            Debug.LogError("Gagal play Boost: boostSFX NULL!");
            return;
        }

        Debug.Log("Play Boost SFX");
        playerSFXSource.PlayOneShot(boostSFX, sfxVolume * volume);
    }

    public void PlayFallDown(float volume = 1f)
    {
        if (playerSFXSource == null)
        {
            Debug.LogError("Gagal play FallDown: playerSFXSource NULL!");
            return;
        }

        if (fallDownSFX == null)
        {
            Debug.LogError("Gagal play FallDown: fallDownSFX NULL!");
            return;
        }

        Debug.Log("Play FallDown SFX");
        playerSFXSource.PlayOneShot(fallDownSFX, sfxVolume * volume);
    }

    public void StartFootstepLoop(float volume = 1f)
    {
        if (footstepSource == null)
        {
            Debug.LogError("Gagal play Footstep: footstepSource NULL!");
            return;
        }

        if (sprintFootstepSFX == null)
        {
            Debug.LogError("Gagal play Footstep: sprintFootstepSFX NULL!");
            return;
        }

        if (footstepSource.clip != sprintFootstepSFX)
            footstepSource.clip = sprintFootstepSFX;

        footstepSource.volume = sfxVolume * volume;
        footstepSource.loop = true;

        if (!footstepSource.isPlaying)
        {
            Debug.Log("Play Footstep Loop");
            footstepSource.Play();
        }
    }

    public void StopFootstepLoop()
    {
        if (footstepSource == null) return;

        if (footstepSource.isPlaying)
        {
            Debug.Log("Stop Footstep Loop");
            footstepSource.Stop();
        }
    }

    // =========================
    // ENVIRONMENT / CROWD
    // =========================

    public void PlayCrowd()
    {
        if (environmentSource == null)
        {
            Debug.LogError("Gagal play Crowd: environmentSource NULL!");
            return;
        }

        if (crowdSFX == null)
        {
            Debug.LogError("Gagal play Crowd: crowdSFX NULL!");
            return;
        }

        if (environmentSource.clip != crowdSFX)
            environmentSource.clip = crowdSFX;

        environmentSource.loop = true;
        environmentSource.volume = environmentVolume;

        if (!environmentSource.isPlaying)
        {
            Debug.Log("Play Crowd SFX");
            environmentSource.Play();
        }
    }

    public void StopCrowd()
    {
        if (environmentSource == null) return;

        if (environmentSource.isPlaying)
        {
            Debug.Log("Stop Crowd SFX");
            environmentSource.Stop();
        }
    }
}
using UnityEngine;

public class PlayerSFXSystem : MonoBehaviour
{
    [Header("Reference")]
    public RunController runController;

    [Header("SFX Clips")]
    public AudioClip sprintFootstepSFX;
    public AudioClip boostSFX;
    public AudioClip fallDownSFX;

    [Header("Settings")]
    public float sprintSpeedThreshold = 1f;
    public float sprintStepInterval = 0.35f;
    public float sprintVolume = 1f;
    public float boostVolume = 1f;
    public float fallDownVolume = 1f;

    private AudioSource footstepSource;
    private AudioSource sfxSource;

    private float footstepTimer;
    private bool wasDashing;
    private bool wasStunned;

    void Start()
    {
        if (runController == null)
            runController = GetComponent<RunController>();

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.playOnAwake = false;
        footstepSource.loop = false;
        footstepSource.spatialBlend = 0f;
        footstepSource.volume = sprintVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f;
        sfxSource.volume = 1f;

        // TEST AUDIO SAAT GAME MULAI
        Invoke(nameof(TestAudio), 1f);
    }

    void TestAudio()
    {
        Debug.Log("TEST AUDIO DIPANGGIL");

        if (sprintFootstepSFX == null)
        {
            Debug.LogWarning("Sprint Footstep SFX kosong!");
            return;
        }

        footstepSource.PlayOneShot(sprintFootstepSFX, 1f);
    }

    void Update()
    {
        if (runController == null)
        {
            Debug.LogWarning("RunController belum keisi!");
            return;
        }

        HandleSprintFootstep();
        HandleBoostSFX();
        HandleFallDownSFX();
    }

    void HandleSprintFootstep()
    {
        bool canPlay =
            !runController.IsDashing &&
            !runController.IsStunned &&
            runController.CurrentSpeed >= sprintSpeedThreshold;

        if (!canPlay)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            if (sprintFootstepSFX == null)
            {
                Debug.LogWarning("Sprint Footstep SFX kosong!");
                return;
            }

            Debug.Log("FOOTSTEP PLAY | Speed: " + runController.CurrentSpeed);

            footstepSource.PlayOneShot(sprintFootstepSFX, sprintVolume);
            footstepTimer = sprintStepInterval;
        }
    }

    void HandleBoostSFX()
    {
        if (runController.IsDashing && !wasDashing)
        {
            Debug.Log("BOOST PLAY");

            if (boostSFX != null)
                sfxSource.PlayOneShot(boostSFX, boostVolume);
        }

        wasDashing = runController.IsDashing;
    }

    void HandleFallDownSFX()
    {
        if (runController.IsStunned && !wasStunned)
        {
            Debug.Log("FALLDOWN PLAY");

            if (fallDownSFX != null)
                sfxSource.PlayOneShot(fallDownSFX, fallDownVolume);
        }

        wasStunned = runController.IsStunned;
    }
}
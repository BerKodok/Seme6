using UnityEngine;

public class Footstep : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip[] footstepSounds;

    public void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;

        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];

        footstepSource.PlayOneShot(clip);
    }
}
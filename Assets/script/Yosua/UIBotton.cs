using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioClip clickSFX;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            sfxSource.PlayOneShot(clickSFX);
        });
    }
}
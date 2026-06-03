using System.Collections;
using UnityEngine;

public class GameplayAudioStarter : MonoBehaviour
{
    public bool playGameplayBGM = true;
    public bool playCrowd = true;

    IEnumerator Start()
    {
        yield return null;

        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance NULL! Pastikan _AudioManager ada di scene ini atau kebawa dari Main Menu.");
            yield break;
        }

        Debug.Log("GameplayAudioStarter jalan.");

        if (playGameplayBGM)
        {
            AudioManager.Instance.PlayGameplayBGM();
        }

        if (playCrowd)
        {
            AudioManager.Instance.PlayCrowd();
        }
    }

    void OnDestroy()
    {
        if (AudioManager.Instance == null) return;

        AudioManager.Instance.StopCrowd();
    }
}
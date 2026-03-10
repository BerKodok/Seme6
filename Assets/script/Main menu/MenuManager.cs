using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public CinemachineCamera MenuCam;
    public CinemachineCamera CharacterSelectCam;

    // Fungsi ini dipanggil lewat UI Button
    public void SwitchToMenu()
    {
        CameraManager.SwitchCamera(MenuCam);
    }

    public void SwitchToOption()
    {
        CameraManager.SwitchCamera(CharacterSelectCam);
    }
 
    public void PlayGame()
    {
        SceneManager.LoadScene("InGame");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
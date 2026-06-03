using UnityEngine;
using UnityEngine.SceneManagement;

public class back : MonoBehaviour
{
    public void LoadScene2()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
using UnityEngine;

public class BindAllCanvasToCamera : MonoBehaviour
{
    public Camera playerCamera;

    void Start()
    {
        Canvas[] canvases = GetComponentsInChildren<Canvas>(true);

        foreach (Canvas c in canvases)
        {
            c.renderMode = RenderMode.ScreenSpaceCamera;
            c.worldCamera = playerCamera;
        }
    }
}
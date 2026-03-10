using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public RaceManager raceManager;

    private void OnTriggerEnter(Collider other)
    {
        RunController controller = other.GetComponent<RunController>();

        if (controller != null)
        {
            raceManager.PlayerFinished(controller);
        }
    }
}
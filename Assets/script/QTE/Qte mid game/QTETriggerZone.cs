using UnityEngine;

public class QTETriggerZone : MonoBehaviour
{
    [Header("Settings")]
    public bool triggerOnce = true;

    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && alreadyTriggered) return;
        if (!other.CompareTag("Player")) return;

        // ambil script QTE dari player
        StaminaQTETrigger qte = other.GetComponent<StaminaQTETrigger>();

        if (qte != null)
        {
            qte.StartQTE();
            alreadyTriggered = true;

            Debug.Log("QTE DIPANGGIL dari trigger");
        }
        else
        {
            Debug.LogError("StaminaQTETrigger tidak ditemukan di Player!");
        }
    }
}
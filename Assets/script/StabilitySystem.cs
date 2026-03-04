using UnityEngine;
using UnityEngine.UI;

public class StabilitySystem : MonoBehaviour
{

    public float safeZone = 5f;

    [Header("UI")]
    public Slider stabilitySlider;

    [Header("Stability Settings")]
    public float driftSpeed = 15f;      // seberapa cepat slider bergerak oto
    public float controlSpeed = 40f;    // seberapa kuat analog mengoreksi
    public float maxOffset = 40f;       // batas sebelum dianggap kehilangan kontrol

    private float driftDirection = 1f;

    public float CurrentOffset => stabilitySlider.value - 50f;
    public bool IsUnstable => Mathf.Abs(CurrentOffset) > maxOffset;

    void Start()
    {
        stabilitySlider.value = 50f;
        driftDirection = Random.Range(0, 2) == 0 ? -1 : 1;
    }

    public void UpdateStability(float analogInput)
    {
        
        stabilitySlider.value += driftDirection * driftSpeed * Time.deltaTime;

        
        stabilitySlider.value += analogInput * controlSpeed * Time.deltaTime;

        
        stabilitySlider.value = Mathf.Clamp(stabilitySlider.value, 0f, 100f);

        
        if (Random.value < 0.01f)
            driftDirection *= -1f;
    }
    public float TurnDirection
    {
        get
        {
            if (stabilitySlider.value < 50f) return -1f;
            if (stabilitySlider.value > 50f) return 1f;
            return 0f;
        }
    }

    public float DistanceFromCenter
    {
        get
        {
            return Mathf.Abs(stabilitySlider.value - 50f) / 50f;
            // 0 - 1
        }
    }

    public void ResetToCenter(float resetSpeed)
    {
        stabilitySlider.value = Mathf.Lerp(
            stabilitySlider.value,
            50f,
            resetSpeed * Time.deltaTime
        );
    }
    public void SnapToCenter()
    {
        stabilitySlider.value = 50f;
    }
}
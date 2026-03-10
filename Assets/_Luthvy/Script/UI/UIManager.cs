using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Reference")]
    public RunController player;

    [Header("Sliders")]
    public Slider staminaSlider;
    public Slider speedSlider;

    [Header("Dash Icon")]
    public RawImage dashIcon;
    public Color dashReadyColor = Color.white;
    public Color dashCooldownColor = Color.black;

    [Header("Speed Settings")]
    public float normalSpeedometerMax = 100f;
    public float dashSpeedometerMax = 200f;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("UIManager: Player reference missing!");
            return;
        }

        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = player.MaxStamina;
        }

        if (speedSlider != null)
        {
            speedSlider.minValue = 0f;
            speedSlider.maxValue = normalSpeedometerMax;
        }
    }

    void Update()
    {
        if (player == null) return;

        UpdateStamina();
        UpdateSpeed();
        UpdateDashIcon();
    }

    void UpdateStamina()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = player.CurrentStamina;
        }
    }

    void UpdateSpeed()
    {
        if (speedSlider == null) return;

        float currentMax = player.IsDashing ? dashSpeedometerMax : normalSpeedometerMax;

        speedSlider.maxValue = currentMax;

        speedSlider.value = player.CurrentSpeed;
    }

    void UpdateDashIcon()
    {
        if (dashIcon == null) return;

        dashIcon.color = player.IsDashing
            ? dashCooldownColor
            : dashReadyColor;
    }
}
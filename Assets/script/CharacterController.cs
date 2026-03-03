using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class RunController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 8f;
    public float maxSprintSpeed = 25f;
    public float sideSpeed = 8f;
    public float gravity = -25f;
    public float speedSmooth = 12f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaCostPerCombo = 15f;
    public float staminaRegen = 20f;

    [Header("Stack Boost")]
    public float boostPercentPerStack = 0.08f;
    public int maxStacks = 10;
    public float stackDecayDelay = 1f;
    public float stackDecayInterval = 0.5f;
    public int stackDecayAmount = 2;

    [Header("Boost Effect")]
    public ParticleSystem boostParticle;
    public Camera playerCamera;
    public float boostFOV = 75f;
    public float normalFOV = 60f;
    public float fovLerpSpeed = 6f;

    [Header("Dash")]
    public float dashBonusPercentPerStack = 0.10f;
    public float dashDuration = 2f;
    public float dashCooldown = 3f;

    [Header("OverSpeed Stun")]
    public float stunDuration = 2f;
    public Animator animator;

    [Header("Dash Camera Effect")]
    public float dashCameraBackDistance = 1.5f;
    public float cameraMoveSpeed = 5f;

    private CharacterController controller;
    private PlayerInputActions input;

    private Vector2 moveInput;
    private float currentSpeed;
    private float targetSpeed;
    private float currentStamina;
    private float yVelocity;

    private int currentStacks = 0;
    private float decayTimer;

    private bool waitingForL1 = false;
    public float comboWindow = 0.5f;
    private float comboTimer;

    private bool isDashing = false;
    private float dashTimer;
    private float dashCooldownTimer;

    private bool isStunned = false;
    private float stunTimer;

    private float normalMaxSpeed;
    private float dashMaxSpeed;

    private Vector3 originalCamLocalPos;

    private float normalSpeedometerMax = 100f;
    private float dashSpeedometerMax = 200f;
    private float currentSpeedometerMax;

    public float CurrentSpeed => currentSpeed;
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public bool IsDashing => isDashing;
    public float DashCooldownTimer => dashCooldownTimer;
    public float CurrentSpeedometerMax => currentSpeedometerMax;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.Dash.performed += ctx => StartDash();
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        input.Player.R1.performed += ctx => StartCombo();
        input.Player.L1.performed += ctx => ExecuteCombo();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        normalMaxSpeed = maxSprintSpeed;
        dashMaxSpeed = maxSprintSpeed * 2f;

        currentSpeed = walkSpeed;
        currentStamina = maxStamina;

        currentSpeedometerMax = normalSpeedometerMax;

        if (playerCamera != null)
            originalCamLocalPos = playerCamera.transform.localPosition;
    }

    void Update()
    {
        HandleComboWindow();
        HandleStackDecay();
        UpdateSpeed();
        RegenerateStamina();
        ApplyGravity();
        Move();
        HandleFOV();
        UpdateDash();
        HandleDashCamera();
        CheckOverSpeed();
        UpdateStun();
    }

    void StartCombo()
    {
        waitingForL1 = true;
        comboTimer = comboWindow;
    }

    void ExecuteCombo()
    {
        if (!waitingForL1) return;
        waitingForL1 = false;

        if (currentStamina < staminaCostPerCombo)
            return;

        currentStamina -= staminaCostPerCombo;

        currentStacks = Mathf.Clamp(currentStacks + 1, 0, maxStacks);
        decayTimer = stackDecayDelay;

        if (boostParticle != null)
            boostParticle.Play();
    }

    void HandleComboWindow()
    {
        if (!waitingForL1) return;

        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0f)
            waitingForL1 = false;
    }

    void HandleStackDecay()
    {
        if (currentStacks <= 0) return;

        decayTimer -= Time.deltaTime;

        if (decayTimer <= 0f)
        {
            currentStacks = Mathf.Clamp(currentStacks - stackDecayAmount, 0, maxStacks);
            decayTimer = stackDecayInterval;
        }
    }

    void UpdateSpeed()
    {
        float stackMultiplier = boostPercentPerStack * currentStacks;
        float dashMultiplier = isDashing ? dashBonusPercentPerStack * currentStacks : 0f;

        float totalMultiplier = 1f + stackMultiplier + dashMultiplier;
        float currentMaxSpeed = isDashing ? dashMaxSpeed : normalMaxSpeed;

        targetSpeed = Mathf.Clamp(walkSpeed * totalMultiplier, walkSpeed, currentMaxSpeed);

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedSmooth);
    }

    void RegenerateStamina()
    {
        if (currentStacks != 0) return;

        currentStamina += staminaRegen * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && yVelocity < 0)
            yVelocity = -2f;

        yVelocity += gravity * Time.deltaTime;
    }

    void Move()
    {
        if (isStunned)
        {
            controller.Move(Vector3.zero);
            return;
        }

        Vector3 move =
            transform.forward * currentSpeed +
            transform.right * moveInput.x * sideSpeed;

        Vector3 velocity = move + Vector3.up * yVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleFOV()
    {
        if (playerCamera == null) return;

        float targetFOV = currentStacks > 0 ? boostFOV : normalFOV;

        playerCamera.fieldOfView =
            Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
    }

    void StartDash()
    {
        if (dashCooldownTimer > 0f) return;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        currentSpeedometerMax = dashSpeedometerMax;
    }

    void UpdateDash()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (!isDashing) return;

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            isDashing = false;
            currentSpeedometerMax = normalSpeedometerMax;
            currentSpeed = normalMaxSpeed * 0.8f;
        }
    }

    void HandleDashCamera()
    {
        if (playerCamera == null) return;

        Vector3 targetPos = isDashing
            ? originalCamLocalPos - new Vector3(0, 0, dashCameraBackDistance)
            : originalCamLocalPos;

        playerCamera.transform.localPosition =
            Vector3.Lerp(playerCamera.transform.localPosition,
                         targetPos,
                         Time.deltaTime * cameraMoveSpeed);
    }

    void CheckOverSpeed()
    {
        if (isStunned || isDashing) return;

        if (currentSpeed >= currentSpeedometerMax - 0.1f)
            EnterStun();
    }

    void EnterStun()
    {
        isStunned = true;
        stunTimer = stunDuration;
        currentStacks = 0;
        currentSpeed = 0;

        if (animator != null)
            animator.SetTrigger("Stun");
    }

    void UpdateStun()
    {
        if (!isStunned) return;

        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0f)
            isStunned = false;
    }
}
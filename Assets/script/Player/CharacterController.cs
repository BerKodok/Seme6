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


    [Header("Animator")]
    public Animator PlayerAnimator;
    public Animator SecondaryAnimator; 

    [Header("Dash Camera Effect")]
    public float dashCameraBackDistance = 1.5f;
    public float cameraMoveSpeed = 5f;

    [Header("Stability")]
    public StabilitySystem stabilitySystem;
    public float unstableTurnStrength = 5f;

    [Header("Advanced Stability")]
    public float turnAcceleration = 10f;   // seberapa cepat banting makin kuat
    public float maxTurnForce = 15f;       // batas maksimal banting
    public float turnRecoverySpeed = 8f;   // seberapa cepat pulih kalau kembali ke tengah

    [Header("QTE Trigger")]
    public float qteDelay = 5f;
    public StaminaQTE staminaQTE;

    [Header("Player SFX Volume")]
    public float sprintFootstepVolume = 1f;
    public float boostSFXVolume = 1f;
    public float fallDownSFXVolume = 1f;

    [Header("Footstep Timing")]
    public float footstepInterval = 0.35f;
    private float footstepTimer = 0f;

    [Header("Race / Countdown")]
    public bool countdownFinished = false;
    public float minFootstepSpeed = 7f;


    private bool fallDownSFXPlayed = false;

    [Header("Footstep Animation Sync")]
    [Range(0f, 1f)] public float leftFootStepTime = 0.18f;
    [Range(0f, 1f)] public float rightFootStepTime = 0.68f;


    private int lastFootstepLoop = -1;
    private bool leftFootPlayed = false;
    private bool rightFootPlayed = false;


    private float qteTimer;
    private bool qteTriggered = false;


    private float currentTurnForce = 0f;

    public bool squareLocked = false;
    private float squareLockTimer = 0f; //tombol kotak


    public bool boostReady = true;
    private float boostCooldownTimer = 0f;

    private WallBounce wallBounce;

    public GameObject boostIndicator;

    private bool boostLocked = false;
    private float boostLockTimer = 0f;

    // ADDED
    public bool canUseBoost = false;

    private CharacterController controller;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction dashAction;
    private InputAction r1Action;
    private InputAction l1Action;

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

    private bool boostOnCooldown = false;

    private Vector3 originalCamLocalPos;

    private float normalSpeedometerMax = 100f;
    private float dashSpeedometerMax = 200f;
    private float currentSpeedometerMax;

    public float CurrentSpeed => currentSpeed;
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public bool IsDashing => isDashing;
    public bool IsStunned => isStunned;
    public float DashCooldownTimer => dashCooldownTimer;
    public float CurrentSpeedometerMax => currentSpeedometerMax;

    public PointerController pointerController;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        dashAction = playerInput.actions["Dash"];
        r1Action = playerInput.actions["R1"];
        l1Action = playerInput.actions["L1"];

        wallBounce = GetComponent<WallBounce>();
    }

    void OnEnable()
    {
        dashAction.performed += OnDashPressed;

        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;

        r1Action.performed += ctx => StartCombo();
        l1Action.performed += ctx => ExecuteCombo();

        // hidupkan animator lagi
        if (PlayerAnimator != null)
        {
            PlayerAnimator.speed = 1f;
        }

        if (SecondaryAnimator != null)
        {
            SecondaryAnimator.speed = 1f;
        }
    }
    void OnDashPressed(InputAction.CallbackContext ctx)
    {
        // jika boost sedang dikunci karena QTE gagal
        if (boostLocked)
        {
            Debug.Log("Boost masih cooldown dari QTE!");
            return;
        }

        // cooldown dash normal
        if (!boostReady || !canUseBoost) return;

        boostReady = false;
        boostCooldownTimer = dashCooldown;

        StartDash();

        if (boostIndicator != null)
            boostIndicator.SetActive(false);
    }

    void OnDisable()
    {
        dashAction.performed -= OnDashPressed;

        StopFootstepSFX();

        // stop animator utama
        if (PlayerAnimator != null)
        {
            PlayerAnimator.speed = 0f;
        }

        // stop animator kedua
        if (SecondaryAnimator != null)
        {
            SecondaryAnimator.speed = 0f;
        }
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

        if (boostIndicator != null)
            boostIndicator.SetActive(true);

        // INIT POINTER QTE
        if (pointerController != null)
        {
            pointerController.Init(playerInput);
        }

        // INIT STAMINA QTE
        if (staminaQTE != null)
        {
            staminaQTE.Init(playerInput);
        }

        qteTimer = qteDelay;

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
        UpdateWalkAnimation();
        HandlePlayerSFX();

        if (squareLocked)
        {
            squareLockTimer -= Time.deltaTime;

            if (squareLockTimer <= 0)
            {
                squareLocked = false;
            }
        }

        if (boostLocked)
        {
            boostLockTimer -= Time.deltaTime;

            int display = Mathf.CeilToInt(boostLockTimer);
            Debug.Log("Boost timer: " + display);

            if (boostLockTimer <= 0f)
            {
                boostLocked = false;
                canUseBoost = true;

                Debug.Log("BOOST READY");

                if (boostIndicator != null)
                    boostIndicator.SetActive(true);
            }
        }
        if (!boostReady)
{
            boostCooldownTimer -= Time.deltaTime;

            if (boostCooldownTimer <= 0f)
            {
                boostReady = true;

                // hanya hidupkan indikator jika tidak sedang QTE cooldown
                if (!boostLocked && boostIndicator != null)
                    boostIndicator.SetActive(true);
            }
        }
        Debug.Log(moveInput);

        if (!qteTriggered)
        {
            qteTimer -= Time.deltaTime;

            if (qteTimer <= 0f)
            {
                qteTriggered = true;

                if (staminaQTE != null)
                {
                    Debug.Log("Trigger QTE dari RunController");

                    staminaQTE.StartQTE();
                }
                else
                {
                    Debug.LogError("StaminaQTE belum di-assign!");
                }
            }
        }
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

        //kalau speed 0 bakal diem
        if (currentSpeed <= 0.1f)
        {
            stabilitySystem.SnapToCenter();
            currentTurnForce = 0f;
            controller.Move(Vector3.zero);
            return;
        }

        stabilitySystem.UpdateStability(moveInput.x);

        float direction = stabilitySystem.TurnDirection;
        float distance = stabilitySystem.DistanceFromCenter;

        if (direction != 0)
        {
            currentTurnForce += direction * turnAcceleration * distance * Time.deltaTime;
        }
        else
        {
            currentTurnForce = Mathf.Lerp(currentTurnForce, 0f, turnRecoverySpeed * Time.deltaTime);
        }

        currentTurnForce = Mathf.Clamp(currentTurnForce, -maxTurnForce, maxTurnForce);

        Vector3 move =
            transform.forward * currentSpeed +
            transform.right * currentTurnForce;

        if (wallBounce != null)
        {
            wallBounce.SetMoveDirection(move.normalized);
        }

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

    public void StartDash()
    {
        if (boostLocked) return;

        if (dashCooldownTimer > 0f) return;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        currentSpeedometerMax = dashSpeedometerMax;

        if (AudioManager.Instance != null)
        {
            Debug.Log("BOOST SFX DARI AUDIOMANAGER");
            AudioManager.Instance.PlayBoost(boostSFXVolume);
        }
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

        if (PlayerAnimator != null)
            PlayerAnimator.SetTrigger("Stun");

        if (SecondaryAnimator != null)
            SecondaryAnimator.SetTrigger("Stun");
    }

    void UpdateStun()
    {
        if (!isStunned) return;

        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0f)
            isStunned = false;
    }
    void PressSquare()
    {
        if (squareLocked) return;

        Debug.Log("Square Pressed");

        
    }

    public void LockSquare(float time)
    {
        squareLocked = true;
        squareLockTimer = time;
    }

    public void AutoPressSquare()
    {
        PressSquare();
    }
    public void LockBoost(float time)
    {
        boostLocked = true;
        boostLockTimer = time;

        boostReady = false;
        canUseBoost = false;

        if (boostIndicator != null)
            boostIndicator.SetActive(false);
    }

    public void AddStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    public void FinishCountdown()
    {
        countdownFinished = true;
        Debug.Log("COUNTDOWN SELESAI - FOOTSTEP AKTIF");
    }

    void HandlePlayerSFX()
    {
        HandleSprintFootstepSFX();
        HandleFallDownSFX();
    }

    void HandleSprintFootstepSFX()
    {
        if (AudioManager.Instance == null) return;

        if (!countdownFinished)
        {
            footstepTimer = 0f;
            return;
        }

        bool canPlayFootstep =
            !isDashing &&
            !isStunned &&
            currentSpeed >= minFootstepSpeed;

        if (!canPlayFootstep)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            Debug.Log("FOOTSTEP PLAY LEWAT PLAYER SFX");
            AudioManager.Instance.PlayFootstep(sprintFootstepVolume);
            footstepTimer = footstepInterval;
        }
    }

    void StopFootstepSFX()
    {
        footstepTimer = 0f;
    }

    void HandleFallDownSFX()
    {
        if (AudioManager.Instance == null) return;

        if (isStunned && !fallDownSFXPlayed)
        {
            Debug.Log("FALLDOWN SFX DARI AUDIOMANAGER");

            AudioManager.Instance.PlayFallDown(fallDownSFXVolume);
            fallDownSFXPlayed = true;
        }

        if (!isStunned)
        {
            fallDownSFXPlayed = false;
        }
    }
    void UpdateWalkAnimation()
    {
        if (PlayerAnimator == null) return;

        float speedValue = currentSpeed;

        // animator utama
        PlayerAnimator.SetFloat("Speed", speedValue);

        // animator kedua
        if (SecondaryAnimator != null)
        {
            SecondaryAnimator.SetFloat("Speed", speedValue);
        }
    }
}
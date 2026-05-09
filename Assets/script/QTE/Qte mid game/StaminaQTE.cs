using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class StaminaQTE : MonoBehaviour
{
    [Header("Pointer")]
    public RectTransform pointA;
    public RectTransform pointB;
    public RectTransform safeZone;
    public RectTransform pointerTransform;

    [Header("UI")]
    public TMP_Text arrowText;
    public TMP_Text countdownText;

    public GameObject qteCanvas;

    [Header("Settings")]
    public float moveSpeed = 500f;
    public float countdown = 6f;

    public int maxCombo = 4;
    public float staminaReward = 40f;

    [Header("Auto Trigger")]
    public bool autoStartQTE = true;

    [Tooltip("QTE muncul setelah berapa detik")]
    public float qteDelay = 10f;

    [Tooltip("Apakah QTE muncul terus berulang")]
    public bool repeatQTE = true;

    private float qteTimer;

    private int comboCount = 0;

    private Vector2 targetPosition;
    private Vector2 currentDirection;

    private PlayerInput playerInput;
    private InputAction dpadAction;

    private RunController controller;

    private bool qteActive = false;
    private bool qteFinished = false;

    private float timer;

    public bool IsActive => qteActive;

    void Awake()
    {
        if (qteCanvas != null)
            qteCanvas.SetActive(false);
    }

    void Start()
    {
        controller = GetComponent<RunController>();
        playerInput = GetComponent<PlayerInput>();

        qteTimer = qteDelay;
    }

    void Update()
    {
        HandleAutoQTE();

        if (!qteActive) return;

        MovePointer();

        timer -= Time.deltaTime;

        if (countdownText != null)
            countdownText.text = Mathf.CeilToInt(timer).ToString();

        if (timer <= 0f)
        {
            timer = 0f;
            FailQTE();
        }
    }

    void HandleAutoQTE()
    {
        if (!autoStartQTE) return;

        if (qteActive) return;

        qteTimer -= Time.deltaTime;

        if (qteTimer <= 0f)
        {
            StartQTE();

            if (repeatQTE)
                qteTimer = qteDelay;
            else
                autoStartQTE = false;
        }
    }

    public void StartQTE()
    {
        if (qteActive) return;

        qteActive = true;
        qteFinished = false;

        comboCount = 0;

        timer = countdown;

        pointerTransform.anchoredPosition =
            pointA.anchoredPosition;

        targetPosition = pointB.anchoredPosition;

        if (qteCanvas != null)
            qteCanvas.SetActive(true);

        // freeze player
        controller.enabled = false;

        if (controller.PlayerAnimator != null)
            controller.PlayerAnimator.speed = 0f;

        dpadAction = playerInput.actions["Dpad"];

        if (dpadAction != null)
        {
            dpadAction.Enable();
            dpadAction.performed += OnDpadPressed;
        }

        GenerateArrow();

        Debug.Log("AUTO STAMINA QTE START");
    }

    void MovePointer()
    {
        pointerTransform.anchoredPosition =
            Vector2.MoveTowards(
                pointerTransform.anchoredPosition,
                targetPosition,
                moveSpeed * Time.deltaTime);

        float distA =
            Vector2.Distance(
                pointerTransform.anchoredPosition,
                pointA.anchoredPosition);

        float distB =
            Vector2.Distance(
                pointerTransform.anchoredPosition,
                pointB.anchoredPosition);

        if (distA < 5f)
        {
            targetPosition = pointB.anchoredPosition;
        }
        else if (distB < 5f)
        {
            targetPosition = pointA.anchoredPosition;
        }
    }

    void GenerateArrow()
    {
        int rand = Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                currentDirection = Vector2.up;
                arrowText.text = "↑";
                break;

            case 1:
                currentDirection = Vector2.down;
                arrowText.text = "↓";
                break;

            case 2:
                currentDirection = Vector2.left;
                arrowText.text = "←";
                break;

            case 3:
                currentDirection = Vector2.right;
                arrowText.text = "→";
                break;
        }
    }

    void OnDpadPressed(InputAction.CallbackContext ctx)
    {
        if (!qteActive || qteFinished) return;

        Vector2 inputDir =
            ctx.ReadValue<Vector2>().normalized;

        // salah arah
        if (Vector2.Dot(inputDir, currentDirection) < 0.9f)
        {
            FailQTE();
            return;
        }

        // pointer masuk safe zone
        if (RectTransformUtility.RectangleContainsScreenPoint(
            safeZone,
            pointerTransform.position,
            null))
        {
            comboCount++;

            Debug.Log("COMBO: " + comboCount);

            if (comboCount >= maxCombo)
            {
                SuccessQTE();
                return;
            }

            GenerateArrow();
        }
        else
        {
            FailQTE();
        }
    }

    void SuccessQTE()
    {
        if (qteFinished) return;

        qteFinished = true;
        qteActive = false;

        controller.AddStamina(staminaReward);

        Debug.Log("STAMINA BERHASIL");

        EndQTE();
    }

    void FailQTE()
    {
        if (qteFinished) return;

        qteFinished = true;
        qteActive = false;

        Debug.Log("QTE GAGAL");

        EndQTE();
    }

    void EndQTE()
    {
        if (qteCanvas != null)
            qteCanvas.SetActive(false);

        if (dpadAction != null)
            dpadAction.performed -= OnDpadPressed;

        if (controller != null)
        {
            controller.enabled = true;

            if (controller.PlayerAnimator != null)
                controller.PlayerAnimator.speed = 1f;
        }
    }
}
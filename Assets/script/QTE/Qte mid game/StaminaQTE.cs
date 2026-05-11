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
    public GameObject countdownCanvas;

    [Header("Settings")]
    public float moveSpeed = 500f;
    public float countdown = 6f;

    public int maxCombo = 4;
    public float staminaReward = 40f;

    [Header("Auto Trigger")]
    public bool autoStartQTE = true;
    public float qteDelay = 10f;
    public bool repeatQTE = true;

    private float qteTimer;

    private int comboCount = 0;

    private Vector2 targetPosition;
    private Vector2 currentDirection;

    private PlayerInput playerInput;
    private InputAction dpadAction;

    public RunController controller;

    private bool qteActive = false;
    private bool qteFinished = false;

    private float timer;

    // INPUT BUFFER
    private Vector2 lastInput;
    private bool inputPressed;

    public bool IsActive => qteActive;

    void Awake()
    {
        if (qteCanvas != null)
            qteCanvas.SetActive(false);
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        qteTimer = qteDelay;

        if (playerInput != null)
        {
            dpadAction = playerInput.actions["Dpad"];

            if (dpadAction != null)
            {
                dpadAction.Enable();

                Debug.Log("STAMINA QTE DPAD READY");
            }
            else
            {
                Debug.LogError("ACTION DPAD TIDAK DITEMUKAN!");
            }
        }
        else
        {
            Debug.LogError("PLAYER INPUT TIDAK ADA!");
        }
    }

    void Update()
    {
        HandleAutoQTE();

        if (!qteActive)
            return;

        ReadDpadInput();

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

    void ReadDpadInput()
    {
        if (dpadAction == null)
            return;

        Vector2 input = dpadAction.ReadValue<Vector2>();

        // tekan sekali
        if (input != Vector2.zero && !inputPressed)
        {
            inputPressed = true;

            lastInput = input.normalized;

            CheckInput(lastInput);
        }

        // reset saat dilepas
        if (input == Vector2.zero)
        {
            inputPressed = false;
        }
    }

    void HandleAutoQTE()
    {
        if (!autoStartQTE)
            return;

        if (qteActive)
            return;

        qteTimer -= Time.deltaTime;

        if (qteTimer <= 0f)
        {
            StartQTE();

            if (!repeatQTE)
                autoStartQTE = false;
        }
    }

    public void StartQTE()
    {
        if (qteActive)
            return;

        qteActive = true;
        qteFinished = false;

        comboCount = 0;

        timer = countdown;

        pointerTransform.anchoredPosition =
            pointA.anchoredPosition;

        targetPosition = pointB.anchoredPosition;

        if (qteCanvas != null)
            qteCanvas.SetActive(true);

        if (countdownCanvas != null)
            countdownCanvas.SetActive(true);

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

    void CheckInput(Vector2 inputDir)
    {
        if (!qteActive || qteFinished)
            return;

        Debug.Log("INPUT MASUK: " + inputDir);

        // cek arah
        if (Vector2.Dot(inputDir, currentDirection) < 0.9f)
        {
            Debug.Log("SALAH ARAH");
            FailQTE();
            return;
        }

        bool insideSafeZone =
            RectTransformUtility.RectangleContainsScreenPoint(
                safeZone,
                pointerTransform.position,
                null);

        if (insideSafeZone)
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
            Debug.Log("POINTER TIDAK MASUK SAFEZONE");
            FailQTE();
        }
    }

    void SuccessQTE()
    {
        if (qteFinished)
            return;

        qteFinished = true;
        qteActive = false;

        if (controller != null)
        {
            controller.AddStamina(staminaReward);

            Debug.Log("Tambah stamina: " + staminaReward);
        }
        else
        {
            Debug.LogError("RUN CONTROLLER NULL!");
        }

        Debug.Log("STAMINA BERHASIL");

        EndQTE();
    }

    void FailQTE()
    {
        if (qteFinished)
            return;

        qteFinished = true;
        qteActive = false;

        Debug.Log("QTE GAGAL");

        EndQTE();
    }

    void EndQTE()
    {
        if (qteCanvas != null)
            qteCanvas.SetActive(false);

        if (countdownCanvas != null)
            countdownCanvas.SetActive(false);

        if (repeatQTE)
        {
            qteTimer = qteDelay;
        }
    }

    public void Init(PlayerInput inputFromPlayer)
    {
        playerInput = inputFromPlayer;

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput NULL!");
            return;
        }

        dpadAction = playerInput.actions["Dpad"];

        if (dpadAction == null)
        {
            Debug.LogError("Action Dpad tidak ditemukan!");
            return;
        }

        dpadAction.Enable();

        Debug.Log("StaminaQTE Input Connected");
    }

    void OnDestroy()
    {
        if (dpadAction != null)
        {
            dpadAction.Disable();
        }
    }
}
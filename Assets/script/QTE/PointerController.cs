using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PointerController : MonoBehaviour
{
    public RectTransform pointA;
    public RectTransform pointB;
    public RectTransform safeZone;
    public float moveSpeed = 100f;

    public TMP_Text arrowText;
    public TMP_Text countdownText;

    public GameObject qteCanvas;
    public GameObject countdownCanvas;

    public RunController playerController;

    public RectTransform pointerTransform;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip countdownBeep;

    private bool playedThreeSound = false;

    private Vector2 targetPosition;

    private PlayerInput playerInput;
    private InputAction dpadAction;
    private Vector2 currentDirection;

    float countdownTimer = 10f;

    private int comboCount = 0;
    private int maxCombo = 4;

    private bool qteActive = false;
    private bool qteFinished = false;
    private bool qteSuccess = false;

    void Start()
    {
        Debug.Log("PlayerInput: " + playerInput);

        if (pointA == null || pointB == null || pointerTransform == null)
        {
            Debug.LogError("PointerController belum lengkap!");
            enabled = false;
            return;
        }

        targetPosition = pointB.anchoredPosition;

        if (playerController != null)
        {
            playerController.enabled = false;

            if (playerController.PlayerAnimator != null)
                playerController.PlayerAnimator.speed = 0f;
        }

        if (qteCanvas != null) qteCanvas.SetActive(true);
        if (countdownCanvas != null) countdownCanvas.SetActive(true);

        qteActive = true;

        GenerateRandomArrow();
    }

    void Update()
    {
        RunCountdown();

        if (qteActive && countdownTimer > 0f)
            MovePointer();
    }

    void RunCountdown()
    {
        if (countdownTimer <= 0f) return;

        countdownTimer -= Time.deltaTime;

        int display = Mathf.CeilToInt(countdownTimer);

        if (countdownText != null)
            countdownText.text = display.ToString();

        // PLAY SOUND SAAT ANGKA 3
        if (display == 3 && !playedThreeSound)
        {
            playedThreeSound = true;

            if (audioSource != null && countdownBeep != null)
            {
                audioSource.PlayOneShot(countdownBeep);
            }
        }

        if (countdownTimer <= 0f)
        {
            countdownTimer = 0f;
            CountdownFinished();
        }
    }

    void CountdownFinished()
    {
        if (countdownCanvas != null)
            countdownCanvas.SetActive(false);

        if (!qteFinished)
        {
            qteSuccess = false;

            if (qteCanvas != null)
                qteCanvas.SetActive(false);
        }

        if (playerController != null)
        {
            if (playerController.PlayerAnimator != null)
                playerController.PlayerAnimator.speed = 1f;

            playerController.enabled = true;

            if (qteSuccess)
                playerController.canUseBoost = true;
            else
                playerController.LockBoost(6f);
        }

        if (!qteActive || qteFinished) return;
    }

    void MovePointer()
    {
        pointerTransform.anchoredPosition = Vector2.MoveTowards(
            pointerTransform.anchoredPosition,
            targetPosition,
            moveSpeed * Time.deltaTime);

        float distA = Vector2.Distance(pointerTransform.anchoredPosition, pointA.anchoredPosition);
        float distB = Vector2.Distance(pointerTransform.anchoredPosition, pointB.anchoredPosition);

        if (distA < 5f)
            targetPosition = pointB.anchoredPosition;
        else if (distB < 5f)
            targetPosition = pointA.anchoredPosition;
    }

    void GenerateRandomArrow()
    {
        int rand = Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                currentDirection = Vector2.up;
                if (arrowText != null) arrowText.text = "↑";
                break;

            case 1:
                currentDirection = Vector2.down;
                if (arrowText != null) arrowText.text = "↓";
                break;

            case 2:
                currentDirection = Vector2.left;
                if (arrowText != null) arrowText.text = "←";
                break;

            case 3:
                currentDirection = Vector2.right;
                if (arrowText != null) arrowText.text = "→";
                break;
        }
    }

    void OnDpadPressed(InputAction.CallbackContext ctx)
    {
        if (!qteActive || qteFinished) return;

        Vector2 inputDir = ctx.ReadValue<Vector2>().normalized;

        if (Vector2.Dot(inputDir, currentDirection) < 0.9f)
        {
            FailQTE();
            return;
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(
            safeZone,
            pointerTransform.position,
            null))
        {
            comboCount++;

            if (comboCount >= maxCombo)
            {
                FinishQTE();
                return;
            }

            GenerateRandomArrow();
        }
        else
        {
            FailQTE();
        }

        Debug.Log("INPUT MASUK: " + ctx.ReadValue<Vector2>());
    }

    void FailQTE()
    {
        if (qteFinished) return;

        qteSuccess = false;
        qteFinished = true;
        qteActive = false;

        if (qteCanvas != null)
            qteCanvas.SetActive(false);

        CleanupInput();
    }

    void FinishQTE()
    {
        if (qteFinished) return;

        qteSuccess = true;
        qteFinished = true;
        qteActive = false;

        if (qteCanvas != null)
            qteCanvas.SetActive(false);

        CleanupInput();
    }

    void CleanupInput()
    {
        if (dpadAction != null)
            dpadAction.performed -= OnDpadPressed;
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

        dpadAction.performed -= OnDpadPressed;
        dpadAction.performed += OnDpadPressed;

        Debug.Log("Dpad siap untuk player: " + playerInput.playerIndex);
    }

    void OnDestroy()
    {
        if (dpadAction != null)
        {
            dpadAction.performed -= OnDpadPressed;
        }
    }
}
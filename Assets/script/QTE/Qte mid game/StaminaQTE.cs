using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class StaminaQTETrigger : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public RectTransform safeZone;
    public float moveSpeed = 100f;

    public TMP_Text arrowText;
    public TMP_Text countdownText;

    public GameObject qteCanvas;
    public GameObject countdownCanvas;

    public RectTransform pointerTransform;

    private PlayerInput playerInput;
    private InputAction dpadAction;
    private RunController playerController;

    private Vector3 targetPosition;
    private Vector2 currentDirection;

    float countdownTimer = 10f;

    private int comboCount = 0;
    private int maxCombo = 4;

    private bool qteActive = false;
    private bool qteFinished = false;
    private bool triggerUsed = false;

    void Start()
    {
        if (pointA == null || pointB == null || pointerTransform == null)
        {
            Debug.LogError("QTE Setup belum lengkap!");
            enabled = false;
            return;
        }

        targetPosition = pointB.position;

        if (qteCanvas != null) qteCanvas.SetActive(false);
        if (countdownCanvas != null) countdownCanvas.SetActive(false);
    }

    void Update()
    {
        if (!qteActive) return;

        RunCountdown();

        if (countdownTimer > 0f)
            MovePointer();
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggerUsed) return;
        if (!other.CompareTag("Player")) return;

        triggerUsed = true;

        // 🔥 AMBIL PlayerInput dari player yang masuk
        playerInput = other.GetComponent<PlayerInput>();
        playerController = other.GetComponent<RunController>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput tidak ditemukan di player!");
            return;
        }

        // 🔥 ambil action dari player ini
        dpadAction = playerInput.actions["Dpad"];

        if (dpadAction == null)
        {
            Debug.LogError("Action Dpad tidak ditemukan!");
            return;
        }

        dpadAction.performed += OnDpadPressed;

        StartQTE();
    }

    void StartQTE()
    {
        qteActive = true;
        qteFinished = false;
        comboCount = 0;

        countdownTimer = 10f;

        if (qteCanvas != null) qteCanvas.SetActive(true);
        if (countdownCanvas != null) countdownCanvas.SetActive(true);

        GenerateRandomArrow();
    }

    void RunCountdown()
    {
        if (countdownTimer <= 0f) return;

        countdownTimer -= Time.deltaTime;

        int display = Mathf.CeilToInt(countdownTimer);

        if (countdownText != null)
            countdownText.text = display.ToString();

        if (countdownTimer <= 0f)
        {
            countdownTimer = 0f;
            EndQTE();
        }
    }

    void EndQTE()
    {
        qteActive = false;

        if (qteCanvas != null) qteCanvas.SetActive(false);
        if (countdownCanvas != null) countdownCanvas.SetActive(false);

        // 🔥 penting: lepas event biar tidak nempel
        if (dpadAction != null)
            dpadAction.performed -= OnDpadPressed;
    }

    void MovePointer()
    {
        pointerTransform.position = Vector3.MoveTowards(
            pointerTransform.position,
            targetPosition,
            moveSpeed * Time.deltaTime);

        if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
            targetPosition = pointB.position;
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
            targetPosition = pointA.position;
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

        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
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
    }

    void FailQTE()
    {
        if (qteFinished) return;

        qteFinished = true;
        EndQTE();
    }

    void FinishQTE()
    {
        if (qteFinished) return;

        qteFinished = true;

        if (playerController != null)
            playerController.AddStamina(40f);

        EndQTE();
    }
}
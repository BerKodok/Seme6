using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class StaminaQTETrigger : MonoBehaviour
{
    [Header("Pointer")]
    public Transform pointA;
    public Transform pointB;
    public RectTransform safeZone;
    public RectTransform pointerTransform;

    [Header("UI")]
    public TMP_Text arrowText;
    public TMP_Text countdownText;
    public GameObject qteCanvas;
    public GameObject countdownCanvas;

    [Header("Settings")]
    public float moveSpeed = 100f;
    public float countdownDuration = 10f;
    public int maxCombo = 4;

    private PlayerInput playerInput;
    private InputAction dpadAction;
    private RunController playerController;

    private Vector3 targetPosition;
    private Vector2 currentDirection;

    private float countdownTimer;
    private int comboCount;

    private bool qteActive = false;
    private bool qteFinished = false;
    private bool triggerUsed = false;

    void Start()
    {
        if (pointA == null || pointB == null || pointerTransform == null)
        {
            Debug.LogError("❌ QTE Setup belum lengkap di: " + gameObject.name);
            enabled = false;
            return;
        }

        targetPosition = pointB.position;

        if (qteCanvas) qteCanvas.SetActive(false);
        if (countdownCanvas) countdownCanvas.SetActive(false);
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

        Debug.Log("🔥 Player masuk trigger QTE");

        triggerUsed = true;

        // AMBIL COMPONENT DARI PLAYER
        playerInput = other.GetComponent<PlayerInput>();
        playerController = other.GetComponent<RunController>();

        if (playerInput == null)
        {
            Debug.LogError("❌ PlayerInput tidak ditemukan!");
            return;
        }

        // AMBIL ACTION
        dpadAction = playerInput.actions["Dpad"];

        if (dpadAction == null)
        {
            Debug.LogError("❌ Action Dpad tidak ditemukan!");
            return;
        }

        // AKTIFKAN INPUT
        dpadAction.Enable();
        dpadAction.performed += OnDpadPressed;

        StartQTE();
    }

    // 🔥 WAJIB PUBLIC (POINT 1)
    public void StartQTE()
    {
        Debug.Log("✅ QTE DIMULAI");

        qteActive = true;
        qteFinished = false;
        comboCount = 0;

        countdownTimer = countdownDuration;

        if (qteCanvas)
        {
            qteCanvas.SetActive(true);
            Debug.Log("Canvas QTE ON");
        }

        if (countdownCanvas)
        {
            countdownCanvas.SetActive(true);
            Debug.Log("Countdown ON");
        }

        GenerateRandomArrow();
    }

    void RunCountdown()
    {
        if (countdownTimer <= 0f) return;

        countdownTimer -= Time.deltaTime;

        if (countdownText)
            countdownText.text = Mathf.CeilToInt(countdownTimer).ToString();

        if (countdownTimer <= 0f)
        {
            countdownTimer = 0f;
            EndQTE();
        }
    }

    void EndQTE()
    {
        Debug.Log("⛔ QTE SELESAI");

        qteActive = false;

        if (qteCanvas) qteCanvas.SetActive(false);
        if (countdownCanvas) countdownCanvas.SetActive(false);

        if (dpadAction != null)
        {
            dpadAction.performed -= OnDpadPressed;
            dpadAction.Disable();
        }
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
            case 0: currentDirection = Vector2.up; arrowText.text = "↑"; break;
            case 1: currentDirection = Vector2.down; arrowText.text = "↓"; break;
            case 2: currentDirection = Vector2.left; arrowText.text = "←"; break;
            case 3: currentDirection = Vector2.right; arrowText.text = "→"; break;
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

        Camera cam = Camera.main;

        if (RectTransformUtility.RectangleContainsScreenPoint(
            safeZone,
            RectTransformUtility.WorldToScreenPoint(cam, pointerTransform.position),
            cam))
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

        Debug.Log("❌ QTE GAGAL");

        qteFinished = true;
        EndQTE();
    }

    void FinishQTE()
    {
        if (qteFinished) return;

        Debug.Log("🎉 QTE BERHASIL");

        qteFinished = true;

        if (playerController != null)
            playerController.AddStamina(40f);

        EndQTE();
    }
}
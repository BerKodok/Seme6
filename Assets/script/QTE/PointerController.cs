using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PointerController : MonoBehaviour
{

    public Transform pointA;
    public Transform pointB;
    public RectTransform safeZone;
    public float moveSpeed = 100f;

    public TMP_Text arrowText;
    public TMP_Text countdownText;

    public GameObject qteCanvas;
    public GameObject countdownCanvas;

    public RunController playerController;

    public RectTransform pointerTransform;
    private Vector3 targetPosition;

    private PlayerInputActions input;
    private Vector2 currentDirection;

    float countdownTimer = 6f;

    private int comboCount = 0;
    private int maxCombo = 4;

    private bool qteActive = true;
    private bool qteSuccess = false;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.Dpad.performed += OnDpadPressed;
    }

    void OnDisable()
    {
        input.Player.Dpad.performed -= OnDpadPressed;
        input.Disable();
    }

    void Start()
    {
        targetPosition = pointB.position;

        playerController.enabled = false;

        GenerateRandomArrow();
    }

    void Update()
    {
        RunCountdown();

        if (qteActive)
        {
            MovePointer();
        }
    }

    void MovePointer()
    {
        pointerTransform.position = Vector3.MoveTowards(
            pointerTransform.position,
            targetPosition,
            moveSpeed * Time.deltaTime);

        if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
        {
            targetPosition = pointB.position;
        }
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
        {
            targetPosition = pointA.position;
        }
    }

    void GenerateRandomArrow()
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
        if (!qteActive) return;

        Vector2 inputDir = ctx.ReadValue<Vector2>();

        if (inputDir != currentDirection)
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
        qteActive = false;
        qteSuccess = false;

        playerController.LockBoost(6f);

        qteCanvas.SetActive(false);
        countdownCanvas.SetActive(true);
    }

    void FinishQTE()
    {
        qteActive = false;
        qteSuccess = true;

        qteCanvas.SetActive(false);
        countdownCanvas.SetActive(true);
    }

    void RunCountdown()
    {
        countdownTimer -= Time.deltaTime;

        countdownText.text = Mathf.Ceil(countdownTimer).ToString();

        if (countdownTimer <= 0)
        {
            CountdownFinished();
        }
    }

    void CountdownFinished()
    {
        countdownCanvas.SetActive(false);

        playerController.enabled = true;

        if (qteSuccess)
        {
            playerController.StartDash();
        }

        qteActive = true;
        comboCount = 0;

        GenerateRandomArrow();
    }
}
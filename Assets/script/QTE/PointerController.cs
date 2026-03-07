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
    public TMP_Text timerText;

    public GameObject qteCanvas;

    private RectTransform pointerTransform;
    private Vector3 targetPosition;

    private PlayerInputActions input;

    private Vector2 currentDirection;

    private int comboCount = 0;
    private int maxCombo = 4;

    private float timer = 6f;
    private bool qteActive = true;

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
        pointerTransform = GetComponent<RectTransform>();
        targetPosition = pointB.position;

        GenerateRandomArrow();
    }

    void Update()
    {
        if (!qteActive) return;

        HandleTimer();
        MovePointer();
    }

    void HandleTimer()
    {
        timer -= Time.deltaTime;

        timerText.text = " " + Mathf.Ceil(timer).ToString();

        if (timer <= 0)
        {
            FailQTE();
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

            Debug.Log("SUCCESS " + comboCount);

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

        Debug.Log("QTE FAILED");

        qteCanvas.SetActive(false);
    }

    void FinishQTE()
    {
        qteActive = false;

        Debug.Log("QTE COMPLETE!");

        qteCanvas.SetActive(false);
    }
}
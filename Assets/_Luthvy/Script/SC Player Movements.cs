using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class SCPlayerMovements : MonoBehaviour
{
   /// ///////////////////////////////////////////////////////
    /// PLAYER BIT
    private Rigidbody rb;
    
    //private PlayerInput playerInput;
    //private PlayerInputActions playerInputActions;
    private InputActionAsset inputAsset;
    private InputActionMap player;
    private InputAction moveCharacter, moveMouse, moveJump;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask groundMask;
    private bool OnGround;
    /// ///////////////////////////////////////////////////////
    /// CAMERA BIT
    [SerializeField] private Transform camTransform;
    [SerializeField] private float cameraSensitivity = 0.1f;
    //[SerializeField] private CinemachineCamera playerCam;
    private float cameraYaw;
    private float cameraPitch;

    /// ///////////////////////////////////////////////////////
    /// KEYBOARD BIT
    [SerializeField] private float f_jump = 5f;
    [SerializeField] private float f_speed = 5f;
    [SerializeField] private float f_acceleration = 20f;

    /// ///////////////////////////////////////////////////////
    /// NETWORK
    //private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1);

    /// ///////////////////////////////////////////////////////
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("ThePlayer");
        //camTransform, playerCam = GetComponentInChildren<cin>

        //playerInputActions = new PlayerInputActions();

    }
    /// ///////////////////////////////////////////////////////
    void OnEnable()
    {
        //if (!IsOwner) return;
        // playerInputActions.ThePlayer.Enable();
        // playerInputActions.ThePlayer.Jump.performed += Jump;       
        moveJump = player.FindAction("Jump"); moveJump.performed += DoJump;
        moveCharacter = player.FindAction("Movement");
        moveMouse = player.FindAction("Look");
        player.Enable();
    }

    void OnDisable()
    {
        // playerInputActions.ThePlayer.Jump.performed -= Jump;   
        // playerInputActions.ThePlayer.Disable();   
        moveJump.performed -= DoJump;
        player.Disable();
    }


    /// ///////////////////////////////////////////////////////
    /// 


    private void FixedUpdate()
    {

        OnGround = Physics.Raycast
        (transform.position + Vector3.up * 0.05f,
        Vector3.down, groundCheckDistance, groundMask); // RAYCAST GROUND CHECK
        ///////////////////////////////////////////////////////

        Vector2 vectorInput = moveCharacter.ReadValue<Vector2>(); // INPUT SYSTEM READ TO V2

        Vector3 camForward = camTransform.forward; // Z POS CAM
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = camTransform.right; // X POS CAM
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 move = camForward * vectorInput.y + camRight * vectorInput.x; // CALC moveV3 from CAMV3 & ISV2 
        move = Vector3.ClampMagnitude(move, 1f);

        Vector3 velo = rb.linearVelocity;

        if (move.sqrMagnitude > 0.001f)
        {
            Vector3 targetVelocity = move * f_speed; // MOVE CALC

            Vector3 horizontalVelocity = new Vector3(velo.x, 0f, velo.z);
            Vector3 newHorizontalVelocity = Vector3.MoveTowards(
                horizontalVelocity,
                targetVelocity,
                f_acceleration * Time.fixedDeltaTime
            );

            rb.linearVelocity = new Vector3(
                newHorizontalVelocity.x,
                velo.y,
                newHorizontalVelocity.z
            ); // MOVE APPLY TO RB
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, velo.y, 0f); // INSTANT STOP , IGNORE velo Y
        }

        rb.MoveRotation(Quaternion.Euler(0f, cameraYaw, 0f)); // ROTATE RB Y BASED OF CAM

        Vector2 lookInput = moveMouse.ReadValue<Vector2>(); // INPUT SYSTEM CAM READ TO V2
        cameraYaw += lookInput.x * cameraSensitivity; //* Time.deltaTime; // CAM X V2 ADD TO = cameraYaw

        cameraPitch -= lookInput.y * cameraSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        camTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }


    private void Update()
    {

    }
    public void DoJump(InputAction.CallbackContext context) // JUMP METHOD
    {
        if(!context.performed || !OnGround) return; // IF KEY NOT PRESSED(context.performed) OR NOT GROUNDED
        rb.AddForce(Vector3.up * f_jump, ForceMode.Impulse); // RB UP ADD BY JUMP FLOAT
        
    }
}

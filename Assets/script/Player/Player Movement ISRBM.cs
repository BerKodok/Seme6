using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
//using Unity.Netcode;
using Unity.Mathematics;
using Mirror;
//using System.Numerics;
//using System.Numerics;


public class PlayerMovementISRBM : NetworkBehaviour
{
    /// ///////////////////////////////////////////////////////
    /// PLAYER BIT
    private Rigidbody rb;
    
    //private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask groundMask;
    private bool OnGround;

    /// ///////////////////////////////////////////////////////
    /// CAMERA BIT
    [SerializeField] private Transform camTransform;
    [SerializeField] private CinemachineCamera playerCam;
    private float cameraYaw;

    /// ///////////////////////////////////////////////////////
    /// KEYBOARD BIT
    [SerializeField] private float f_jump = 5f;
    [SerializeField] private float f_speed = 5f;
    [SerializeField] private float f_acceleration = 20f;

    /// ///////////////////////////////////////////////////////
    /// NETWORK
    private Vector2 serverMoveInput;
    private float serverYaw;
    [Command]
    void CmdSendInput(Vector2 moveInput, Vector2 lookInput)
    {
        serverMoveInput = moveInput;
        serverYaw += lookInput.x *0.1f;
    }

    /// ///////////////////////////////////////////////////////
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //camTransform, playerCam = GetComponentInChildren<cin>

        playerInputActions = new PlayerInputActions();

    }
    /// ///////////////////////////////////////////////////////
    void OnEnable()
    {
    
    }

    void OnDisable()
    {
        if (playerInputActions != null)
        {
            playerInputActions.ThePlayer.Jump.performed -= Jump;   
            playerInputActions.ThePlayer.Disable();
            playerInputActions.TheCam.Disable();             
        }
    }

    public override void OnStartLocalPlayer()
    {
        playerCam.enabled = true;

        //if (!isLocalPlayer) return;
        playerInputActions.ThePlayer.Enable();
        playerInputActions.TheCam.Enable();
        playerInputActions.ThePlayer.Jump.performed += Jump;   
    }

    public override void OnStartClient()
    {
        playerCam.enabled = false;
        if (!isServer)  {rb.isKinematic = true;}
    }

    private void FixedUpdate()
    {
        if (!isServer) return;

        OnGround = Physics.Raycast
        (transform.position + Vector3.up * 0.05f,
        Vector3.down, groundCheckDistance, groundMask); // RAYCAST GROUND CHECK
        ///////////////////////////////////////////////////////


        ///////////////////////////////////////////////////////
        
        Vector3 foward = Quaternion.Euler (0f,serverYaw, 0f) * Vector3.forward;
        Vector3 right  = Quaternion.Euler (0f, serverYaw, 0f) * Vector3.right;

        Vector3 move = foward * serverMoveInput.y + right * serverMoveInput.x;
        move = Vector3.ClampMagnitude(move, 1f);

        Vector3 velo = rb.linearVelocity;

        Vector3 targetVelocity     = move * f_speed;
        Vector3 horizontalVelocity = new Vector3(velo.x, 0f, velo.z);

        Vector3 newHorizontalvelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, f_acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(newHorizontalvelocity.x, velo.y, newHorizontalvelocity.z);
        rb.MoveRotation(Quaternion.Euler(0f, serverYaw, 0f));
    }


    private void Update()
    {
        //Debug.Log(OwnerClientId + "; randomNumber: " + randomNumber.Value);
        if (!isLocalPlayer) return;

        Vector2 moveInput = playerInputActions.ThePlayer.Movement.ReadValue<Vector2>();
        Vector2 lookInput = playerInputActions.TheCam.Look.ReadValue<Vector2>(); // INPUT SYSTEM CAM READ TO V2
        //cameraYaw += lookInput.x * 0.1f; // CAM X V2 ADD TO = cameraYaw
        CmdSendInput(moveInput, lookInput);
    }
    public void Jump(InputAction.CallbackContext context) // JUMP METHOD
    {
        //Debug.Log(context);
        if (!isLocalPlayer)    return;
        if(!context.performed) return; // IF KEY NOT PRESSED(context.performed) OR NOT GROUNDED
        
        CmdJump();
    }

    [Command]
    void CmdJump()
    {
        if (!OnGround) return;
        rb.AddForce(Vector3.up * f_jump, ForceMode.Impulse); // RB UP ADD BY JUMP FLOAT
    }
}

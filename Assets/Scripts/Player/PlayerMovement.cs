using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;

    [Header("Movement Settings")]
    public float moveSpeed;
    public float jumpForce;
    public float airMultiplier;
    public float groundDrag;
    public float playerHeight;
    public LayerMask groundLayer;
    public bool isGrounded;
    public Transform orientation;

    [Header("Jump Budget & Impulse")]
    public int maxJumps = 3;
    private int currentJumps;
    public float airJumpRefillInterval = 2.0f;
    private float refillTimer;
    public float airJumpImpulse = 5f; // Extra directional "kick" for air jumps

    // HUD Implementation
    public int AvailableAirJumps => currentJumps;
    public float AirJumpRefreshProgress01 => (currentJumps < maxJumps) ? (refillTimer / airJumpRefillInterval) : 1f;

    [Header("Effects")]
    private bool wasGrounded;
    private float footstepTimer;
    public float footstepDelay = 0.4f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentJumps = maxJumps;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("3DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        jumpAction = new InputAction("Jump", InputActionType.Value);
        jumpAction.AddBinding("<Keyboard>/space");
    }

    private void OnEnable() { moveAction.Enable(); jumpAction.Enable(); }
    private void OnDisable() { moveAction.Disable(); jumpAction.Disable(); }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);

        HandleRefillLogic();
        HandleJumpInput();
        HandleVelocityAndSounds();

        rb.linearDamping = isGrounded ? groundDrag : 0;
    }

    private void HandleRefillLogic()
    {
        if (!wasGrounded && isGrounded) SoundManager.PlaySound(SoundType.Player_Land);
        wasGrounded = isGrounded;

        if (isGrounded && currentJumps < maxJumps)
        {
            refillTimer += Time.deltaTime;
            if (refillTimer >= airJumpRefillInterval)
            {
                currentJumps++;
                refillTimer = 0f;
            }
        }
    }

    private void HandleJumpInput()
    {
        if (jumpAction.triggered && currentJumps > 0)
        {
            Vector3 input = moveAction.ReadValue<Vector3>();
            Vector3 jumpDir = orientation.forward * input.y + orientation.right * input.x;

            currentJumps--;
            Jump(jumpDir.normalized);
        }
    }

    private void HandleVelocityAndSounds()
    {
        horizontalInput = moveAction.ReadValue<Vector3>().x;
        verticalInput = moveAction.ReadValue<Vector3>().y;

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // Horizontal speed limit
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }

        if (isGrounded && flatVel.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                SoundManager.PlaySound(SoundType.Player_Footstep);
                footstepTimer = footstepDelay;
            }
        }
    }

    private void FixedUpdate()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        float multiplier = isGrounded ? 10f : 10f * airMultiplier;
        rb.AddForce(moveDirection.normalized * moveSpeed * multiplier, ForceMode.Force);
    }

    private void Jump(Vector3 direction)
    {
        // AIR JUMP LOGIC
        if (!isGrounded)
        {
            // Reset Y to zero out falling momentum
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (direction.magnitude > 0.1f)
            {
                // Instantaneously change momentum to face input
                rb.linearVelocity = direction * moveSpeed;
                // APPLY EXTRA FORCE Burst (The "Kick")
                rb.AddForce(direction * airJumpImpulse, ForceMode.Impulse);
            }
            else
            {
                // Neutral air jump: stop horizontal momentum
                rb.linearVelocity = Vector3.zero;
            }
        }
        // GROUND JUMP LOGIC
        else
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }

        // Apply Vertical Jump Force
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        SoundManager.PlaySound(SoundType.Player_Jump);
    }
public void ApplyDashForce(Vector3 direction, float force)
    {
        // PLAY DASH SOUND
        SoundManager.PlaySound(SoundType.Player_Dash);   

        // Zero out current velocity first for a consistent dash distance
        rb.linearVelocity = Vector3.zero;

        // Apply the burst of speed
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}
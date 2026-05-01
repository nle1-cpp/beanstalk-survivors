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

    public float moveSpeed;
    public float jumpForce;
    public float airMultiplier;
    public float groundDrag;
    public float playerHeight;
    public LayerMask groundLayer;
    public bool isGrounded;
    public Transform orientation;

    // FOOTSTEP + LANDING TRACKING
    private bool wasGrounded;
    private float footstepTimer;
    public float footstepDelay = 0.4f;

    // double jump
    // public int MaxJumps = 2;
    // private int _jumpCount = 0;

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

      private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void Update()
    {
        // Check if grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        // LAND SOUND (trigger once when hitting ground)
        if (!wasGrounded && isGrounded)
        {
            SoundManager.PlaySound(SoundType.Player_Land);
        }
        wasGrounded = isGrounded;

        // Read Player Movement Direction
        horizontalInput = moveAction.ReadValue<Vector3>().x;
        verticalInput = moveAction.ReadValue<Vector3>().y;

        // Limit Max Velocity
        Vector3 currVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (currVelocity.magnitude > moveSpeed)
        {
            Vector3 maxVelocity = currVelocity.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(maxVelocity.x, rb.linearVelocity.y, maxVelocity.z);
        }

        // Jump
        if (jumpAction.triggered && jumpAction.ReadValue<float>() > 0f && isGrounded)
        {
            Jump();
        }

        // FOOTSTEPS SOUND (only when grounded and moving)
        if (isGrounded && currVelocity.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                SoundManager.PlaySound(SoundType.Player_Footstep);
                footstepTimer = footstepDelay;
            }
        }
        else
        {
            footstepTimer = 0f; // reset timer when not moving or in air
        }

        // Apply ground drag when grounded
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        } else
        {
            rb.linearDamping = 0;
        }
    }

    private void FixedUpdate()
    {
        // Move Player
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (isGrounded) {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        } else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void Jump()
    {
        // Reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Apply jump force once
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    
        // PLAY JUMP SOUND
        SoundManager.PlaySound(SoundType.Player_Jump);   
    }

    public void ApplyDashForce(Vector3 direction, float force)
    {
        // Zero out current velocity first for a consistent dash distance
        rb.linearVelocity = Vector3.zero;

        // Apply the burst of speed
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

}
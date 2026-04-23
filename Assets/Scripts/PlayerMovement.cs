using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector3 moveDirection;
    private Vector3 moveInput;
    private bool jumpRequested;
    private float airJumpRegenTimer;
    private int availableAirJumps;

    public float moveSpeed;
    public float jumpForce;
    public float airMultiplier;
    public float groundDrag;
    public float playerHeight;
    public LayerMask groundLayer;
    public bool isGrounded;
    public Transform orientation;

    public float groundAcceleration = 40f;
    public float airAcceleration = 14f;
    public float gravity = -30f;
    public float terminalVelocity = -40f;
    public float groundedStickVelocity = -2f;
    public float groundCheckRadius = 0.28f;
    public float groundCheckOffset = 0.08f;
    public int maxAirJumps = 3;
    public float airJumpRefillInterval = 0.35f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        if (orientation == null)
        {
            orientation = transform;
        }

        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("3DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");

        maxAirJumps = Mathf.Clamp(maxAirJumps, 0, 3);
        availableAirJumps = maxAirJumps;
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        jumpForce = Mathf.Max(0f, jumpForce);
        airMultiplier = Mathf.Max(0f, airMultiplier);
        groundDrag = Mathf.Max(0f, groundDrag);
        playerHeight = Mathf.Max(0f, playerHeight);
        groundAcceleration = Mathf.Max(0f, groundAcceleration);
        airAcceleration = Mathf.Max(0f, airAcceleration);
        terminalVelocity = Mathf.Min(0f, terminalVelocity);
        groundCheckRadius = Mathf.Max(0.01f, groundCheckRadius);
        groundCheckOffset = Mathf.Max(0f, groundCheckOffset);
        maxAirJumps = Mathf.Clamp(maxAirJumps, 0, 3);
        airJumpRefillInterval = Mathf.Max(0f, airJumpRefillInterval);
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
        UpdateGroundedState();
        UpdateAirJumpRefill();

        moveInput = moveAction.ReadValue<Vector3>();
        if (jumpAction.WasPressedThisFrame())
        {
            jumpRequested = true;
        }

        rb.linearDamping = isGrounded ? groundDrag : 0f;
    }

    private void FixedUpdate()
    {
        bool skipHorizontalSteering = false;

        if (jumpRequested)
        {
            skipHorizontalSteering = TryPerformJump();
            jumpRequested = false;
        }

        ApplyHorizontalMovement(skipHorizontalSteering);
        ApplyGravity();
    }

    private void UpdateGroundedState()
    {
        Vector3 groundCheckOrigin = GetGroundCheckOrigin();
        isGrounded = Physics.CheckSphere(groundCheckOrigin, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    private Vector3 GetGroundCheckOrigin()
    {
        if (capsuleCollider != null)
        {
            Bounds bounds = capsuleCollider.bounds;
            return new Vector3(bounds.center.x, bounds.min.y + groundCheckOffset, bounds.center.z);
        }

        float halfHeight = Mathf.Max(playerHeight * 0.5f, 0.5f);
        return transform.position + Vector3.down * (halfHeight - groundCheckOffset);
    }

    private void UpdateAirJumpRefill()
    {
        if (!ShouldRefreshAirJumps())
        {
            airJumpRegenTimer = 0f;
            return;
        }

        if (availableAirJumps >= maxAirJumps)
        {
            airJumpRegenTimer = 0f;
            return;
        }

        airJumpRegenTimer += Time.deltaTime;
        if (airJumpRegenTimer < airJumpRefillInterval)
        {
            return;
        }

        airJumpRegenTimer = 0f;
        availableAirJumps++;
    }

    private bool ShouldRefreshAirJumps()
    {
        // Placeholder hook for future refill sources like landing pads or pickups.
        return isGrounded;
    }

    private bool TryPerformJump()
    {
        if (isGrounded)
        {
            PerformJump(false);
            return false;
        }

        if (availableAirJumps <= 0)
        {
            return false;
        }

        availableAirJumps--;
        PerformJump(true);
        return true;
    }

    private void PerformJump(bool isAirJump)
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 jumpDirection = GetMoveDirection();

        if (isAirJump)
        {
            if (jumpDirection.sqrMagnitude > 0.0001f)
            {
                float horizontalSpeed = horizontalVelocity.magnitude;
                horizontalVelocity = jumpDirection.normalized * horizontalSpeed;
            }
            else
            {
                horizontalVelocity = Vector3.zero;
            }
        }

        rb.linearVelocity = new Vector3(horizontalVelocity.x, 0f, horizontalVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ApplyHorizontalMovement(bool skipSteeringThisFrame)
    {
        if (skipSteeringThisFrame)
        {
            return;
        }

        moveDirection = GetMoveDirection();
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            Vector3 targetVelocity = moveDirection.normalized * moveSpeed;
            float acceleration = isGrounded ? groundAcceleration : airAcceleration * airMultiplier;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else if (isGrounded)
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, groundAcceleration * Time.fixedDeltaTime);
        }

        if (horizontalVelocity.magnitude > moveSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * moveSpeed;
        }

        rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
    }

    private void ApplyGravity()
    {
        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, groundedStickVelocity, rb.linearVelocity.z);
            return;
        }

        Vector3 velocity = rb.linearVelocity;
        velocity.y += gravity * Time.fixedDeltaTime;
        velocity.y = Mathf.Max(velocity.y, terminalVelocity);
        rb.linearVelocity = velocity;
    }

    private Vector3 GetMoveDirection()
    {
        if (orientation == null)
        {
            return Vector3.zero;
        }

        Vector3 forward = Vector3.ProjectOnPlane(orientation.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(orientation.right, Vector3.up).normalized;
        return forward * moveInput.y + right * moveInput.x;
    }
}

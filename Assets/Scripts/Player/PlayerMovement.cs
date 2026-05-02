using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private Vector3 moveDirection;
    private Vector2 moveInput;
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

    [Header("Ground Check")]
    public float groundCheckRadius = 0.28f;
    public float groundCheckOffset = 0.08f;

    [Header("Air Jump")]
    public int maxAirJumps = 3;
    public float airJumpRefillInterval = 0.35f;

    [Header("Gravity")]
    public float groundAcceleration = 40f;
    public float airAcceleration = 14f;
    public float gravity = -30f;
    public float terminalVelocity = -40f;
    public float groundedStickVelocity = -2f;

    [Header("Sounds")]
    public float footstepDelay = 0.4f;

    private bool wasGrounded;
    private float footstepTimer;

    public Vector3 Velocity => rb != null ? rb.linearVelocity : Vector3.zero;
    public int AvailableAirJumps => availableAirJumps;
    public int MaxAirJumps => maxAirJumps;

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

        maxAirJumps = Mathf.Max(0, maxAirJumps);
        availableAirJumps = maxAirJumps;
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        jumpForce = Mathf.Max(0f, jumpForce);
        airMultiplier = Mathf.Max(0f, airMultiplier);
        groundDrag = Mathf.Max(0f, groundDrag);
        playerHeight = Mathf.Max(0f, playerHeight);
        groundCheckRadius = Mathf.Max(0.01f, groundCheckRadius);
        groundCheckOffset = Mathf.Max(0f, groundCheckOffset);
        maxAirJumps = Mathf.Max(0, maxAirJumps);
        airJumpRefillInterval = Mathf.Max(0f, airJumpRefillInterval);
        groundAcceleration = Mathf.Max(0f, groundAcceleration);
        airAcceleration = Mathf.Max(0f, airAcceleration);
        gravity = Mathf.Min(0f, gravity);
        terminalVelocity = Mathf.Min(0f, terminalVelocity);
        groundedStickVelocity = Mathf.Min(0f, groundedStickVelocity);
        footstepDelay = Mathf.Max(0f, footstepDelay);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpRequested = true;
        }
    }

    public void ResetForPlaytest(Vector3 position)
    {
        transform.position = position;

        if (rb != null)
        {
            rb.position = position;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        moveInput = Vector2.zero;
        moveDirection = Vector3.zero;
        jumpRequested = false;
        airJumpRegenTimer = 0f;
        availableAirJumps = maxAirJumps;
        isGrounded = false;
        wasGrounded = false;
        footstepTimer = 0f;
    }

    private void Update()
    {
        UpdateGroundedState();
        UpdateLandingSound();
        UpdateAirJumpRefill();
        UpdateFootsteps();

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

    private void UpdateLandingSound()
    {
        if (!wasGrounded && isGrounded)
        {
            SoundManager.PlaySound(SoundType.Player_Land);
        }

        wasGrounded = isGrounded;
    }

    private void UpdateAirJumpRefill()
    {
        if (!isGrounded || availableAirJumps >= maxAirJumps)
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

    private void UpdateFootsteps()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (isGrounded && horizontalVelocity.magnitude > 0.1f)
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
            footstepTimer = 0f;
        }
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
        SoundManager.PlaySound(SoundType.Player_Jump);
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

    public void ApplyDashForce(Vector3 direction, float force)
    {
        // Zero out current velocity first for a consistent dash distance
        rb.linearVelocity = Vector3.zero;

        // Apply the burst of speed
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}

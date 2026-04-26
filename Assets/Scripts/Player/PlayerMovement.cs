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
    }

}
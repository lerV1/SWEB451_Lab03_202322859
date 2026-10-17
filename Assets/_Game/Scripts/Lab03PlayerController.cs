using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
public class Lab03PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference sprintAction;
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpImpulse = 6f;
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private LayerMask groundLayers;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpRequested;

    private void Awake()
    {
        // TODO 1: Get the Rigidbody attached to this GameObject.
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        // TODO 2: Enable Move, Jump, and Sprint.
        moveAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();

        // TODO 3: Subscribe OnJump to the Jump performed event.
        jumpAction.action.performed += OnJump;
    }
    private void OnDisable()
    {
        // TODO 4: Unsubscribe before disabling the Actions.
        jumpAction.action.performed -= OnJump;
        moveAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();
    }
    private void Update()
    {
        // TODO 5: Read Move as Vector2 and store it in moveInput.
        moveInput = moveAction.action.ReadValue<Vector2>();

    }
    private void FixedUpdate()
    {
        // TODO 6: Convert moveInput to X/Z world movement.
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        // TODO 7: Prevent diagonal movement from becoming faster.
        if (moveDirection.magnitude > 1f)
        {
            moveDirection = moveDirection.normalized;
        }
        // TODO 8: Choose walk speed or Sprint speed
        float currentSpeed;
        if (sprintAction.action.IsPressed())
        {
            currentSpeed = moveSpeed * sprintMultiplier;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
        // TODO 9: Set horizontal linearVelocity and preserve Y velocity.
        rb.linearVelocity = new Vector3(
            moveDirection.x * currentSpeed,
            rb.linearVelocity.y,
            moveDirection.z * currentSpeed);
        // TODO 10: If a jump was requested and the Player is grounded,
        // apply an upward impulse.
        if (IsGrounded() == true && jumpRequested == true)
        {
            rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
        }
        // TODO 11: Clear jumpRequested after checking it.
        jumpRequested = false;
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        // TODO 12: Queue the jump for FixedUpdate.
        jumpRequested = true;
    }
    private bool IsGrounded()
    {
        // TODO 13: Cast a ray down using groundCheckDistance and groundLayers.
        bool Gorunded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayers);
        return Gorunded;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
public class Lab03PlayerController : MonoBehaviour
{
    [Header("Sprint Feedback")]
    [SerializeField] private TrailRenderer sprintTrail;
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference resetAction;
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpImpulse = 6f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private LayerMask groundLayers;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpRequested;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        // TODO 1: Get the Rigidbody attached to this GameObject.
        rb = GetComponent<Rigidbody>();

        // Option A: store the start pose once.
        startPosition = rb.position;
        startRotation = rb.rotation;
    }
    private void OnEnable()
    {
        // TODO 2: Enable Move, Jump, and Sprint.
        moveAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();

        // TODO 3: Subscribe OnJump to the Jump performed event.
        jumpAction.action.performed += OnJump;

        resetAction.action.Enable();
        resetAction.action.performed += OnReset;
    }
    private void OnDisable()
    {
        // TODO 4: Unsubscribe before disabling the Actions.
        jumpAction.action.performed -= OnJump;
        resetAction.action.performed -= OnReset;
        moveAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();
        resetAction.action.Disable();
    }
    private void Update()
    {
        // TODO 5: Read Move as Vector2 and store it in moveInput.
        moveInput = moveAction.action.ReadValue<Vector2>();
        if (sprintTrail != null)
        {
            sprintTrail.emitting =
                sprintAction.action.IsPressed() &&
                moveInput.sqrMagnitude > 0.01f;
        }

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
        Vector3 currentHorizontal = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );

        Vector3 targetHorizontal = moveDirection * currentSpeed;

        float rate;

        if (moveDirection.magnitude > 0.01f)
        {
            rate = acceleration;
        }
        else
        {
            rate = deceleration;
        }

        Vector3 nextHorizontal = Vector3.MoveTowards(
            currentHorizontal,
            targetHorizontal,
            rate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(
            nextHorizontal.x,
            rb.linearVelocity.y,
            nextHorizontal.z
        );


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
    private void OnReset(InputAction.CallbackContext context)
    {
        ResetPlayer();
    }
    // Option A: return to the saved start position and clear all motion.
    private void ResetPlayer()
    {
        rb.position = startPosition;
        rb.rotation = startRotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        jumpRequested = false;

        if (sprintTrail != null)
        {
            sprintTrail.Clear();
        }
    }
    private bool IsGrounded()
    {
        // TODO 13: Cast a ray down using groundCheckDistance and groundLayers.
        bool Gorunded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayers);
        return Gorunded;
    }
}
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Movement speed in units per second")]
    public float speed = 5f;

    // InputAction can be set up in code or wired from an Input Action asset in the inspector.
    [Tooltip("Optional: assign an InputAction that provides a Vector2 for movement (or leave null to create a default one)")]
    public InputAction movementAction;

    // Internal state
    private Rigidbody2D rb;
    private Vector2 moveInput = Vector2.zero;
    private bool createdAction = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // If no action assigned in inspector, create a simple one that maps to arrow keys, WASD and gamepad stick
        if (movementAction == null || movementAction.bindings.Count == 0)
        {
            movementAction = new InputAction("Move", InputActionType.Value, "<Gamepad>/leftStick");
            // Add 2D Vector composite for keyboard arrows and WASD
            movementAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");

            createdAction = true;
        }

        // Register callbacks
        movementAction.performed += OnMovementPerformed;
        movementAction.canceled += OnMovementCanceled;
    }

    void OnEnable()
    {
        movementAction?.Enable();
    }

    void OnDisable()
    {
        movementAction?.Disable();
    }

    void OnDestroy()
    {
        // Unregister callbacks and dispose created action
        if (movementAction != null)
        {
            movementAction.performed -= OnMovementPerformed;
            movementAction.canceled -= OnMovementCanceled;
            if (createdAction)
            {
                movementAction.Dispose();
                movementAction = null;
            }
        }
    }

    private void OnMovementPerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    // Use FixedUpdate for physics-driven movement
    void FixedUpdate()
    {
        if (rb == null) return;

        Vector2 currentPos = rb.position;
        Vector2 delta = moveInput * speed * Time.fixedDeltaTime; // frame-rate independent using fixed timestep
        Vector2 targetPos = currentPos + delta;

        // MovePosition provides smooth, interpolation-friendly movement when using Rigidbody2D
        rb.MovePosition(targetPos);
    }
}

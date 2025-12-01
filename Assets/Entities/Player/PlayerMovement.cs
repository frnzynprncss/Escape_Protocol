using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference move_inputs;
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        input = move_inputs.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.velocity = input * moveSpeed;
    }
}

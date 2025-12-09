using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Control Layout")]
    public PlayerControl controls;
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        input = get_input();
    }

    private void FixedUpdate()
    {
        rb.velocity = input * moveSpeed * Time.deltaTime;
    }

    public Vector2 get_input()
    {
        float x_input = 0f;
        float y_input = 0f;

        if (Input.GetKey(controls.move_right)) x_input = 1f;
        if (Input.GetKey(controls.move_left)) x_input = -1f;
        if (Input.GetKey(controls.move_up)) y_input = 1f;
        if (Input.GetKey(controls.move_down)) y_input = -1f;

        return new Vector2(x_input, y_input).normalized;
    }
}

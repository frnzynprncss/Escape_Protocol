using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Control Layout")]
    public PlayerControl controls;
    public GameObject weapon_pivot;
    public float moveSpeed = 5f;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;
    private Vector2 input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        input = get_input();
        rotate_weapon();

        update_animation_values();
    }

    private void FixedUpdate()
    {
        rb.velocity = input * moveSpeed * Time.deltaTime;
    }

    private void rotate_weapon()
    {
        if (input == Vector2.zero) return;

        float angle_radians = Mathf.Atan2(input.y, input.x);
        float angle_degrees = angle_radians * Mathf.Rad2Deg;
        
        Quaternion weapon_rotation = Quaternion.Euler(0f, 0f, angle_degrees);
        weapon_pivot.transform.rotation = weapon_rotation;
    }

    private void update_animation_values()
    {
        if (animator == null) return;

        // Use "Speed" exactly as shown in your Animator Parameters tab.
        animator.SetFloat("Speed", input.magnitude);

        // You can remove these if you are only blending Idle/Run and not direction:
        // animator.SetFloat("Horizontal", input.x);
        // animator.SetFloat("Vertical", input.y);
    }

    private Vector2 get_input()
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

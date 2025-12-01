using UnityEngine;

public class PlatformerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Keybinds")]
    public KeyCode keyLeft = KeyCode.A;
    public KeyCode keyRight = KeyCode.D;
    public KeyCode keyUp = KeyCode.W;
    public KeyCode keyDown = KeyCode.S;

    private Rigidbody2D rb;
    private Vector2 respawnPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1f;
            rb.freezeRotation = true;
        }

        if (GetComponent<Collider2D>() == null)
            gameObject.AddComponent<BoxCollider2D>();

        respawnPos = transform.position;
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(keyLeft)) horizontal = -1f;
        if (Input.GetKey(keyRight)) horizontal = 1f;
        if (Input.GetKey(keyUp)) vertical = 1f;
        if (Input.GetKey(keyDown)) vertical = -1f;

        Vector2 move = new Vector2(horizontal, vertical).normalized;


        if (horizontal > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (horizontal < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        rb.velocity = move * moveSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
            gameObject.SetActive(false);
    }

    public void Respawn()
    {
        transform.position = respawnPos;
        rb.velocity = Vector2.zero;
    }
}

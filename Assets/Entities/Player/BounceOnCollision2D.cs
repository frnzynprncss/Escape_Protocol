using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BounceOnCollision2D : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce = 8f;
    [SerializeField] private float cooldown = 0.15f;

    private Rigidbody2D rb;
    private float lastBounceTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time - lastBounceTime < cooldown) return;

        // Only Player <-> Enemy interaction
        if (!collision.gameObject.CompareTag("Player") &&
            !collision.gameObject.CompareTag("Enemy"))
            return;

        Vector2 bounceDir = (rb.position - collision.rigidbody.position).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(bounceDir * bounceForce, ForceMode2D.Impulse);

        lastBounceTime = Time.time;
    }
}

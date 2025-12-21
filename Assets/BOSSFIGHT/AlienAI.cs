using UnityEngine;
using System.Collections;

// This little class mimics what your HealthComponent expects
public class TemporaryAttack : AttackComponent
{
    public int damage;
}

public class AlienAI : MonoBehaviour
{
    [Header("Targeting")]
    public string targetTag = "Player";

    [Header("Movement Settings")]
    public float speed = 3f;
    public float animationDelay = 0.2f;
    public float stoppingDistance = 1.5f;

    [Header("Combat Settings")]
    public int damageAmount = 10;
    public float damageRate = 0.5f;
    private float nextDamageTime;

    [Header("Directional Sprites")]
    public Sprite moveUp_1;
    public Sprite moveUp_2;
    public Sprite moveSide_1;
    public Sprite moveSide_2;

    private SpriteRenderer sr;
    private Transform player;
    private float animTimer;
    private int frameIndex = 0;
    private Rigidbody2D rb;
    private float orbitDirection;
    private bool isTouchingPlayer = false;
    private Coroutine damageEffectRoutine;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        orbitDirection = (Random.value > 0.5f) ? 1f : -1f;
        FindClosestPlayer();
    }

    void Update()
    {
        FindClosestPlayer();

        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            MoveWithPhysics(direction);
            AnimateDirectionalSprite(direction);
        }
        else
        {
            // If no players are alive/found, stop moving
            if (rb != null) rb.velocity = Vector2.zero;
        }
    }

    void FindClosestPlayer()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (GameObject t in targets)
        {
            // NEW: Check if the player is actually alive before targeting
            HealthComponent h = t.GetComponent<HealthComponent>();
            if (h != null && h.health > 0)
            {
                float distance = Vector2.Distance(transform.position, t.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = t.transform;
                }
            }
        }
        player = closestTarget;
    }

    void MoveWithPhysics(Vector2 dirToPlayer)
    {
        if (isTouchingPlayer)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 finalVelocity = dirToPlayer * speed;

        if (distance < stoppingDistance)
        {
            Vector2 orbitDir = new Vector2(-dirToPlayer.y, dirToPlayer.x) * orbitDirection;
            finalVelocity = (dirToPlayer * 0.1f + orbitDir * 0.9f).normalized * speed;
        }

        rb.velocity = finalVelocity;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            // Extra safety: only count as "touching" if they are alive
            HealthComponent health = collision.gameObject.GetComponent<HealthComponent>();
            if (health != null && health.health > 0)
            {
                isTouchingPlayer = true;

                if (Time.time >= nextDamageTime)
                {
                    AttackComponent attacker = GetComponent<AttackComponent>();
                    if (attacker != null)
                    {
                        health.take_damage(attacker);
                        nextDamageTime = Time.time + damageRate;
                    }
                    else
                    {
                        Debug.LogWarning("Alien is missing an AttackComponent!");
                    }
                }
            }
            else
            {
                isTouchingPlayer = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            HealthComponent h = collision.gameObject.GetComponent<HealthComponent>();
            if (h != null && h.health > 0) isTouchingPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag)) isTouchingPlayer = false;
    }

    public void TakeDamage()
    {
        if (damageEffectRoutine != null) StopCoroutine(damageEffectRoutine);
        damageEffectRoutine = StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    void AnimateDirectionalSprite(Vector2 dir)
    {
        animTimer += Time.deltaTime;
        if (animTimer >= animationDelay)
        {
            frameIndex = (frameIndex == 0) ? 1 : 0;
            animTimer = 0;
        }

        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            sr.sprite = (frameIndex == 0) ? moveUp_1 : moveUp_2;
            sr.flipX = false;
        }
        else
        {
            sr.sprite = (frameIndex == 0) ? moveSide_1 : moveSide_2;
            sr.flipX = dir.x < 0;
        }
    }

    void OnMouseDown()
    {
        TakeDamage();
        Destroy(gameObject, 0.1f);
    }
}
using UnityEngine;

public class TentacleLinkDamage : MonoBehaviour
{
    [Header("Offense Settings")]
    public int damageToPlayer = 100; // Instant kill for testing
    public float knockbackForce = 60f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we touched is the Player
        if (other.CompareTag("Player"))
        {
            HealthComponent playerHealth = other.GetComponent<HealthComponent>();

            if (playerHealth != null)
            {
                // Create the attack using your AttackComponent script
                AttackComponent tentacleAttack = gameObject.AddComponent<AttackComponent>();
                tentacleAttack.set_attack(damageToPlayer);

                // Apply damage to player
                playerHealth.take_damage(tentacleAttack);

                // Verify in the Console
                Debug.Log("<color=red>PLAYER HIT!</color> Health remaining: " + playerHealth.health);

                Destroy(tentacleAttack);
            }

            // Apply Knockback to push the player away
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                playerRb.velocity = Vector2.zero;
                playerRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}
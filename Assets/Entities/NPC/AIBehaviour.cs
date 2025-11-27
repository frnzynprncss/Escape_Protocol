using System.Collections;
using UnityEngine;




public enum AIState
{
    IDLE,
    CHASING,
    ATTACKING
}

public class AIBehaviour : MonoBehaviour
{
    [Header("AI Behaviour")]
    public AIState current_state;
    public Transform target;

    public float move_speed = 10f;

    [Header("Attack Properties")]
    public int attack_damage = 1;
    public float attack_cooldown = 1f;
    public float attack_range = 1f;
    public bool is_attacking { get; private set; } = false;

    private void FixedUpdate()
    {
        switch (current_state)
        {
            case AIState.ATTACKING:
                if (is_attacking) return;
                StartCoroutine(attack_target()); break;
            case AIState.CHASING: move_towards_target(); break;
            case AIState.IDLE: break;
        }
    }

    private IEnumerator attack_target()
    {
        is_attacking = true;
            
        // Attack Target Here
        print("Attacking Target");

        yield return new WaitForSeconds(attack_cooldown);
        is_attacking = false;

        if (!near_target())
        {
            current_state = AIState.CHASING;
        }
    }

    private void move_towards_target()
    {
        if (target == null) return;
        if (near_target())
        {
            current_state = AIState.ATTACKING;
        }

        transform.parent.position = Vector3.MoveTowards(transform.position, target.position, move_speed * Time.deltaTime);
    }

    private bool near_target()
    {
        return Vector2.Distance(transform.parent.position, target.position) < attack_range;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        target = collision.gameObject.transform;
        current_state = AIState.CHASING;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        target = null;
        current_state = AIState.IDLE;
    }
}



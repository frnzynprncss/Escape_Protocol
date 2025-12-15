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
    public float search_radius = 10f;

    [Header("Attack Properties")]
    public AttackComponent attack_data;
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

            case AIState.IDLE: search_players(); break;
        }
    }


    private IEnumerator attack_target()
    {
        is_attacking = true;
        target.GetComponent<HealthComponent>()?.take_damage(attack_data);

        yield return new WaitForSeconds(attack_cooldown);

        is_attacking = false;

        if (!near_target())
        {
            current_state = AIState.CHASING;
        }
    }

    private void search_players()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, search_radius);

        foreach (Collider2D objects in hit)
        {
            if (objects.gameObject.CompareTag("Player"))
            {
                target = objects.gameObject.transform;
                current_state = AIState.CHASING;
            }
        }
    }

    private void move_towards_target()
    {
        // If target is far away from search radius, they switch to the Idle State
        if (target == null) return;
        if (Vector2.Distance(transform.position, target.position) > search_radius) current_state = AIState.IDLE;
        if (near_target()) current_state = AIState.ATTACKING;

        transform.parent.position = Vector3.MoveTowards(transform.position, target.position, move_speed * Time.deltaTime);
    }

    private bool near_target()
    {
        if (target == null) return false;
        return Vector2.Distance(transform.parent.position, target.position) < attack_range;
    }
}
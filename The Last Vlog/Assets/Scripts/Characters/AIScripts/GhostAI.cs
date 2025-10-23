using System.Collections;
using Pathfinding;
using UnityEngine;

public enum GhostState
{
    WANDERING,
    CHASING
}

public class GhostAI : MonoBehaviour
{
    [SerializeField] private AIDestinationSetter ai_destination;
    [SerializeField] private string[] trap_tags;

    public GhostState current_state { get; private set; } = GhostState.WANDERING;

    [Header("AI Values")]
    public float sight_range;
    public float chase_duration;
    public float attack_range;

    public Transform player;
    public LayerMask collision_layer;
    public Jumpscare jumpscare;

    private float player_seen_time = 0f;
    
    [Header("Activation Values")]
    public float time_until_activation = 3f;
    public bool looking_for_player = true;
    public Transform hide_position;
    public Transform[] spawn_points;
    public Transform[] idle_points;

    private void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        ai_behavior();
    }

    public virtual void ai_behavior()
    {
        if (!looking_for_player) return;

        if (Vector3.Distance(player.position, transform.position) < attack_range)
        {
            StartCoroutine(jumpscare_player());
        }

        bool seen_player = sees_player();

        if (!seen_player)
        {
            player_seen_time += Time.deltaTime;
        }

        if (current_state == GhostState.WANDERING)
        {
            player_seen_time = 0f;
            if (seen_player && current_state != GhostState.CHASING)
            {
                set_state(GhostState.CHASING);
            }
        }
        else if (current_state == GhostState.CHASING && player_seen_time >= chase_duration)
        {
            set_state(GhostState.WANDERING);
        }

        switch (current_state)
        {
            case GhostState.CHASING: move_to_target(); break;
            case GhostState.WANDERING: idle(); break;
        }
    }

    public bool sees_player()
    {
        if (player == null) return false;

        Vector3 dir = (player.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, sight_range, collision_layer);
        
        Debug.DrawRay(transform.position, dir * sight_range, Color.red);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.DrawRay(transform.position, dir * sight_range, Color.green);
                return true;
            }
        }

        return false;
    }
    
    public void move_to_target()
    {
        ai_destination.target = player;
    }

    public void idle()
    {
        if (idle_points.Length <= 0)
        {
            ai_destination.target = transform;
            return;
        }
        if (ai_destination.target == null) ai_destination.target = idle_points[Random.Range(0, idle_points.Length)];

        if (near_target() || ai_destination.target == player.transform)
        {
            ai_destination.target = idle_points[Random.Range(0, idle_points.Length)];
        }
    }

    public IEnumerator jumpscare_player()
    {
        jumpscare.ShowJumpscare();
        de_activate();

        yield return new WaitForSeconds(time_until_activation);
        activate();
        looking_for_player = true;
    }


    public void de_activate()
    {
        looking_for_player = false;

        transform.parent.position = hide_position.position;
    }

    public void activate()
    {
        looking_for_player = true;
        
        int rand_spawn = Random.Range(0, spawn_points.Length);
        transform.parent.position = spawn_points[rand_spawn].position;
    }
    
    public void set_state(GhostState state)
    {
        current_state = state;
        print($"State: {state}");
    }

    public virtual void trap_collided()
    {
        // custom function on what the ghost reacts when collided with the trap
        de_activate();
    }


    public virtual void light_collided()
    {
        // custom function on what the ghost reacts when the player flashes their light on them
    }

    public bool near_target()
    {
        return Vector3.Distance(transform.position, ai_destination.target.position) < attack_range;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (trap_tags.Length > 0)
        {
            foreach (string tag in trap_tags)
            {
                if (collision.CompareTag(tag))
                {
                    trap_collided();
                }
            }
        }

        if (collision.CompareTag("Flashlight"))
        {
            light_collided();
        }
    }
}

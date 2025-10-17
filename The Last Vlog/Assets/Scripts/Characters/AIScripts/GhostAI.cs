using Pathfinding;
using UnityEditor;
using UnityEngine;

public enum GhostState
{
    WANDERING,
    CHASING
}

public class GhostAI : MonoBehaviour
{
    [SerializeField] private AIDestinationSetter ai_destination;
    [SerializeField] private string[] trap_layers;

    public GhostState current_state { get; private set; } = GhostState.WANDERING;
    public Transform player;

    public float sight_range;
    public float chase_duration;
    public float attack_range;

    private float player_seen_time;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    private void Update()
    {
        if (!sees_player())
        {
            player_seen_time += Time.deltaTime;
        }

        ai_behavior();
    }

    private bool sees_player()
    {
        if (player == null) return false;

        Vector3 dir = player.position - transform.position;
        RaycastHit raycast;

        if (Physics.Raycast(transform.position, dir, out raycast, sight_range))
        {
            if (raycast.transform == player)
            {
                transform.LookAt(player);
                return true;
            }
        }

        return false;
    }

    private void move_to_target()
    {
        if (player == null) return;

        ai_destination.target = player;
    }

    private void idle()
    {
        ai_destination.target = transform;
    }

    public virtual void ai_behavior()
    {
        /* TODO:
            if Raycast sees player, enter CHASING State
                if near_target() jumpscare the player and disappear

                if Raycast does not see player for a few seconds, return to WANDERING State
            else if Raycast does not see player, just wonder around points
        */

        bool seen_player = sees_player();

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
    
    public void set_state(GhostState state)
    {
        current_state = state;
        print($"State: {state}");
    }

    public virtual void trap_collided()
    {
        // custom function on what the ghost reacts when collided with the trap
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
        // if collides with a trap
        // if (collision.CompareTag("TargetName")) trap_collided;

        // if collides with the flashlight
        // if (collision.CompareTag("Flashlight")) light_collided();
    }
}

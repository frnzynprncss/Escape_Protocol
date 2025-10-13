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
    [SerializeField] private string[] trap_layers;

    public float sight_range;
    public float attack_range;

    private void Update()
    {
        ai_behavior();
    }

    public virtual void ai_behavior()
    {
        /* TODO:
            if Raycast sees player, enter CHASING State
                if near_target() jumpscare the player and disappear

                if Raycast does not see player for a few seconds, return to WANDERING State
            else if Raycast does not see player, just wonder around points
        */
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

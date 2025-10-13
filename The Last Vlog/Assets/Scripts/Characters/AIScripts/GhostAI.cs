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

    public float sight_range;
    public float attack_range;

    private void Update()
    {
        /* TODO:
            if Raycast sees player, enter CHASING State
                if near_target() jumpscare the player and disappear

                if Raycast does not see player for a few seconds, return to WANDERING State
            else if Raycast does not see player, just wonder around points
        */
    }

    public bool near_target()
    {
        return Vector3.Distance(transform.position, ai_destination.target.position) < attack_range;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if collides with a trap
        // if (collision.CompareTag("TargetName")) queue_free();
    }
}

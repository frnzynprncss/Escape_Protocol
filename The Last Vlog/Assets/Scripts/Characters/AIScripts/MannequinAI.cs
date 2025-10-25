using UnityEngine;

public class MannequinAI : GhostAI
{
    [Header("Mannequin")]
    public bool is_flashlighted = false;

    private void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player)
        {
            ai_destination.target = player;
        }
    }

    private void Update()
    {
        ai_behavior();
    }

    private void LateUpdate()
    {
        is_flashlighted = false;
    }

    public override void ai_behavior()
    {
        if (player == null) return;

        if (!looking_for_player || is_flashlighted)
        {
            ai_destination.target = transform;
        }
        else
        {
            ai_destination.target = player.transform;
        }

        if (Vector3.Distance(player.position, transform.position) < attack_range)
        {
            StartCoroutine(jumpscare_player());
        }
    }
}

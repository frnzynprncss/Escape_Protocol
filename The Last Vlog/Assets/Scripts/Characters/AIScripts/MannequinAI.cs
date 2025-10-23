using UnityEngine;

public class MannequinAI : GhostAI
{
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

    public override void ai_behavior()
    {
        if (player == null) return;

        if (!looking_for_player)
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

    public override void light_collided()
    {
        looking_for_player = false;
    }
}

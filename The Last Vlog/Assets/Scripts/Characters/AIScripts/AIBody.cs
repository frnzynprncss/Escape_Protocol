using Pathfinding;
using UnityEngine;

public class AIBody : MonoBehaviour
{
    [SerializeField] private AIPath ai_path;

    private void Update()
    {
        flip_sprite();
        play_animation();
    }

    private void play_animation()
    {
        
    }
    
    private void flip_sprite()
    {
        if (!ai_path) return;
        if (ai_path.desiredVelocity.x == 0) return;

        Vector3 scale = transform.localScale;

        if (ai_path.desiredVelocity.x < 0f)
        {
            scale.x = -1;
        }
        else
        {
            scale.x = 1;
        }

        transform.localScale = scale;
    }
}

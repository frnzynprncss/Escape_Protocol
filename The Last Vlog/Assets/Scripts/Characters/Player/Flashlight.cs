using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] LayerMask collision_layers;
    [SerializeField] float flashlight_range = 10f;
    [SerializeField] float flashlight_angle = 30f;
    [SerializeField] int ray_count = 15;

    private void Update()
    {
        cast_rays();
    }

    private void cast_rays()
    {
        float starting_angle = -flashlight_angle / 2f;
        float angle_incrememnt = flashlight_angle / ray_count;

        for (int i = 0; i < ray_count; i++)
        {
            float current_angle = starting_angle + i * angle_incrememnt;
            Vector3 ray_direction = Quaternion.Euler(0, 0, current_angle) * transform.right;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, ray_direction, flashlight_range, collision_layers);

            if (hit.collider != null)
            {
                Debug.DrawRay(transform.position, ray_direction * flashlight_range, Color.green);

                MannequinAI mannequin = hit.collider.gameObject.GetComponentInChildren<MannequinAI>();
                if (mannequin != null)
                {
                    mannequin.is_flashlighted = true;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, ray_direction * flashlight_range, Color.red);
            }
        }
    }
}

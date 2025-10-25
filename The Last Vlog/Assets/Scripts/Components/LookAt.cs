using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private float look_speed = 10f;
    private Camera main_cam;

    void Start()
    {
        main_cam = Camera.main;
    }

    void Update()
    {
        Vector3 mouse_position = (Vector2)main_cam.ScreenToWorldPoint(Input.mousePosition);
        float angle_rad = Mathf.Atan2(mouse_position.y - transform.position.y, mouse_position.x - transform.position.x);
        float angle_deg = (180 / Mathf.PI) * angle_rad;
        
        Quaternion target_rotation = Quaternion.Euler(0f, 0f, angle_deg);
        transform.rotation = Quaternion.Lerp(transform.rotation, target_rotation, look_speed * Time.deltaTime);
    }
}

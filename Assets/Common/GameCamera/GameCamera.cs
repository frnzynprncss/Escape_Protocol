using System.Collections;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private Camera game_camera;
    public Transform player1;
    public Transform player2;
    [SerializeField] private Transform default_cam_position;

    [Header("Zoom Values")]
    [SerializeField] private float default_zoom = 5f;
    [SerializeField] private float smooth_speed;
    [SerializeField] private float zoom_margin;
    [SerializeField] private float min_zoom;
    [SerializeField] private float max_zoom;

    public bool follow_players;
    private Coroutine current_shake;

    private void Start()
    {
        game_camera.orthographicSize = default_zoom;
    }

    private void LateUpdate()
    {
        if (follow_players && player1 != null && player2 != null)
        {
            Vector3 center_point = get_center(player1, player2);
            move_camera(center_point);
            dual_zoom(player1, player2);
        }
        else if (follow_players && player1 != null && player2 == null)
        {
            move_camera(player1.position);
            zoom_camera(default_zoom);
        }
        else if (follow_players && player1 == null && player2 != null)
        {
            move_camera(player2.position);
            zoom_camera(default_zoom);
        }
        else
        {
            move_camera(default_cam_position.position);
            zoom_camera(default_zoom);
        }
    }

    private Vector3 get_center(Transform target1, Transform target2)
    {
        Vector3 center = (target1.position + target2.position) / 2f;
        center.z = transform.position.z;

        return center;
    }

    private void move_camera(Vector3 target_position)
    {
        target_position.z = transform.position.z;

        Vector3 smoothed_pos = Vector3.Lerp(transform.position, target_position, smooth_speed * Time.deltaTime);
        transform.position = smoothed_pos;
    }

    private void zoom_camera(float zoom_level)
    {
        game_camera.orthographicSize = Mathf.Lerp(game_camera.orthographicSize, zoom_level, smooth_speed * Time.deltaTime);
    }

    private void dual_zoom(Transform target1, Transform target2)
    {
        float distance = Vector3.Distance(target1.position, target2.position);
        float size = distance / 2f + zoom_margin;
        float target_size = Mathf.Clamp(size, min_zoom, max_zoom);

        zoom_camera(target_size);
    }

    public void shake_camera(float duration, float magnitude)
    {
        if (current_shake != null)
        {
            StopCoroutine(current_shake);
        }

        current_shake = StartCoroutine(ShakeCoroutine());
        IEnumerator ShakeCoroutine()
        {
            float elapsed_time = 0f;

            while (elapsed_time < duration)
            {
                float decay_rate = 1f - (elapsed_time / duration);
                float x = Random.Range(-magnitude, magnitude) * decay_rate;
                float y = Random.Range(-magnitude, magnitude) * decay_rate;

                game_camera.transform.position = transform.position + new Vector3(x, y, transform.position.z);

                elapsed_time += Time.deltaTime;
                yield return null;
            }

            game_camera.transform.position = transform.position;
            current_shake = null;
        }
    }
}
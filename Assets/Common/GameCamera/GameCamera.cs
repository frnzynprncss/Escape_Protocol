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
    [SerializeField] private float smooth_speed = 5f;
    [SerializeField] private float zoom_margin = 2f;
    [SerializeField] private float min_zoom = 3f;
    [SerializeField] private float max_zoom = 10f;

    public bool follow_players = true;
    private Coroutine current_shake;

    private void OnEnable()
    {
        RoomFirstDungeonGenerator.OnPlayersSpawned += SetPlayers;
    }

    private void OnDisable()
    {
        RoomFirstDungeonGenerator.OnPlayersSpawned -= SetPlayers;
    }

    private void LateUpdate()
    {
        if (follow_players && player1 != null && player2 != null)
        {
            Vector3 center = (player1.position + player2.position) / 2f;
            center.z = transform.position.z;
            MoveCamera(center);
            DualZoom(player1, player2);
        }
        else if (follow_players && player1 != null)
        {
            MoveCamera(player1.position);
            ZoomCamera(default_zoom);
        }
        else if (follow_players && player2 != null)
        {
            MoveCamera(player2.position);
            ZoomCamera(default_zoom);
        }
        else
        {
            MoveCamera(default_cam_position.position);
            ZoomCamera(default_zoom);
        }
    }

    private void MoveCamera(Vector3 target)
    {
        target.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, target, smooth_speed * Time.deltaTime);
    }

    private void ZoomCamera(float zoom)
    {
        game_camera.orthographicSize = Mathf.Lerp(game_camera.orthographicSize, zoom, smooth_speed * Time.deltaTime);
    }

    private void DualZoom(Transform t1, Transform t2)
    {
        float distance = Vector3.Distance(t1.position, t2.position);
        float size = distance / 2f + zoom_margin;
        float targetSize = Mathf.Clamp(size, min_zoom, max_zoom);
        ZoomCamera(targetSize);
    }

    public void SetPlayers(Transform p1, Transform p2)
    {
        player1 = p1;
        player2 = p2;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        if (current_shake != null)
            StopCoroutine(current_shake);
        current_shake = StartCoroutine(ShakeCoroutine());

        IEnumerator ShakeCoroutine()
        {
            float elapsed = 0f;
            Vector3 originalPos = transform.position;

            while (elapsed < duration)
            {
                float decay = 1f - (elapsed / duration);
                float x = Random.Range(-magnitude, magnitude) * decay;
                float y = Random.Range(-magnitude, magnitude) * decay;

                game_camera.transform.position = originalPos + new Vector3(x, y, originalPos.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            game_camera.transform.position = originalPos;
            current_shake = null;
        }
    }
}

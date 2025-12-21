using System.Collections;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private Camera game_camera;
    public Transform player1;
    public Transform player2;
    [SerializeField] private Transform default_cam_position;

    // --- NEW: References to Health to check if players are dead ---
    private HealthComponent health1;
    private HealthComponent health2;

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
        if (!follow_players) return;

        // Check if players are actually alive (Health > 0)
        bool p1Alive = IsPlayerAlive(player1, health1);
        bool p2Alive = IsPlayerAlive(player2, health2);

        // CASE 1: Both Alive -> Follow Center & Dual Zoom
        if (p1Alive && p2Alive)
        {
            Vector3 center = (player1.position + player2.position) / 2f;
            center.z = transform.position.z;
            MoveCamera(center);
            DualZoom(player1, player2);
        }
        // CASE 2: Only Player 1 is Alive -> Focus solely on P1 (Normal Zoom)
        else if (p1Alive)
        {
            MoveCamera(player1.position);
            ZoomCamera(default_zoom); // Reset zoom so it doesn't stay expanded
        }
        // CASE 3: Only Player 2 is Alive -> Focus solely on P2 (Normal Zoom)
        else if (p2Alive)
        {
            MoveCamera(player2.position);
            ZoomCamera(default_zoom); // Reset zoom so it doesn't stay expanded
        }
        // CASE 4: Both Dead -> Return to default
        else
        {
            if (default_cam_position != null)
                MoveCamera(default_cam_position.position);

            ZoomCamera(default_zoom);
        }
    }

    // --- HELPER TO CHECK HEALTH ---
    private bool IsPlayerAlive(Transform playerTransform, HealthComponent playerHealth)
    {
        // 1. If no player object, they aren't alive
        if (playerTransform == null) return false;

        // 2. If object is disabled, they aren't alive
        if (!playerTransform.gameObject.activeInHierarchy) return false;

        // 3. (THE FIX) If Health is 0 or less, consider them dead for the camera
        if (playerHealth != null && playerHealth.health <= 0) return false;

        return true;
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

        // Automatically find the HealthComponents when players spawn
        if (player1 != null) health1 = player1.GetComponent<HealthComponent>();
        if (player2 != null) health2 = player2.GetComponent<HealthComponent>();
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
            // We don't snap back here because LateUpdate will handle it next frame
            current_shake = null;
        }
    }
}
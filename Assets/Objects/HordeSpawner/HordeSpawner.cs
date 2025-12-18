using System.Collections;
using UnityEngine;

public class HordeSpawner : MonoBehaviour
{
    /// <summary>
    /// The center of the player's position, use the GameCamera game object
    /// for the center position of both players
    /// </summary>
    public Transform player_center;
    public GameObject enemy_prefab;

    [Header("Spawning Values")]
    [Range(1, 100f)] public int min_enemy_size;
    [Range(1, 100f)] public int max_enemy_size;
    [Range(1, 100f)] public float min_spawn_time;
    [Range(1, 100f)] public float max_spawn_time;
    public float enemy_spawn_offset;
    /// <summary>
    /// The Distance from player's positions needed to spawn enemies 
    /// </summary>
    public float spawning_distance;

    private Transform[] spawn_locations;

    public void Start()
    {
        StartCoroutine(enemy_spawner());
    }

    private IEnumerator enemy_spawner()
    {
        yield return new WaitForSeconds(Random.Range(min_spawn_time, max_spawn_time));

        Transform spawn_location;
        do
        {
            spawn_location = find_spawn_location();
        }
        while (Vector2.Distance(spawn_location.position, player_center.position) > spawning_distance);

        for (int i = 0; i < Random.Range(min_enemy_size, max_enemy_size); i++)
        {
            float rand_offset = Random.Range(enemy_spawn_offset, enemy_spawn_offset);
            Vector3 spawn_offset = new Vector3(rand_offset, rand_offset);
            Instantiate(enemy_prefab, spawn_location.position + spawn_offset, Quaternion.identity);
        }

        StartCoroutine(enemy_spawner());
    }
    
    private Transform find_spawn_location()
    {
        GameObject[] locations = GameObject.FindGameObjectsWithTag("EnemySpawnLocation");
        if (locations.Length <= 0) return null;

        return locations[Random.Range(0, locations.Length)].transform;
    }
}

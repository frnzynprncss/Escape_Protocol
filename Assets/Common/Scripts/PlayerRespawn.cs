using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// =========================
// RESPAWN RESET INTERFACE
// =========================
public interface IRespawnReset
{
    void OnRespawn();
}

public class PlayerRespawn : MonoBehaviour
{
    public GameObject gameOverPanel;

    [Header("Respawn Settings")]
    public float respawnTime = 15f;
    public float spawnRadius = 3f;

    [Header("Spawn Validation")]
    public int maxSpawnAttempts = 20;

    private Vector3 initialSpawnPoint;
    private HealthComponent myHealth;
    private Transform cachedTransform;

    private static RespawnRunner runner;

    private void Awake()
    {
        cachedTransform = transform;

        if (runner == null)
        {
            GameObject go = new GameObject("RespawnRunner");
            runner = go.AddComponent<RespawnRunner>();
            DontDestroyOnLoad(go);
        }
    }

    private void Start()
    {
        initialSpawnPoint = cachedTransform.position;
        myHealth = GetComponent<HealthComponent>();

        if (gameOverPanel == null)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas c in canvases)
            {
                Transform t = c.transform.Find("LosePanel");
                if (t != null)
                {
                    gameOverPanel = t.gameObject;
                    break;
                }
            }
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void StartRespawnProcess()
    {
        cachedTransform.gameObject.SetActive(false);

        if (AreAllPlayersDead())
        {
            GameOver();
        }
        else
        {
            runner.StartCoroutine(
                RespawnRoutine(
                    cachedTransform,
                    myHealth,
                    initialSpawnPoint
                )
            );
        }
    }

    private IEnumerator RespawnRoutine(
        Transform targetTransform,
        HealthComponent health,
        Vector3 fallbackPos
    )
    {
        yield return new WaitForSeconds(respawnTime);

        if (AreAllPlayersDead())
            yield break;

        Vector3 spawnPos = GetSafeSpawnPositionNearTeammate(
            targetTransform,
            fallbackPos
        );

        // REPOSITION & REACTIVATE
        targetTransform.position = spawnPos;
        targetTransform.gameObject.SetActive(true);

        // =========================
        // CRITICAL FIX:
        // RESET ALL COMBAT / INPUT LOGIC
        // =========================
        IRespawnReset[] resetters =
            targetTransform.GetComponentsInChildren<IRespawnReset>(true);

        foreach (var r in resetters)
            r.OnRespawn();

        health.Revive();
    }

    private bool AreAllPlayersDead()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            HealthComponent hp = p.GetComponent<HealthComponent>();
            if (hp != null && hp.health > 0)
                return false;
        }

        return true;
    }

    private void GameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // =========================
    // SAFE FLOOR-AWARE SPAWN
    // =========================
    private Vector3 GetSafeSpawnPositionNearTeammate(
        Transform self,
        Vector3 fallback
    )
    {
        if (RoomFirstDungeonGenerator.Instance == null)
            return fallback;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.transform == self) continue;

            HealthComponent hp = p.GetComponent<HealthComponent>();
            if (hp == null || hp.health <= 0 || !p.activeInHierarchy)
                continue;

            Vector3 basePos = p.transform.position;

            for (int i = 0; i < maxSpawnAttempts; i++)
            {
                Vector2 offset = Random.insideUnitCircle * spawnRadius;
                Vector3 candidate =
                    basePos + new Vector3(offset.x, offset.y, 0f);

                if (RoomFirstDungeonGenerator.Instance
                    .IsPositionOnFloor(candidate))
                    return candidate;
            }

            return basePos;
        }

        return fallback;
    }
}

// =========================
// PERSISTENT COROUTINE HOST
// =========================
public class RespawnRunner : MonoBehaviour { }

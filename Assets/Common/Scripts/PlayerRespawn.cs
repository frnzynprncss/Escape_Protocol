using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject gameOverPanel;

    [Header("Respawn Settings")]
    public float respawnTime = 15f;
    public float spawnRadius = 3f;

    // Internal Variables
    private Vector3 initialSpawnPoint;
    private HealthComponent myHealth;

    // Logic components
    private Collider myCollider;
    private Rigidbody myRb;
    private CharacterController myCharController;
    private Renderer[] myRenderers;

    // We store the scripts we disabled so we can turn them back on later
    private List<MonoBehaviour> disabledScripts = new List<MonoBehaviour>();

    private void Start()
    {
        initialSpawnPoint = transform.position;
        myHealth = GetComponent<HealthComponent>();

        // 1. Auto-Find Game Over Panel if missing
        if (gameOverPanel == null)
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            foreach (Canvas c in allCanvases)
            {
                Transform panelTrans = c.transform.Find("LosePanel");
                if (panelTrans != null)
                {
                    gameOverPanel = panelTrans.gameObject;
                    break;
                }
            }
        }
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // 2. Get Standard Components
        myCollider = GetComponent<Collider>();
        myRb = GetComponent<Rigidbody>();
        myCharController = GetComponent<CharacterController>();
        myRenderers = GetComponentsInChildren<Renderer>();
    }

    public void StartRespawnProcess()
    {
        TogglePlayer(false);

        if (AreAllPlayersDead())
        {
            GameOver();
        }
        else
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    private bool AreAllPlayersDead()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in allPlayers)
        {
            if (p != this.gameObject)
            {
                HealthComponent hp = p.GetComponent<HealthComponent>();
                if (hp != null && hp.health > 0) return false;
            }
        }
        return true;
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER!");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    private IEnumerator RespawnRoutine()
    {
        Debug.Log($"Player {myHealth.playerID} died. Waiting {respawnTime} seconds...");

        yield return new WaitForSeconds(respawnTime);

        if (AreAllPlayersDead()) yield break;

        Vector3 spawnPos = GetSpawnPositionNearTeammate();

        // Teleport Logic
        if (myCharController != null) myCharController.enabled = false;
        transform.position = spawnPos;

        // Re-enable everything
        TogglePlayer(true);
        myHealth.Revive();

        Debug.Log("Player Respawned!");
    }

    private void TogglePlayer(bool isActive)
    {
        if (myCollider) myCollider.enabled = isActive;
        if (myRb) myRb.isKinematic = !isActive;
        if (myCharController) myCharController.enabled = isActive;

        foreach (var rend in myRenderers) rend.enabled = isActive;

        if (isActive == false)
        {
            disabledScripts.Clear();
            MonoBehaviour[] allScripts = GetComponentsInChildren<MonoBehaviour>();

            foreach (var script in allScripts)
            {
                if (script == this) continue;
                if (script == myHealth) continue;
                if (script.GetComponent<Camera>() != null) continue;
                string scriptName = script.GetType().Name;
                if (scriptName.Contains("Camera") || scriptName.Contains("Follow")) continue;

                if (script.enabled)
                {
                    script.enabled = false;
                    disabledScripts.Add(script);
                }
            }
        }
        else
        {
            foreach (var script in disabledScripts)
            {
                if (script != null) script.enabled = true;
            }
        }
    }

    // --- UPDATED SPAWN LOGIC: Checks for walls ---
    private Vector3 GetSpawnPositionNearTeammate()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        GameObject target = null;

        // Find a living teammate
        foreach (GameObject p in allPlayers)
        {
            if (p != this.gameObject && p.activeInHierarchy)
            {
                HealthComponent mateHealth = p.GetComponent<HealthComponent>();
                if (mateHealth && mateHealth.health > 0)
                {
                    target = p;
                    break;
                }
            }
        }

        if (target != null)
        {
            // If the dungeon generator exists, we check if the floor is valid
            if (RoomFirstDungeonGenerator.Instance != null)
            {
                // Try 20 times to find a valid spot
                for (int i = 0; i < 20; i++)
                {
                    Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
                    Vector3 potentialPos = target.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

                    // Ask the generator: Is this a floor?
                    if (RoomFirstDungeonGenerator.Instance.IsPositionOnFloor(potentialPos))
                    {
                        return potentialPos;
                    }
                }
                // If 20 tries failed (too cramped), just spawn EXACTLY on the teammate
                return target.transform.position;
            }
            else
            {
                // Fallback for scenes without the generator
                Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
                return target.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            }
        }

        return initialSpawnPoint;
    }
}
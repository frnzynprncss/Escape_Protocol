using UnityEngine;
using System.Collections;
using System.Collections.Generic; // <--- THIS WAS MISSING
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour
{
    // Leave this empty in the inspector. The code will find it.
    public GameObject gameOverPanel;

    [Header("Respawn Settings")]
    public float respawnTime = 15f;
    public float spawnRadius = 3f;

    private Vector3 initialSpawnPoint;
    private HealthComponent myHealth;

    // Components
    private Collider myCollider;
    private Rigidbody myRb;
    private CharacterController myCharController;
    private Renderer[] myRenderers;

    private void Start()
    {
        initialSpawnPoint = transform.position;
        myHealth = GetComponent<HealthComponent>();

        // --- THE NUCLEAR OPTION: FIND BY TYPE ---
        // This ignores names and looks for ANY Canvas in the scene
        if (gameOverPanel == null)
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();

            foreach (Canvas c in allCanvases)
            {
                // Look for a child named "LosePanel" inside this canvas
                // (We use 'true' to search inside hidden children too)
                Transform panelTrans = c.transform.Find("LosePanel");

                if (panelTrans != null)
                {
                    gameOverPanel = panelTrans.gameObject;
                    Debug.Log("SUCCESS: Found LosePanel inside " + c.name);
                    break; // Stop looking, we found it
                }
            }
        }

        // Final check
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // Hide it immediately
        }
        else
        {
            Debug.LogError("STILL FAILING: 1. Is the panel named 'LosePanel' exactly? 2. Is it inside a Canvas?");
        }
        // ---------------------------------

        myCollider = GetComponent<Collider>();
        myRb = GetComponent<Rigidbody>();
        myCharController = GetComponent<CharacterController>();
        myRenderers = GetComponentsInChildren<Renderer>();
    }

    public void StartRespawnProcess()
    {
        SetPlayerActive(false);

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
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private IEnumerator RespawnRoutine()
    {
        Debug.Log($"Player {myHealth.playerID} died. Waiting...");

        yield return new WaitForSeconds(respawnTime);

        if (AreAllPlayersDead()) yield break;

        Vector3 spawnPos = GetSpawnPositionNearTeammate();

        if (myCharController != null) myCharController.enabled = false;
        transform.position = spawnPos;
        if (myCharController != null) myCharController.enabled = true;

        SetPlayerActive(true);
        myHealth.Revive();

        Debug.Log("Player Respawned!");
    }

    private Vector3 GetSpawnPositionNearTeammate()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> teammates = new List<GameObject>();

        foreach (GameObject p in allPlayers)
        {
            if (p != this.gameObject && p.activeInHierarchy)
            {
                HealthComponent mateHealth = p.GetComponent<HealthComponent>();
                if (mateHealth && mateHealth.health > 0)
                {
                    teammates.Add(p);
                }
            }
        }

        if (teammates.Count > 0)
        {
            GameObject target = teammates[Random.Range(0, teammates.Count)];
            Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
            return target.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }

        return initialSpawnPoint;
    }

    private void SetPlayerActive(bool isActive)
    {
        if (myCollider) myCollider.enabled = isActive;
        foreach (var rend in myRenderers) rend.enabled = isActive;
        if (myRb) myRb.isKinematic = !isActive;
    }
}
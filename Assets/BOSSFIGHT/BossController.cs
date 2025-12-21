using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    [Header("Visuals - Directional")]
    public Sprite moveRight_A;
    public Sprite moveRight_B;
    public Sprite moveLeft_A;
    public Sprite moveLeft_B;

    [Header("Visuals - Transitions")]
    public Sprite beforeMovingRight;
    public Sprite beforeMovingLeft;

    private Sprite spriteA;
    private Sprite spriteB;
    private SpriteRenderer sr;

    [Header("Spawning")]
    public GameObject[] alienPrefabs;
    public int swarmCount = 10;
    public float spawnRadius = 3f;

    [Header("Hover Motion")]
    public float boxSpeed = 0.8f;
    public float boxSize = 0.5f;
    private Vector2 startPosition;
    private float boxTimer;
    private float currentSide;

    [Header("Health")]
    public HealthComponent healthComponent;

    [Header("UI")]
    public GameObject winPanel;   // ASSIGN IN INSPECTOR

    private List<GameObject> activeAliens = new List<GameObject>();
    private Coroutine damageEffectRoutine;
    private Collider2D bossCollider;
    private bool bossDead = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startPosition = transform.position;

        if (healthComponent == null)
            healthComponent = GetComponent<HealthComponent>();

        bossCollider = GetComponent<Collider2D>();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Kinematic;

        if (winPanel != null)
            winPanel.SetActive(false);

        // ✅ CORRECT DEATH HOOK (NO POLLING)
        healthComponent.on_death.AddListener(OnBossDeath);

        StartCoroutine(BossAnimationRoutine());
        SpawnSwarm();
        UpdateDamageState();
    }

    void Update()
    {
        if (bossDead)
            return;

        boxTimer += Time.deltaTime * boxSpeed;
        currentSide = boxTimer % 4f;

        float offsetX = 0;
        float offsetY = 0;

        if (currentSide < 1f) { offsetX = Mathf.Lerp(-boxSize, boxSize, currentSide); offsetY = -boxSize; }
        else if (currentSide < 2f) { offsetX = boxSize; offsetY = Mathf.Lerp(-boxSize, boxSize, currentSide - 1f); }
        else if (currentSide < 3f) { offsetX = Mathf.Lerp(boxSize, -boxSize, currentSide - 2f); offsetY = boxSize; }
        else { offsetX = -boxSize; offsetY = Mathf.Lerp(boxSize, -boxSize, currentSide - 3f); }

        transform.position = startPosition + new Vector2(offsetX, offsetY);

        // Remove dead minions
        activeAliens.RemoveAll(a => a == null);
        UpdateDamageState();
    }

    IEnumerator BossAnimationRoutine()
    {
        while (!bossDead)
        {
            if (currentSide > 3.7f || currentSide < 0.3f)
                sr.sprite = beforeMovingRight;
            else if (currentSide > 1.7f && currentSide < 2.3f)
                sr.sprite = beforeMovingLeft;
            else
            {
                if (currentSide < 2f) { spriteA = moveRight_A; spriteB = moveRight_B; }
                else { spriteA = moveLeft_A; spriteB = moveLeft_B; }

                sr.sprite = (sr.sprite == spriteA) ? spriteB : spriteA;
            }

            yield return new WaitForSeconds(0.15f);
        }
    }

    void SpawnSwarm()
    {
        activeAliens.Clear();

        for (int i = 0; i < swarmCount; i++)
        {
            Vector2 spawnPos =
                (Vector2)transform.position +
                Random.insideUnitCircle.normalized * spawnRadius;

            GameObject prefab = alienPrefabs[Random.Range(0, alienPrefabs.Length)];
            GameObject alien = Instantiate(prefab, spawnPos, Quaternion.identity);

            AlienAI ai = alien.GetComponent<AlienAI>();
            if (ai != null)
                ai.targetTag = "Player";

            activeAliens.Add(alien);
        }
    }

    void UpdateDamageState()
    {
        bool invulnerable = activeAliens.Count > 0;

        if (bossCollider != null)
            bossCollider.enabled = !invulnerable;
    }

    public void ApplyBossDamage(AttackComponent attack)
    {
        if (bossDead || activeAliens.Count > 0)
            return;

        if (healthComponent == null)
            return;

        healthComponent.take_damage(attack);

        if (damageEffectRoutine != null)
            StopCoroutine(damageEffectRoutine);

        damageEffectRoutine = StartCoroutine(FlashRed());
    }

    // ✅ CALLED BY HealthComponent.on_death
    void OnBossDeath()
    {
        if (bossDead)
            return;

        bossDead = true;

        if (bossCollider != null)
            bossCollider.enabled = false;

        StopAllCoroutines();

        if (winPanel != null)
            winPanel.SetActive(true);
    }

    IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }
}

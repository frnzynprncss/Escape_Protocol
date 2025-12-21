using UnityEngine;

public class TentacleStable : MonoBehaviour
{
    public Transform target;
    public Transform[] segments;
    public float segmentLength = 0.5f;

    [Header("Timing")]
    public float waitTime = 3f;      // How long it waves above
    public float strikeDuration = 0.5f; // How long it stays reaching out
    private float timer;
    private float strikeTimer;
    private bool isStriking = false;

    [Header("Waving Settings")]
    public float waveSpeed = 2f;
    public float waveHeight = 2f;
    public Vector2 idleOffset = new Vector2(0, 5f);

    [Header("Visuals")]
    public LineRenderer lineRenderer; // NEW: Drag your LineRenderer component here

    void Start()
    {
        timer = waitTime;

        // Initialize line renderer if you assigned it
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = segments.Length;
        }
    }

    void Update()
    {
        if (target == null || segments.Length == 0) return;

        Vector3 currentGoal;

        if (!isStriking)
        {
            // STATE 1: WAVING IDLE
            timer -= Time.deltaTime;

            float xWave = Mathf.Sin(Time.time * waveSpeed) * waveHeight;
            currentGoal = transform.position + (Vector3)idleOffset + new Vector3(xWave, 0, 0);

            if (timer <= 0)
            {
                isStriking = true;
                strikeTimer = strikeDuration;
            }
        }
        else
        {
            // STATE 2: STRIKE AT PLAYER
            currentGoal = target.position;
            strikeTimer -= Time.deltaTime;

            if (strikeTimer <= 0)
            {
                ResetTentacle(); // Go back to waving
            }
        }

        // --- IK MATH ---
        // STEP 1: Drag the TIP to the goal
        segments[segments.Length - 1].position = currentGoal;

        // STEP 2: Pull backward
        for (int i = segments.Length - 2; i >= 0; i--)
        {
            Vector2 dir = (segments[i].position - segments[i + 1].position).normalized;
            segments[i].position = (Vector2)segments[i + 1].position + dir * segmentLength;
        }

        // STEP 3: Anchor to Boss
        segments[0].position = transform.position;

        // STEP 4: Pull forward and Rotate
        for (int i = 1; i < segments.Length; i++)
        {
            Vector2 dir = (segments[i].position - segments[i - 1].position).normalized;
            segments[i].position = (Vector2)segments[i - 1].position + dir * segmentLength;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            segments[i].rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // --- NEW VISUAL CONNECTION ---
        // This makes the segments appear as one connected line
        if (lineRenderer != null)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                lineRenderer.SetPosition(i, segments[i].position);
            }
        }
    }

    public void ResetTentacle()
    {
        timer = waitTime;
        isStriking = false;
    }
}
using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("Player 1 UI Prompts")]
    public GameObject p1_move_prompt; // e.g., Image showing WASD
    public GameObject p1_shoot_prompt; // e.g., Image showing F key

    [Header("Player 2 UI Prompts")]
    public GameObject p2_move_prompt; // e.g., Image showing Arrow Keys
    public GameObject p2_shoot_prompt; // e.g., Image showing Keypad 0

    // State tracking
    private bool p1_has_moved = false;
    private bool p2_has_moved = false;
    private bool movement_phase_complete = false;

    private bool p1_has_shot = false;
    private bool p2_has_shot = false;

    private void Start()
    {
        // Setup: Show Movement, Hide Shooting
        SetCanvasGroupState(p1_move_prompt, true);
        SetCanvasGroupState(p2_move_prompt, true);

        SetCanvasGroupState(p1_shoot_prompt, false);
        SetCanvasGroupState(p2_shoot_prompt, false);
    }

    private void Update()
    {
        if (!movement_phase_complete)
        {
            HandleMovementPhase();
        }
        else
        {
            HandleShootingPhase();
        }
    }

    private void HandleMovementPhase()
    {
        // -- PLAYER 1 CHECK --
        if (!p1_has_moved)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                p1_has_moved = true;
                // Animate or hide the prompt
                StartCoroutine(FadeOut(p1_move_prompt));
            }
        }

        // -- PLAYER 2 CHECK --
        if (!p2_has_moved)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                p2_has_moved = true;
                StartCoroutine(FadeOut(p2_move_prompt));
            }
        }

        // -- CHECK IF BOTH READY --
        if (p1_has_moved && p2_has_moved)
        {
            movement_phase_complete = true;
            // Reveal shooting prompts after a short delay
            StartCoroutine(StartShootingPhase());
        }
    }

    private void HandleShootingPhase()
    {
        // -- PLAYER 1 SHOOT --
        if (!p1_has_shot)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                p1_has_shot = true;
                StartCoroutine(FadeOut(p1_shoot_prompt));
            }
        }

        // -- PLAYER 2 SHOOT --
        if (!p2_has_shot)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                p2_has_shot = true;
                StartCoroutine(FadeOut(p2_shoot_prompt));
            }
        }

        // -- TUTORIAL COMPLETE --
        if (p1_has_shot && p2_has_shot)
        {
            // Optional: Call an event here to spawn the first enemy or open a door!
            this.enabled = false; // Turn off this script, we are done.
        }
    }

    IEnumerator StartShootingPhase()
    {
        yield return new WaitForSeconds(0.5f);
        SetCanvasGroupState(p1_shoot_prompt, true);
        SetCanvasGroupState(p2_shoot_prompt, true);
    }

    // Helper to fade UI out nicely
    IEnumerator FadeOut(GameObject uiElement)
    {
        CanvasGroup group = uiElement.GetComponent<CanvasGroup>();
        if (group != null)
        {
            float duration = 0.5f;
            float time = 0;
            while (time < duration)
            {
                group.alpha = Mathf.Lerp(1, 0, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            group.alpha = 0;
        }
        uiElement.SetActive(false);
    }

    void SetCanvasGroupState(GameObject obj, bool visible)
    {
        if (obj == null) return;
        obj.SetActive(visible);
        CanvasGroup group = obj.GetComponent<CanvasGroup>();
        if (group == null) group = obj.AddComponent<CanvasGroup>(); // Add if missing
        group.alpha = visible ? 1 : 0;
    }
}
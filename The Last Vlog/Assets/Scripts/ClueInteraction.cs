using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClueInteraction2D : MonoBehaviour
{
    // --- Public Variables (Set in Inspector) ---

    [Header("UI Elements")]
    [Tooltip("The TextMeshPro element to display the 'Press E' prompt.")]
    public TextMeshProUGUI promptText; 
    [Tooltip("The Image element that will display the full-screen clue.")]
    public Image clueImageDisplay;
    [Tooltip("The Sprite of the clue paper to show when E is pressed.")]
    public Sprite clueSprite;

    [Header("Clue Behavior")]
    [Tooltip("The time scale when the clue is active (set to 0 to stop player movement).")]
    public float timeScaleWhenActive = 0f;
    [Tooltip("The message to display in the prompt text.")]
    public string promptMessage = "Press E to Interact";

    // --- Private Variables ---

    private bool playerIsInTrigger = false;
    private bool isClueActive = false;
    private float originalTimeScale; 

    // --- Unity Lifecycle Methods ---

    void Start()
    {
        // Ensure UI elements are hidden at the start of the game
        if (promptText != null)
        {
            promptText.text = "";
        }

        if (clueImageDisplay != null)
        {
            clueImageDisplay.gameObject.SetActive(false);
            clueImageDisplay.sprite = clueSprite; 
        }

        originalTimeScale = Time.timeScale;
    }

    void Update()
    {
        // Handle the interaction when the player is near and the clue isn't active
        if (playerIsInTrigger && !isClueActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                promptText.text = "";
                DisplayClue();
                isClueActive = true;
            }
        }
        // Handle dismissing the clue when it is active
        else if (isClueActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DismissClue();
            }
        }
    }

    // --- 2D Trigger Methods (Collision Detection) ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInTrigger = true;
            // Show the prompt
            if (promptText != null && !isClueActive)
            {
                promptText.text = promptMessage;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInTrigger = false;
            // Hide the prompt
            if (promptText != null)
            {
                promptText.text = "";
            }
        }
    }

    // --- Helper Methods ---

    void DisplayClue()
    {
        if (clueImageDisplay != null)
        {
            clueImageDisplay.sprite = clueSprite;
            clueImageDisplay.gameObject.SetActive(true);
        }

        Time.timeScale = timeScaleWhenActive;
    }

    void DismissClue()
    {
        if (clueImageDisplay != null)
        {
            clueImageDisplay.gameObject.SetActive(false);
        }

        Time.timeScale = originalTimeScale;
        isClueActive = false;
        
        // Destroy the clue paper object
        Destroy(gameObject);
    }
}
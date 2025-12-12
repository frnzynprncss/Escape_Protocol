using UnityEngine;
using UnityEngine.UI; // Needed for Image component for color tinting
using UnityEngine.EventSystems;
using TMPro; // Needed if your button text is TextMeshPro

// New Interfaces: IPointerDownHandler & IPointerUpHandler for click effects
public class SmoothButtonAnimation : MonoBehaviour,
                                     IPointerEnterHandler,
                                     IPointerExitHandler,
                                     IPointerDownHandler,
                                     IPointerUpHandler
{
    // --- VARIABLES ---

    [Header("Scaling & Speed")]
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public float scaleSpeed = 10f; // Controls how quickly it moves to the target scale

    [Header("Color Tinting")]
    public Color hoverColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Slightly lighter gray
    public Color normalColor = Color.white; // Starting color
    public float colorSpeed = 5f; // Controls how quickly the color changes

    [Header("Click Animation")]
    public Vector3 pressSquish = new Vector3(0.9f, 0.9f, 0.9f); // Scale when pressed down

    [Header("Audio Feedback")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private Vector3 targetScale;
    private Color targetColor;
    private Image buttonImage;
    private AudioSource audioSource; // Optional: If you want sound effects

    // --- INITIALIZATION ---
    void Awake()
    {
        // Get the Image component for color changes
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            normalColor = buttonImage.color;
            targetColor = normalColor;
        }

        // Try to find an AudioSource on the button or its parent
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        targetScale = transform.localScale;
    }

    // --- GAME LOOP (For smooth transitions) ---
    void Update()
    {
        // 1. Smoothly Transition Scale (Runs on unscaled time)
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);

        // 2. Smoothly Transition Color (Runs on unscaled time)
        if (buttonImage != null)
        {
            buttonImage.color = Color.Lerp(buttonImage.color, targetColor, Time.unscaledDeltaTime * colorSpeed);
        }
    }

    // --- POINTER EVENT HANDLERS ---

    // Mouse enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;
        targetColor = hoverColor;

        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    // Mouse leaves the button area
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = Vector3.one; // Back to normal scale
        targetColor = normalColor; // Back to normal color
    }

    // Mouse button is pressed DOWN on the button
    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = pressSquish; // Squish the button
        targetColor = hoverColor * 0.8f; // Darken the color slightly
    }

    // Mouse button is released UP on the button
    public void OnPointerUp(PointerEventData eventData)
    {
        // If the pointer is still over the button, go back to the hover state
        if (eventData.hovered.Contains(gameObject))
        {
            targetScale = hoverScale;
            targetColor = hoverColor;
        }
        else
        {
            // Otherwise, go back to the normal state
            targetScale = Vector3.one;
            targetColor = normalColor;
        }

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
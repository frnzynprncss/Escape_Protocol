using UnityEngine;
using UnityEngine.UI; // Required for UI Images

public class BlinkingLightEffect : MonoBehaviour
{
    [Header("Blink Settings")]
    [Tooltip("How fast the light blinks.")]
    [Range(0.1f, 10.0f)]
    public float blinkSpeed = 2.0f;

    [Tooltip("The dimmest the light will get (0 is invisible, 1 is full).")]
    [Range(0.0f, 1.0f)]
    public float minAlpha = 0.2f;

    [Tooltip("The brightest the light will get.")]
    [Range(0.0f, 1.0f)]
    public float maxAlpha = 1.0f;

    private Image targetImage;
    private SpriteRenderer targetSprite;
    private bool isUI = true;

    void Start()
    {
        // Try to get the UI Image component first
        targetImage = GetComponent<Image>();

        // If no UI Image is found, look for a SpriteRenderer (in case it's a 2D world object)
        if (targetImage == null)
        {
            targetSprite = GetComponent<SpriteRenderer>();
            isUI = false;
        }
    }

    void Update()
    {
        // Calculate a smooth "PingPong" value between 0 and 1 using a Sine wave
        // (Sin goes from -1 to 1, so we normalize it to 0 to 1)
        float wave = (Mathf.Sin(Time.time * blinkSpeed) + 1.0f) / 2.0f;

        // Lerp (Linear Interpolate) calculates the current alpha based on the wave
        float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, wave);

        if (isUI && targetImage != null)
        {
            // Update UI Image Color
            Color c = targetImage.color;
            c.a = currentAlpha;
            targetImage.color = c;
        }
        else if (!isUI && targetSprite != null)
        {
            // Update Sprite Renderer Color
            Color c = targetSprite.color;
            c.a = currentAlpha;
            targetSprite.color = c;
        }
    }
}
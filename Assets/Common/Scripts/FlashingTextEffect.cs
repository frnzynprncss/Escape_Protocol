using UnityEngine;
using TMPro; // Important: Need this for TextMeshPro

public class FlashingTextEffect : MonoBehaviour
{
    private TextMeshProUGUI textComponent;

    // --- EFFECT PARAMETERS ---

    // 1. Alpha Pulse Settings
    [Header("Alpha Pulse")]
    public float flashSpeed = 1.5f;
    public float minAlpha = 0.3f; // Lowest transparency
    public float maxAlpha = 1.0f; // Highest transparency

    // 2. Color Shift Settings
    [Header("Color Shift")]
    public Color baseColor = Color.red;       // The main color (set this in the Inspector)
    public Color accentColor = Color.yellow;  // The color it shifts towards
    public float colorShiftSpeed = 0.5f;      // Slower shift speed

    // 3. Scale Pulse Settings
    [Header("Scale Pulse")]
    public float scaleSpeed = 2f;             // Slightly faster pulse speed
    public float minScale = 1.0f;             // Normal size
    public float maxScale = 1.05f;            // 5% larger at its peak

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        if (textComponent == null)
        {
            Debug.LogError("FlashingTextEffect requires a TextMeshProUGUI component.");
            enabled = false;
        }
        else
        {
            // Set the initial color based on the Inspector setting
            textComponent.color = baseColor;
            // Ensure the text has its default scale
            transform.localScale = Vector3.one;
        }
    }

    void Update()
    {
        // Use Time.unscaledTime so effects run when Time.timeScale is 0.
        float unscaledTime = Time.unscaledTime;

        // --- ALPHA PULSE (Breathing Transparency) ---
        // 'alphaWave' smoothly oscillates between 0 and 1
        float alphaWave = Mathf.PingPong(unscaledTime * flashSpeed, 1f);
        Color currentColor = textComponent.color;

        // Interpolate the Alpha value
        currentColor.a = Mathf.Lerp(minAlpha, maxAlpha, alphaWave);


        // --- COLOR SHIFT (Smooth Transition) ---
        // 'colorWave' smoothly oscillates between 0 and 1 over a longer period
        float colorWave = Mathf.PingPong(unscaledTime * colorShiftSpeed, 1f);

        // Interpolate the RGB values between the base and accent color
        Color shiftedColor = Color.Lerp(baseColor, accentColor, colorWave);

        // Keep the calculated alpha, but update the RGB colors
        currentColor.r = shiftedColor.r;
        currentColor.g = shiftedColor.g;
        currentColor.b = shiftedColor.b;

        // Apply the final color (RGB + Alpha)
        textComponent.color = currentColor;


        // --- SCALE PULSE (Breathing Size) ---
        // 'scaleWave' smoothly oscillates between 0 and 1
        float scaleWave = Mathf.PingPong(unscaledTime * scaleSpeed, 1f);

        // Interpolate the scale value
        float currentScale = Mathf.Lerp(minScale, maxScale, scaleWave);

        // Apply the scale to the transform
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);
    }
}
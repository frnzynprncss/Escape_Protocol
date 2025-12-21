using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SmoothButtonAnimation : MonoBehaviour,
                                     IPointerEnterHandler,
                                     IPointerExitHandler,
                                     IPointerDownHandler,
                                     IPointerUpHandler
{
    [Header("Scaling & Speed")]
    public float hoverScaleMultiplier = 1.1f;
    public float scaleSpeed = 10f;

    [Header("Color Tinting")]
    public Color hoverColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color normalColor = Color.white;
    public float colorSpeed = 5f;

    [Header("Click Animation")]
    public float pressSquishMultiplier = 0.9f;

    [Header("Audio Feedback")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private Vector3 initialScale;
    private Vector3 targetScale;
    private Vector3 hoverScaleVector;
    private Vector3 pressSquishVector;
    private Color targetColor;
    private Image buttonImage;
    private AudioSource audioSource;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            normalColor = buttonImage.color;
            targetColor = normalColor;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        initialScale = transform.localScale;

        hoverScaleVector = initialScale * hoverScaleMultiplier;
        pressSquishVector = initialScale * pressSquishMultiplier;

        targetScale = initialScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);

        if (buttonImage != null)
        {
            buttonImage.color = Color.Lerp(buttonImage.color, targetColor, Time.unscaledDeltaTime * colorSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScaleVector;
        targetColor = hoverColor;

        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = initialScale;
        targetColor = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = pressSquishVector;
        targetColor = hoverColor * 0.8f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.hovered.Contains(gameObject))
        {
            targetScale = hoverScaleVector;
            targetColor = hoverColor;
        }
        else
        {
            targetScale = initialScale;
            targetColor = normalColor;
        }

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
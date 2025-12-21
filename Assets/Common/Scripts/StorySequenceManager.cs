using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class StorySequenceManager : MonoBehaviour
{
    [Header("1. Video Settings")]
    public VideoPlayer introVideo;
    public GameObject videoDisplayCanvas;
    public float videoFadeDuration = 1.0f;

    [Header("2. Text & Moving Content")]
    public RectTransform scrollingText;      // Drag your "ScrollingContent" container here
    public float scrollSpeed = 50f;
    public float stopPositionOffset = 200f;
    public CanvasGroup textCanvasGroup;

    [Header("3. Static Content (New)")]
    public CanvasGroup staticContentCanvasGroup; // Drag your Static Images Group here!

    [Header("4. Background & Final Transition")]
    public CanvasGroup blackBackgroundPanel;
    public float panelFadeDuration = 2.0f;
    public float imageDisplayDuration = 3.0f;
    public string mainMenuScene = "MainMenu";

    // Internal State
    private bool isVideoPlaying = true;
    private bool isScrolling = false;
    private bool isFinishing = false;
    private float calculatedStopPosition;

    void Start()
    {
        // 1. Setup Video
        if (introVideo != null)
        {
            introVideo.Play();
            introVideo.loopPointReached += OnVideoFinished;
        }
        else
        {
            StartTextScroll();
        }

        // 2. Setup Text Position (Forcing layout rebuild for accuracy)
        if (scrollingText != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollingText);
            calculatedStopPosition = scrollingText.rect.height + Screen.height + stopPositionOffset;
            scrollingText.gameObject.SetActive(introVideo == null);
        }

        // 3. Ensure Backgrounds/Static items are Visible initially
        if (blackBackgroundPanel != null) blackBackgroundPanel.alpha = 1f;
        if (staticContentCanvasGroup != null) staticContentCanvasGroup.alpha = 1f;
    }

    void Update()
    {
        // SKIP BUTTON
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isVideoPlaying) SkipVideo();
            else if (isScrolling) StartCoroutine(FinishSequence());
        }

        // SCROLLING LOGIC
        if (isScrolling && scrollingText != null && !isFinishing)
        {
            scrollingText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            if (scrollingText.anchoredPosition.y > calculatedStopPosition)
            {
                StartCoroutine(FinishSequence());
            }
        }
    }

    void OnVideoFinished(VideoPlayer vp) { SkipVideo(); }

    void SkipVideo()
    {
        if (!isVideoPlaying) return;
        isVideoPlaying = false;

        if (videoDisplayCanvas != null) videoDisplayCanvas.SetActive(false);
        else if (introVideo != null) introVideo.gameObject.SetActive(false);

        StartTextScroll();
    }

    void StartTextScroll()
    {
        if (scrollingText != null)
        {
            scrollingText.gameObject.SetActive(true);
            isScrolling = true;
        }
        else
        {
            StartCoroutine(FinishSequence());
        }
    }

    IEnumerator FinishSequence()
    {
        if (isFinishing) yield break;
        isFinishing = true;
        isScrolling = false;

        float startAlphaPanel = (blackBackgroundPanel != null) ? blackBackgroundPanel.alpha : 1f;
        float startAlphaStatic = (staticContentCanvasGroup != null) ? staticContentCanvasGroup.alpha : 1f;
        float time = 0;

        // FADE LOOP
        while (time < panelFadeDuration)
        {
            time += Time.deltaTime;
            float progress = time / panelFadeDuration;

            // Fade the Black Panel
            if (blackBackgroundPanel != null)
                blackBackgroundPanel.alpha = Mathf.Lerp(startAlphaPanel, 0, progress);

            // Fade the Static Images (At the same time!)
            if (staticContentCanvasGroup != null)
                staticContentCanvasGroup.alpha = Mathf.Lerp(startAlphaStatic, 0, progress);

            yield return null;
        }

        // Ensure they are fully invisible
        if (blackBackgroundPanel != null) blackBackgroundPanel.alpha = 0;
        if (staticContentCanvasGroup != null) staticContentCanvasGroup.alpha = 0;

        // Wait to show revealed images
        yield return new WaitForSeconds(imageDisplayDuration);

        // Load Menu
        SceneManager.LoadScene(mainMenuScene);
    }
}
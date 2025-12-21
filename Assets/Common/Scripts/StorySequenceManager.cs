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
    public RectTransform scrollingText;
    public float scrollSpeed = 50f;
    public float stopPositionOffset = 200f;
    public CanvasGroup textCanvasGroup;

    [Header("3. Static Content (New)")]
    public CanvasGroup staticContentCanvasGroup;

    [Header("4. Background & Final Transition")]
    public CanvasGroup blackBackgroundPanel;
    public float panelFadeDuration = 2.0f;
    public float imageDisplayDuration = 3.0f;
    public string mainMenuScene = "MainMenu";

    private bool isVideoPlaying = true;
    private bool isScrolling = false;
    private bool isFinishing = false;
    private float calculatedStopPosition;

    void Start()
    {
        if (introVideo != null)
        {
            introVideo.playOnAwake = false;
            introVideo.prepareCompleted += OnVideoPrepared;
            introVideo.loopPointReached += OnVideoFinished;
            introVideo.Prepare();
        }
        else
        {
            isVideoPlaying = false;
            StartTextScroll();
        }

        if (scrollingText != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollingText);
            calculatedStopPosition = scrollingText.rect.height + Screen.height + stopPositionOffset;
            scrollingText.gameObject.SetActive(introVideo == null);
        }

        if (blackBackgroundPanel != null) blackBackgroundPanel.alpha = 1f;
        if (staticContentCanvasGroup != null) staticContentCanvasGroup.alpha = 1f;
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnVideoPrepared;
        vp.Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isVideoPlaying) SkipVideo();
            else if (isScrolling) StartCoroutine(FinishSequence());
        }

        if (isScrolling && scrollingText != null && !isFinishing)
        {
            scrollingText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            if (scrollingText.anchoredPosition.y > calculatedStopPosition)
            {
                StartCoroutine(FinishSequence());
            }
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SkipVideo();
    }

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

        while (time < panelFadeDuration)
        {
            time += Time.deltaTime;
            float progress = time / panelFadeDuration;

            if (blackBackgroundPanel != null)
                blackBackgroundPanel.alpha = Mathf.Lerp(startAlphaPanel, 0, progress);

            if (staticContentCanvasGroup != null)
                staticContentCanvasGroup.alpha = Mathf.Lerp(startAlphaStatic, 0, progress);

            yield return null;
        }

        if (blackBackgroundPanel != null) blackBackgroundPanel.alpha = 0;
        if (staticContentCanvasGroup != null) staticContentCanvasGroup.alpha = 0;

        yield return new WaitForSeconds(imageDisplayDuration);

        SceneManager.LoadScene(mainMenuScene);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Inventory & Player")]
    public PlayerInventory playerInventory;

    [Header("UI References")]
    public TextMeshProUGUI returnIndicatorText;
    public TextMeshProUGUI timerText;

    [Header("Panels")]
    public GameObject LosePanel;

    [Header("Timer Settings")]
    public float timeRemaining = 300f;

    [Header("Scene Transition")]
    public string bossFightSceneName = "BossFightScene"; // EXACT scene name

    private bool timerIsRunning = false;
    private bool gameEnded = false;

    private void Start()
    {
        if (LosePanel != null) LosePanel.SetActive(false);
        if (returnIndicatorText != null) returnIndicatorText.gameObject.SetActive(false);

        timerIsRunning = true;
    }

    private void Update()
    {
        if (timerIsRunning && !gameEnded)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                LoseGame();
            }
        }
    }

    void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timerText == null) return;

        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        timerIsRunning = false;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(bossFightSceneName);
    }

    public void LoseGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        timerIsRunning = false;

        if (LosePanel != null) LosePanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

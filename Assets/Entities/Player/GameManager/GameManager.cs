using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Inventory & Player")]
    // Ensure this is assigned to the PlayerInventoryHolder component!
    public PlayerInventory playerInventory;

    [Header("UI References")]
    public TextMeshProUGUI returnIndicatorText;
    public TextMeshProUGUI timerText; // Drag your Timer Text (TMP) here
    public GameObject WinPanel;
    public GameObject LosePanel;      // Drag your Game Over Panel here

    [Header("Timer Settings")]
    public float timeRemaining = 300f; // 300 seconds = 5 minutes
    private bool timerIsRunning = false;
    private bool gameEnded = false;    // Replaces 'goalAchieved' for general game state

    private void Start()
    {
        // 1. Initialize UI Panels
        if (WinPanel != null) WinPanel.SetActive(false);
        if (LosePanel != null) LosePanel.SetActive(false);

        if (returnIndicatorText != null)
            returnIndicatorText.gameObject.SetActive(false);

        // 2. Start the Timer
        timerIsRunning = true;
    }

    private void Update()
    {
        // Only run the timer if the game hasn't ended
        if (timerIsRunning && !gameEnded)
        {
            if (timeRemaining > 0)
            {
                // Subtract time
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                // Time reached 0
                timeRemaining = 0;
                LoseGame();
            }
        }
    }

    // Formats the float time into 00:00 format
    void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timerText == null) return; // Safety check

        timeToDisplay += 1; // Visual fix so it doesn't show 00:00 for a whole second before stopping
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        timerIsRunning = false; // Stop the timer since we won

        if (WinPanel != null)
        {
            WinPanel.SetActive(true);
        }

        Debug.Log("Congratulations! You won the game!");

        // Optional: Pause the game
        Time.timeScale = 0f;
        UnlockCursor();
    }

    public void LoseGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        timerIsRunning = false;

        Debug.Log("Time is up! Game Over.");

        if (LosePanel != null)
        {
            LosePanel.SetActive(true);
        }

        // Optional: Pause the game
        Time.timeScale = 0f;
        UnlockCursor();
    }

    // Helper to make sure the mouse is visible when a panel opens
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
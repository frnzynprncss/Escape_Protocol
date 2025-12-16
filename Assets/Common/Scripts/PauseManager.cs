using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject pauseMenuPanel;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Cursor Settings")]
    // Check this box in the Inspector if your game is an FPS or uses a locked cursor
    public bool lockCursorDuringGameplay = false;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Initialize cursor state based on your settings
        if (lockCursorDuringGameplay)
        {
            SetCursorState(false); // Lock and hide cursor at start
        }
    }

    void Update()
    {
        // Toggle pause when Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Always show the cursor when paused so the player can click buttons
        SetCursorState(true);
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // If the game uses a locked cursor, re-lock it now
        if (lockCursorDuringGameplay)
        {
            SetCursorState(false);
        }
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // A helper function to handle cursor logic cleanly
    private void SetCursorState(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None; // Unlock the mouse
            Cursor.visible = true;                  // Make it visible
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock mouse to center
            Cursor.visible = false;                   // Hide it
        }
    }
}
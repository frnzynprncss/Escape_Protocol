using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

public class PauseManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject pauseMenuPanel; // Drag your Pause Panel here

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu"; // Type your Main Menu scene name here exactly

    private bool isPaused = false;

    void Start()
    {
        // Ensure the pause menu is hidden when the game starts
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
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
        pauseMenuPanel.SetActive(true); // Show UI
        Time.timeScale = 0f;            // FREEZE TIME
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); // Hide UI
        Time.timeScale = 1f;             // UNFREEZE TIME
        isPaused = false;
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f; // Always unfreeze before reloading!
        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Always unfreeze before leaving
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
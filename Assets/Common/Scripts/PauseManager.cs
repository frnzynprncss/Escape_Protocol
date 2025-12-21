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
    public string retrySceneName = "Level1";

    [Header("Cursor Settings")]
    public bool lockCursorDuringGameplay = false;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (lockCursorDuringGameplay)
        {
            SetCursorState(false);
        }
    }

    void Update()
    {
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
        SetCursorState(true);
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

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

    public void RetryToSpecificScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(retrySceneName);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SetCursorState(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
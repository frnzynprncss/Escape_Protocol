using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    // --- SINGLETON SETUP ---
    // A public static reference to this script ensures easy access from anywhere.
    public static PauseMenuManager Instance;

    // --- VARIABLES ---
    public GameObject pauseMenuUI;

    // Static boolean to track the game state globally (private set is safer)
    public static bool GameIsPaused { get; private set; } = false;

    // --- STARTUP ---

    void Awake()
    {
        // Enforce the Singleton pattern: only one instance is allowed
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional: If you want the manager to persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // --- GAME LOOP LOGIC ---

    void Update()
    {
        // Input check
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                // Note: Now calling the private function
                Pause();
            }
        }
    }

    // --- CORE PAUSE/RESUME LOGIC ---

    // Public function, called by the Resume button
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    // Private function, only called internally by the manager
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    // --- BUTTON FUNCTIONS ---

    public void Restart()
    {
        // Unpause the game before loading the new scene
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        // Application.Quit() is correctly placed here for builds
        Application.Quit();
    }
}
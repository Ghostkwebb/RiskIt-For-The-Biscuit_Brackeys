using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuPanel;

    private bool isPaused = false;

    void Start()
    {
        // Ensure the pause menu is hidden and the game is running at the start.
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        HandlePauseInput();
    }

    private void HandlePauseInput()
    {
        // Listen for the "Escape" key press.
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

    // --- Public methods for the UI buttons to call ---

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // Resumes the game's time.
        Debug.Log("Game Resumed.");
    }

    public void LoadMainMenu()
    {
        // We must un-pause the game before changing scenes, or the next scene might be frozen.
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Assumes your MainMenu is at build index 0
        Debug.Log("Loading Main Menu...");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game from pause menu...");
        Application.Quit();
    }

    // --- Private helper method ---

    private void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // This is the magic line that freezes the game.
        Debug.Log("Game Paused.");
    }
}
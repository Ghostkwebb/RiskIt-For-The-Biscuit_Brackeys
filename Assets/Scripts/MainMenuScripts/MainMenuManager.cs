using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Settings UI Elements")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown graphicsDropdown;

    [Header("Animators")]
    [SerializeField] private Animator fadePanelAnimator;
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private Animator menuButtonsAnimator;

    private Resolution[] resolutions;

    void Awake()
    {
        // Ensure master volume is at 100% when the game starts.
        AudioListener.volume = 1f;
    }

    void Start()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        StartIntroSequence();
        InitializeSettings();
    }

    private void StartIntroSequence()
    {
        if (fadePanelAnimator != null) fadePanelAnimator.SetTrigger("FadeIn");
        if (cameraAnimator != null) cameraAnimator.SetTrigger("ZoomIn");
        if (menuButtonsAnimator != null) menuButtonsAnimator.SetTrigger("ShowMenu");

        Invoke(nameof(ShowMainMenu), 2f);
    }

    private void ShowMainMenu()
    {
        mainPanel.SetActive(true);
    }

    private void InitializeSettings()
    {
        // --- Resolution Setup ---
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // --- Graphics Quality Setup ---
        graphicsDropdown.ClearOptions();
        graphicsDropdown.AddOptions(new List<string>(QualitySettings.names));
        graphicsDropdown.value = QualitySettings.GetQualityLevel();
        graphicsDropdown.RefreshShownValue();

        // --- Load Saved Settings ---
        LoadSettings();
    }

    // --- Main Button Functions ---
    public void StartGame()
    {
        SceneLoader.sceneToLoad = 2;
        SceneManager.LoadScene(1);
    }

    public void OpenTutorial()
    {
        Debug.Log("Tutorial button pressed!");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    // --- Settings Panel Functions ---
    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // --- Settings Control Functions ---
    // The SetVolume method has been removed.

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("GraphicsQuality", qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }

    private void LoadSettings()
    {
        // The section for loading volume has been removed.

        // Load Graphics
        int quality = PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel());
        graphicsDropdown.value = quality;
        QualitySettings.SetQualityLevel(quality);

        // Load Resolution
        int resolution = PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1);
        resolutionDropdown.value = resolution;
        SetResolution(resolution);
    }
}
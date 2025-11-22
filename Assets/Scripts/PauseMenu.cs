using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles pause menu and settings menu functionality
/// Press ESC or P to pause/unpause
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Settings UI - Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Audio Clips (Optional)")]
    [SerializeField] private AudioClip buttonClickSFX;

    private bool isPaused = false;
    private bool settingsOpen = false;

    private void Start()
    {
        // Pastikan panel tertutup pas awal game
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Setup slider values dari AudioManager
        SetupAudioSliders();
    }

    private void SetupAudioSliders()
    {
        if (AudioManager.Instance != null)
        {
            if (musicSlider != null)
            {
                musicSlider.value = AudioManager.Instance.GetMusicVolume();
                musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }
            
            if (sfxSlider != null)
            {
                sfxSlider.value = AudioManager.Instance.GetSFXVolume();
                sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }
        }
        else
        {
            // Kalo gak ada AudioManager, set default value
            if (musicSlider != null) musicSlider.value = 0.7f;
            if (sfxSlider != null) sfxSlider.value = 1f;
        }
    }

    private void Update()
    {
        // Tombol ESC atau P buat pause
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (settingsOpen)
            {
                CloseSettings();
            }
            else if (isPaused)
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
        isPaused = true;
        settingsOpen = false;
        
        if (pausePanel != null) pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        Time.timeScale = 0f; // Stop waktu
        
        PlayButtonSound();
        
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        Debug.Log("🎮 RESUME BUTTON CLICKED!"); // TEST
        
        isPaused = false;
        settingsOpen = false;
        
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        Time.timeScale = 1f; // Jalanin waktu lagi
        
        PlayButtonSound();
        
        Debug.Log("Game Resumed");
    }

    public void RestartGame()
    {
        Debug.Log("🔄 RESTART BUTTON CLICKED!"); // TEST
        
        Time.timeScale = 1f;
        PlayButtonSound();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PlayButtonSound();
        
        // Ganti "MainMenu" sesuai nama scene menu lo
        // SceneManager.LoadScene("MainMenu"); 
        
        // Kalo belom ada main menu scene, quit aja
        Debug.Log("Go to Main Menu / Quit Game");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void OpenSettings()
    {
        settingsOpen = true;
        
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        
        PlayButtonSound();
        
        Debug.Log("Settings Opened");
    }

    public void CloseSettings()
    {
        settingsOpen = false;
        
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
        
        PlayButtonSound();
        
        Debug.Log("Settings Closed");
    }

    private void OnMusicVolumeChanged(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }
    }

    private void OnSFXVolumeChanged(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(volume);
            
            // Play test sound biar user denger perubahannya
            PlayButtonSound();
        }
    }

    private void PlayButtonSound()
    {
        if (AudioManager.Instance != null && buttonClickSFX != null)
        {
            AudioManager.Instance.PlaySFX(buttonClickSFX);
        }
    }

    public bool IsPaused() => isPaused;
}


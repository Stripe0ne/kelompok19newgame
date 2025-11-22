using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main game manager that handles game state and UI
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Building building;
    [SerializeField] private WaveManager waveManager;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI enemyCountText;
    [SerializeField] private TextMeshProUGUI buildingHealthText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    
    [Header("Camera Settings")]
    [SerializeField] private Vector3 cameraPosition = new Vector3(0, 15, -10);
    [SerializeField] private Vector3 cameraRotation = new Vector3(50, 0, 0);
    
    private bool gameOver = false;

    private void Start()
    {
        SetupCamera();
        
        // Find references if not assigned
        if (building == null)
        {
            building = FindObjectOfType<Building>();
        }
        
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }
        
        // Hide game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        UpdateUI();
    }

    private void Update()
    {
        if (!gameOver)
        {
            UpdateUI();
        }
    }

    private void SetupCamera()
    {
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }
        
        if (gameCamera != null)
        {
            gameCamera.transform.position = cameraPosition;
            gameCamera.transform.eulerAngles = cameraRotation;
        }
    }

    private void UpdateUI()
    {
        // Update wave info
        if (waveText != null && waveManager != null)
        {
            waveText.text = $"Wave: {waveManager.GetCurrentWave()}";
        }
        
        // Update enemy count
        if (enemyCountText != null && waveManager != null)
        {
            enemyCountText.text = $"Enemies: {waveManager.GetActiveEnemyCount()}";
        }
        
        // Update building health
        if (buildingHealthText != null && building != null)
        {
            buildingHealthText.text = $"Building HP: {Mathf.CeilToInt(building.GetCurrentHealth())}/{Mathf.CeilToInt(building.GetMaxHealth())}";
        }
    }

    public void OnWaveStart(int waveNumber, int enemyCount)
    {
        Debug.Log($"GameManager: Wave {waveNumber} started with {enemyCount} enemies");
        UpdateUI();
    }

    public void OnWaveComplete(int waveNumber)
    {
        Debug.Log($"GameManager: Wave {waveNumber} completed!");
        UpdateUI();
    }

    public void OnBuildingDestroyed()
    {
        gameOver = true;
        Debug.Log("GameManager: Game Over!");
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (gameOverText != null && waveManager != null)
        {
            gameOverText.text = $"Game Over!\nYou survived {waveManager.GetCurrentWave()} waves!";
        }
        
        // Stop spawning
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
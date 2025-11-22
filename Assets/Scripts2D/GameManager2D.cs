using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main game manager for 2D top-down game (Vampire Survivors style)
/// </summary>
public class GameManager2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Tower2D tower;
    [SerializeField] private WaveManager2D waveManager;
    [SerializeField] private Player2D player;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI enemyCountText;
    [SerializeField] private TextMeshProUGUI towerHealthText;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    
    [Header("Camera Settings - 2D Orthographic")]
    [SerializeField] private float cameraSize = 8f; // Orthographic size for 2D view
    [SerializeField] private Vector3 cameraPosition = new Vector3(0, 0, -10);
    
    private bool gameOver = false;

    private void Start()
    {
        SetupCamera2D();
        
        // Find references if not assigned
        if (tower == null)
        {
            tower = FindObjectOfType<Tower2D>();
        }
        
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager2D>();
        }
        
        if (player == null)
        {
            player = FindObjectOfType<Player2D>();
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

    private void SetupCamera2D()
    {
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }
        
        if (gameCamera != null)
        {
            // Set to orthographic for 2D top-down view
            gameCamera.orthographic = true;
            gameCamera.orthographicSize = cameraSize;
            gameCamera.transform.position = cameraPosition;
            gameCamera.transform.rotation = Quaternion.identity;
            
            // Set background color (dark for Vampire Survivors feel)
            gameCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
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
        
        // Update tower health
        if (towerHealthText != null && tower != null)
        {
            towerHealthText.text = $"Tower HP: {Mathf.CeilToInt(tower.GetCurrentHealth())}/{Mathf.CeilToInt(tower.GetMaxHealth())}";
        }
        
        // Update player health
        if (playerHealthText != null && player != null)
        {
            playerHealthText.text = $"Player HP: {Mathf.CeilToInt(player.GetComponent<Player2D>().GetHealth())}/{Mathf.CeilToInt(player.GetComponent<Player2D>().GetMaxHealth())}";
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

    public void OnTowerDestroyed()
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
        
        // Pause game
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void OnPlayerDied()
    {
        gameOver = true;
        Debug.Log("GameManager: Player died! Game Over!");
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (gameOverText != null && waveManager != null)
        {
            gameOverText.text = $"Player Died!\nYou survived {waveManager.GetCurrentWave()} waves!";
        }
        
        // Pause game
        Time.timeScale = 0f;
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
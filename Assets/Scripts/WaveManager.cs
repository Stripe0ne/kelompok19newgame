using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages enemy waves with random spawn counts
/// </summary>
public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private int currentWave = 0;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float timeBetweenSpawns = 0.5f;
    
    [Header("Enemy Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnRadius = 10f;
    
    [Header("Wave Configuration")]
    [SerializeField] private int minEnemiesWave1 = 4;
    [SerializeField] private int maxEnemiesWave1 = 6;
    [SerializeField] private int enemiesIncreasePerWave = 2;
    
    [Header("Enemy Stat Scaling")]
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float healthScalePerWave = 20f;
    [SerializeField] private float speedScalePerWave = 0.2f;
    [SerializeField] private float damageScalePerWave = 2f;
    
    private List<Enemy> activeEnemies = new List<Enemy>();
    private bool waveInProgress = false;
    private int enemiesToSpawn = 0;
    private int enemiesSpawned = 0;

    private void Start()
    {
        // Auto-generate spawn points if none assigned
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            GenerateSpawnPoints();
        }
        
        // Start the first wave after a short delay
        StartCoroutine(StartNextWaveWithDelay(3f));
    }

    private void GenerateSpawnPoints()
    {
        // Create 8 spawn points in a circle around the building
        GameObject spawnPointContainer = new GameObject("SpawnPoints");
        List<Transform> points = new List<Transform>();
        
        for (int i = 0; i < 8; i++)
        {
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i}");
            spawnPoint.transform.parent = spawnPointContainer.transform;
            
            float angle = i * (360f / 8f) * Mathf.Deg2Rad;
            Vector3 position = new Vector3(
                Mathf.Cos(angle) * spawnRadius,
                0,
                Mathf.Sin(angle) * spawnRadius
            );
            spawnPoint.transform.position = position;
            points.Add(spawnPoint.transform);
        }
        
        spawnPoints = points.ToArray();
    }

    private IEnumerator StartNextWaveWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartWave();
    }

    private void StartWave()
    {
        currentWave++;
        waveInProgress = true;
        
        // Calculate random enemy count for this wave
        int minEnemies = minEnemiesWave1 + (currentWave - 1) * enemiesIncreasePerWave;
        int maxEnemies = maxEnemiesWave1 + (currentWave - 1) * enemiesIncreasePerWave;
        enemiesToSpawn = Random.Range(minEnemies, maxEnemies + 1);
        enemiesSpawned = 0;
        
        Debug.Log($"Wave {currentWave} started! Enemies to spawn: {enemiesToSpawn}");
        
        // Notify UI
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnWaveStart(currentWave, enemiesToSpawn);
        }
        
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        while (enemiesSpawned < enemiesToSpawn)
        {
            SpawnEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned!");
            return;
        }
        
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points available!");
            return;
        }
        
        // Choose a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        
        // Spawn enemy
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        
        if (enemy != null)
        {
            // Scale enemy stats based on wave number
            float health = baseHealth + (currentWave - 1) * healthScalePerWave;
            float speed = baseSpeed + (currentWave - 1) * speedScalePerWave;
            float damage = baseDamage + (currentWave - 1) * damageScalePerWave;
            
            enemy.SetStats(health, speed, damage);
            activeEnemies.Add(enemy);
        }
    }

    public void OnEnemyDied(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
        
        // Check if wave is complete
        if (waveInProgress && enemiesSpawned >= enemiesToSpawn && activeEnemies.Count == 0)
        {
            WaveComplete();
        }
    }

    private void WaveComplete()
    {
        waveInProgress = false;
        Debug.Log($"Wave {currentWave} complete!");
        
        // Notify game manager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnWaveComplete(currentWave);
        }
        
        // Start next wave after delay
        StartCoroutine(StartNextWaveWithDelay(timeBetweenWaves));
    }

    public int GetCurrentWave() => currentWave;
    public int GetActiveEnemyCount() => activeEnemies.Count;
    public bool IsWaveInProgress() => waveInProgress;
}
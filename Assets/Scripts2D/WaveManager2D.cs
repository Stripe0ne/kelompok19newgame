using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages enemy waves in 2D - Vampire Survivors style
/// </summary>
public class WaveManager2D : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private int currentWave = 0;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float timeBetweenSpawns = 0.5f;
    
    [Header("Enemy Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
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
    
    private List<Enemy2D> activeEnemies = new List<Enemy2D>();
    private bool waveInProgress = false;
    private int enemiesToSpawn = 0;
    private int enemiesSpawned = 0;
    private Transform towerTransform;

    private void Start()
    {
        // Find the tower
        GameObject tower = GameObject.FindGameObjectWithTag("Tower");
        if (tower != null)
        {
            towerTransform = tower.transform;
        }
        
        // Start the first wave after a short delay
        StartCoroutine(StartNextWaveWithDelay(3f));
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
        GameManager2D gameManager = FindObjectOfType<GameManager2D>();
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
        
        // Spawn at random position on circle edge (2D)
        Vector2 spawnPosition = GetRandomSpawnPosition();
        
        // Spawn enemy
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        Enemy2D enemy = enemyObj.GetComponent<Enemy2D>();
        
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

    private Vector2 GetRandomSpawnPosition()
    {
        // Spawn enemies in a circle around the tower
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        
        Vector2 centerPosition = towerTransform != null ? 
            (Vector2)towerTransform.position : Vector2.zero;
        
        Vector2 spawnOffset = new Vector2(
            Mathf.Cos(angle) * spawnRadius,
            Mathf.Sin(angle) * spawnRadius
        );
        
        return centerPosition + spawnOffset;
    }

    public void OnEnemyDied(Enemy2D enemy)
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
        GameManager2D gameManager = FindObjectOfType<GameManager2D>();
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

    // Visualize spawn radius in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = towerTransform != null ? towerTransform.position : Vector3.zero;
        
        // Draw circle in 2D (XY plane)
        int segments = 32;
        float angleStep = 360f / segments;
        
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;
            
            Vector3 point1 = center + new Vector3(
                Mathf.Cos(angle1) * spawnRadius,
                Mathf.Sin(angle1) * spawnRadius,
                0
            );
            
            Vector3 point2 = center + new Vector3(
                Mathf.Cos(angle2) * spawnRadius,
                Mathf.Sin(angle2) * spawnRadius,
                0
            );
            
            Gizmos.DrawLine(point1, point2);
        }
    }
}
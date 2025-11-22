using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Auto-attack weapon system - Vampire Survivors style
/// Automatically shoots nearest enemy
/// </summary>
public class Weapon2D : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float projectileSpeed = 10f;
    
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    
    private float lastAttackTime;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = transform;
        
        // Create fire point if not assigned
        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = Vector3.zero;
            firePoint = fp.transform;
        }
    }

    private void Update()
    {
        // Auto-attack nearest enemy
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            TryAttackNearestEnemy();
        }
    }

    private void TryAttackNearestEnemy()
    {
        // Find all enemies
        Enemy2D[] enemies = FindObjectsOfType<Enemy2D>();
        
        if (enemies.Length == 0) return;
        
        // Find nearest enemy within range
        Enemy2D nearestEnemy = null;
        float nearestDistance = attackRange;
        
        foreach (Enemy2D enemy in enemies)
        {
            if (!enemy.IsAlive()) continue;
            
            float distance = Vector2.Distance(playerTransform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        
        // Attack nearest enemy
        if (nearestEnemy != null)
        {
            Attack(nearestEnemy);
            lastAttackTime = Time.time;
        }
    }

    private void Attack(Enemy2D target)
    {
        if (projectilePrefab != null)
        {
            // Spawn projectile
            GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile2D projectile = projectileObj.GetComponent<Projectile2D>();
            
            if (projectile != null)
            {
                Vector2 direction = ((Vector2)target.transform.position - (Vector2)firePoint.position).normalized;
                projectile.Initialize(direction, projectileSpeed, damage, attackRange);
            }
        }
        else
        {
            // Instant damage if no projectile
            target.TakeDamage(damage);
        }
    }

    // Visualize attack range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // Public methods for upgrades
    public void UpgradeDamage(float amount) => damage += amount;
    public void UpgradeRange(float amount) => attackRange += amount;
    public void UpgradeSpeed(float amount) => attackCooldown = Mathf.Max(0.1f, attackCooldown - amount);
}
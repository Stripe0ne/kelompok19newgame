using UnityEngine;

/// <summary>
/// Base enemy class with health, attack, and movement stats
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("References")]
    [SerializeField] private HealthBar healthBar;
    
    private Transform target; // The building in the middle
    private float lastAttackTime;
    private bool isAlive = true;

    private void Start()
    {
        currentHealth = maxHealth;
        
        // Find the building target
        GameObject building = GameObject.FindGameObjectWithTag("Building");
        if (building != null)
        {
            target = building.transform;
        }
        
        // Setup health bar
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    private void Update()
    {
        if (!isAlive || target == null) return;

        // Move towards the building
        MoveTowardsTarget();
        
        // Check if in attack range
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= attackRange)
        {
            TryAttack();
        }
    }

    private void MoveTowardsTarget()
    {
        if (target == null) return;
        
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Make enemy face the building
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void Attack()
    {
        // Deal damage to the building
        Building buildingScript = target.GetComponent<Building>();
        if (buildingScript != null)
        {
            buildingScript.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;
        
        currentHealth -= damage;
        
        // Update health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isAlive = false;
        
        // Notify wave manager
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnEnemyDied(this);
        }
        
        // Destroy with a small delay for death animation if needed
        Destroy(gameObject, 0.1f);
    }

    // Public getters for stats
    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHealth() => currentHealth;
    public float GetMoveSpeed() => moveSpeed;
    public float GetAttackDamage() => attackDamage;
    public bool IsAlive() => isAlive;

    // Allow customization of stats for different enemy types
    public void SetStats(float health, float speed, float damage)
    {
        maxHealth = health;
        currentHealth = health;
        moveSpeed = speed;
        attackDamage = damage;
        
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }
}
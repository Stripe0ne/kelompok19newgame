using UnityEngine;

/// <summary>
/// 2D Enemy class - Vampire Survivors style top-down
/// </summary>
public class Enemy2D : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("References")]
    [SerializeField] private HealthBar2D healthBar;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private Transform target; // The tower/building in the middle
    private float lastAttackTime;
    private bool isAlive = true;
    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Setup Rigidbody2D for 2D top-down
        rb.gravityScale = 0; // No gravity in top-down 2D
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Don't rotate
        
        // Get sprite renderer
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        // Find the tower/building target
        GameObject tower = GameObject.FindGameObjectWithTag("Tower");
        if (tower != null)
        {
            target = tower.transform;
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

        // Move towards the tower
        MoveTowardsTarget();
        
        // Check if in attack range
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget <= attackRange)
        {
            TryAttack();
        }
    }

    private void MoveTowardsTarget()
    {
        if (target == null) return;
        
        // Calculate direction in 2D (only X and Y)
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        
        // Move using Rigidbody2D for smooth physics
        rb.linearVelocity = direction * moveSpeed;
        
        // Flip sprite based on movement direction (optional)
        if (spriteRenderer != null && direction.x != 0)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    private void TryAttack()
    {
        // Stop moving when attacking
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void Attack()
    {
        // Deal damage to the tower
        Tower2D towerScript = target.GetComponent<Tower2D>();
        if (towerScript != null)
        {
            towerScript.TakeDamage(attackDamage);
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
        
        // Flash red when hit (optional visual feedback)
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        isAlive = false;
        
        // Stop movement
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // Notify wave manager
        WaveManager2D waveManager = FindObjectOfType<WaveManager2D>();
        if (waveManager != null)
        {
            waveManager.OnEnemyDied(this);
        }
        
        // Destroy with small delay for death animation
        Destroy(gameObject, 0.1f);
    }

    // Public getters
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

    // Draw attack range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
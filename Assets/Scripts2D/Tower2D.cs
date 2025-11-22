using UnityEngine;

/// <summary>
/// The tower/building in the middle that enemies attack (2D version)
/// </summary>
public class Tower2D : MonoBehaviour
{
    [Header("Tower Stats")]
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private float currentHealth;
    
    [Header("References")]
    [SerializeField] private HealthBar2D healthBar;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private bool isDestroyed = false;

    private void Start()
    {
        currentHealth = maxHealth;
        
        // Get sprite renderer
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        // Setup health bar
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        
        // Make sure tower has the correct tag
        if (!gameObject.CompareTag("Tower"))
        {
            gameObject.tag = "Tower";
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;
        
        currentHealth -= damage;
        
        // Update health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        
        // Visual feedback - flash red
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }
        
        Debug.Log($"Tower took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            TowerDestroyed();
        }
    }

    private System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void TowerDestroyed()
    {
        isDestroyed = true;
        Debug.Log("Tower Destroyed! Game Over!");
        
        // Notify game manager
        GameManager2D gameManager = FindObjectOfType<GameManager2D>();
        if (gameManager != null)
        {
            gameManager.OnTowerDestroyed();
        }
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDestroyed() => isDestroyed;
}
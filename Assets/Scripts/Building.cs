using UnityEngine;

/// <summary>
/// The building in the middle that enemies attack
/// </summary>
public class Building : MonoBehaviour
{
    [Header("Building Stats")]
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private float currentHealth;
    
    [Header("References")]
    [SerializeField] private HealthBar healthBar;
    
    private bool isDestroyed = false;

    private void Start()
    {
        currentHealth = maxHealth;
        
        // Setup health bar
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        
        // Make sure building has the correct tag
        if (!gameObject.CompareTag("Building"))
        {
            gameObject.tag = "Building";
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
        
        Debug.Log($"Building took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            BuildingDestroyed();
        }
    }

    private void BuildingDestroyed()
    {
        isDestroyed = true;
        Debug.Log("Building Destroyed! Game Over!");
        
        // Notify game manager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnBuildingDestroyed();
        }
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDestroyed() => isDestroyed;
}
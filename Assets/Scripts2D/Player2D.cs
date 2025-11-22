using UnityEngine;

/// <summary>
/// Player character - Vampire Survivors style
/// WASD to move, auto-attacks enemies
/// </summary>
public class Player2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private HealthBar2D healthBar;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.right;
    private bool isAlive = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        currentHealth = maxHealth;
        
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    private void Update()
    {
        if (!isAlive) return;
        
        // Direct KeyCode input (works with both old and new Input System)
        float horizontal = 0f;
        float vertical = 0f;
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical = -1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;
        
        moveInput.x = horizontal;
        moveInput.y = vertical;
        
        // Normalize diagonal movement
        moveInput = moveInput.normalized;
        
        // Track last move direction for weapon
        if (moveInput.magnitude > 0.1f)
        {
            lastMoveDirection = moveInput;
        }
        
        // Flip sprite based on movement
        if (spriteRenderer != null && moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;
        
        // Move player
        rb.linearVelocity = moveInput * moveSpeed;
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;
        
        currentHealth -= damage;
        
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        
        // Flash red
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
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        isAlive = false;
        rb.linearVelocity = Vector2.zero;
        
        Debug.Log("Player died!");
        
        // Notify game manager
        GameManager2D gameManager = FindObjectOfType<GameManager2D>();
        if (gameManager != null)
        {
            gameManager.OnPlayerDied();
        }
    }

    public bool IsAlive() => isAlive;
    public Vector2 GetPosition() => transform.position;
    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public Vector2 GetLastMoveDirection() => lastMoveDirection;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Take damage from enemies
        Enemy2D enemy = collision.gameObject.GetComponent<Enemy2D>();
        if (enemy != null && enemy.IsAlive())
        {
            TakeDamage(5f); // Contact damage
        }
    }
}
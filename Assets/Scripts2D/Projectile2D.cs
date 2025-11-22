using UnityEngine;

/// <summary>
/// Projectile that moves and damages enemies
/// </summary>
public class Projectile2D : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float damage;
    private float maxDistance;
    private Vector2 startPosition;
    private bool isInitialized = false;

    public void Initialize(Vector2 dir, float spd, float dmg, float maxDist)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        maxDistance = maxDist;
        startPosition = transform.position;
        isInitialized = true;
        
        // Rotate projectile to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        if (!isInitialized) return;
        
        // Move projectile
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        
        // Destroy if traveled too far
        float distanceTraveled = Vector2.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if hit enemy
        Enemy2D enemy = collision.GetComponent<Enemy2D>();
        if (enemy != null && enemy.IsAlive())
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Projectile destroyed on hit
        }
    }

    private void OnBecameInvisible()
    {
        // Destroy projectile if it goes off screen
        Destroy(gameObject);
    }
}
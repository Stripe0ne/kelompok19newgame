using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private Player2D player;
    public float speed = 10f;
    public float damage = 20f;
    private Vector2 direction = Vector2.right;
    private bool initialized = false;

    void Start()
    {
        player = FindObjectOfType<Player2D>();

        if (player != null)
        {
            Vector2 lastDir = player.GetLastMoveDirection();
            if (lastDir.magnitude > 0.1f)
            {
                direction = lastDir.normalized;
            }
            else
            {
                direction = Vector2.right;
            }
        }
        else
        {
            direction = Vector2.right;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x) > 10f || Mathf.Abs(transform.position.y) > 10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy2D enemy = collision.GetComponentInParent<Enemy2D>();
        if (enemy != null && enemy.IsAlive())
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy2D enemy = collision.gameObject.GetComponentInParent<Enemy2D>();
        if (enemy != null && enemy.IsAlive())
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

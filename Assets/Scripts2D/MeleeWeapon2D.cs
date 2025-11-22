using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Melee weapon system - attacks enemies in range (sword/pedang)
/// </summary>
public class MeleeWeapon2D : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] private float damage = 30f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 0.8f;
    [SerializeField] private float attackArc = 90f; // Degrees of attack arc in front
    [SerializeField] private float swingSpeed = 0.15f; // Durasi animasi swing
    
    [Header("Visual")]
    [SerializeField] private GameObject weaponVisual;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] private Sprite weaponSpriteAsset; // Assign sprite pedang di Inspector
    [SerializeField] private Vector2 weaponOffset = new Vector2(0.5f, 0f); // Jarak dari center player ke tangan
    [SerializeField] private Vector2 weaponPivotOffset = new Vector2(-0.3f, 0f); // Pivot point untuk rotasi (ujung pegangan pedang)
    
    private float lastAttackTime;
    private Transform playerTransform;
    private Vector2 lastMoveDirection = Vector2.right;
    private bool isAttacking = false; // Status lagi nyerang atau ngga

    private void Start()
    {
        playerTransform = transform;
        
        // Auto-create weapon visual kalo belum ada
        if (weaponVisual == null)
        {
            CreateWeaponVisual();
        }
    }
    
    private void CreateWeaponVisual()
    {
        // Buat GameObject untuk weapon visual
        weaponVisual = new GameObject("WeaponVisual");
        weaponVisual.transform.SetParent(transform);
        weaponVisual.transform.localPosition = weaponOffset;
        weaponVisual.transform.localScale = Vector3.one;
        
        // Tambahkan SpriteRenderer
        weaponSprite = weaponVisual.AddComponent<SpriteRenderer>();
        weaponSprite.sortingOrder = 1; // Biar muncul di depan player
        
        // Assign sprite kalo udah di-set
        if (weaponSpriteAsset != null)
        {
            weaponSprite.sprite = weaponSpriteAsset;
        }
        else
        {
            // Bikin sprite temporary kalo belum ada sprite
            CreateDefaultWeaponSprite();
        }
        
        Debug.Log("Weapon visual created! Assign 'weaponSpriteAsset' di Inspector untuk custom sprite pedang.");
    }
    
    private void CreateDefaultWeaponSprite()
    {
        // Bikin texture sederhana untuk pedang (placeholder)
        Texture2D weaponTexture = new Texture2D(32, 8);
        Color[] colors = new Color[32 * 8];
        
        // Isi warna abu-abu untuk gagang dan putih untuk bilah pedang
        for (int i = 0; i < colors.Length; i++)
        {
            int x = i % 32;
            if (x < 8) colors[i] = new Color(0.4f, 0.2f, 0f); // Gagang coklat
            else colors[i] = new Color(0.8f, 0.8f, 0.8f); // Bilah pedang
        }
        
        weaponTexture.SetPixels(colors);
        weaponTexture.filterMode = FilterMode.Point; // Pixel art style
        weaponTexture.Apply();
        
        // Buat sprite dari texture, pivot di gagang (kiri)
        weaponSpriteAsset = Sprite.Create(weaponTexture, new Rect(0, 0, 32, 8), new Vector2(0.25f, 0.5f), 16f);
        weaponSprite.sprite = weaponSpriteAsset;
    }

    private void Update()
    {
        // Auto-attack enemies in melee range
        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            TryAttackNearbyEnemies();
        }
        
        // Update weapon rotation based on movement (only if not attacking)
        if (!isAttacking)
        {
            UpdateWeaponRotation();
        }
    }

    private void UpdateWeaponRotation()
    {
        // Get player movement component
        Player2D player = GetComponent<Player2D>();
        if (player != null)
        {
            Vector2 moveDir = player.GetLastMoveDirection();
            if (moveDir.magnitude > 0.1f)
            {
                lastMoveDirection = moveDir.normalized;
            }
        }
        
        // Rotate weapon visual to face move direction dan update posisi
        if (weaponVisual != null && lastMoveDirection.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;
            weaponVisual.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            // Update posisi weapon di tangan berdasarkan arah
            // Offset biar pedang selalu di "depan" player sesuai arah gerak
            Vector2 offsetDirection = lastMoveDirection.normalized * weaponOffset.magnitude;
            weaponVisual.transform.localPosition = offsetDirection;
            
            // Flip sprite kalo arah ke kiri biar ga kebalik
            if (weaponSprite != null)
            {
                // Kalo arah ke kiri bawah, flip vertikal
                weaponSprite.flipY = lastMoveDirection.x < -0.1f;
            }
        }
    }

    private void TryAttackNearbyEnemies()
    {
        // Find all enemies (Note: Sebaiknya diganti Physics2D.OverlapCircle untuk performa nanti)
        Enemy2D[] enemies = FindObjectsOfType<Enemy2D>();
        
        if (enemies.Length == 0) return;
        
        List<Enemy2D> enemiesHit = new List<Enemy2D>();
        
        // Check enemies in attack range and arc
        foreach (Enemy2D enemy in enemies)
        {
            if (!enemy.IsAlive()) continue;
            
            Vector2 toEnemy = (Vector2)(enemy.transform.position - playerTransform.position);
            float distance = toEnemy.magnitude;
            
            // Check if in range
            if (distance <= attackRange)
            {
                // Check if in attack arc (in front of player)
                float angle = Vector2.Angle(lastMoveDirection, toEnemy.normalized);
                
                if (angle <= attackArc / 2f)
                {
                    enemiesHit.Add(enemy);
                }
            }
        }
        
        // Attack if enemies found, OR just swing anyway if you want consistent attacks
        // For now, only swing if enemies are hit (Vampire Survivors style usually auto-attacks constantly though)
        if (enemiesHit.Count > 0)
        {
            StartCoroutine(SwingAttack(enemiesHit));
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator SwingAttack(List<Enemy2D> enemiesToHit)
    {
        isAttacking = true;
        
        // Calculate base angle from last move direction
        float baseAngle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;
        
        // Start angle (mundur dikit ke atas/bawah tergantung arah swing)
        float startAngle = baseAngle + (attackArc / 2f);
        float endAngle = baseAngle - (attackArc / 2f);
        
        // Alternating swing direction (optional: biar swing kiri-kanan gantian)
        // if (Time.frameCount % 2 == 0) { float temp = startAngle; startAngle = endAngle; endAngle = temp; }

        float elapsed = 0f;
        bool damageDealt = false;

        while (elapsed < swingSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingSpeed;
            
            // Smooth swing curve (Ease-Out)
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);
            
            if (weaponVisual != null)
            {
                weaponVisual.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            }

            // Deal damage di tengah-tengah swing biar pas
            if (!damageDealt && t >= 0.5f)
            {
                ApplyDamage(enemiesToHit);
                damageDealt = true;
            }
            
            yield return null;
        }
        
        // Ensure damage is dealt if swing finished too fast
        if (!damageDealt)
        {
            ApplyDamage(enemiesToHit);
        }

        // Balikin ke posisi idle atau biarin di posisi akhir sebentar
        isAttacking = false;
    }

    private void ApplyDamage(List<Enemy2D> enemies)
    {
        // Deal damage to all enemies hit
        foreach (Enemy2D enemy in enemies)
        {
            if (enemy != null && enemy.IsAlive())
            {
                enemy.TakeDamage(damage);
            }
        }
        
        Debug.Log($"Melee swing hit {enemies.Count} enemies!");
    }

    // Visualize attack range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw attack arc
        Vector3 forward = lastMoveDirection.magnitude > 0.1f ? 
            new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0) : Vector3.right;
        
        Quaternion leftRayRotation = Quaternion.AngleAxis(-attackArc / 2f, Vector3.forward);
        Quaternion rightRayRotation = Quaternion.AngleAxis(attackArc / 2f, Vector3.forward);
        
        Vector3 leftRay = leftRayRotation * forward * attackRange;
        Vector3 rightRay = rightRayRotation * forward * attackRange;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftRay);
        Gizmos.DrawLine(transform.position, transform.position + rightRay);
    }

    // Public methods for upgrades
    public void UpgradeDamage(float amount) => damage += amount;
    public void UpgradeRange(float amount) => attackRange += amount;
    public void UpgradeSpeed(float amount) => attackCooldown = Mathf.Max(0.1f, attackCooldown - amount);
}

using UnityEngine;

/// <summary>
/// CARA PALING GAMPANG!
/// Tinggal drag script ini ke Player GameObject → DONE!
/// Pedang langsung jalan otomatis!
/// </summary>
public class AutoSetupPedang : MonoBehaviour
{
    [Header("🗡️ AUTO SETUP PEDANG 🗡️")]
    [Tooltip("Drag sprite pedang di sini (opsional)")]
    public Sprite spritePedang;
    
    [Header("Settings Pedang (Opsional)")]
    public float damage = 30f;
    public float attackRange = 2f;
    public float attackSpeed = 0.8f;
    
    void Start()
    {
        SetupPedangSekarang();
    }
    
    void SetupPedangSekarang()
    {
        Debug.Log("=================================");
        Debug.Log("🗡️ MULAI SETUP PEDANG...");
        Debug.Log("=================================");
        
        // Check Player2D
        Player2D player = GetComponent<Player2D>();
        if (player == null)
        {
            Debug.LogWarning("⚠️ Player2D ga ada, coba ditambahin...");
            player = gameObject.AddComponent<Player2D>();
            Debug.Log("✅ Player2D berhasil ditambahkan!");
        }
        else
        {
            Debug.Log("✅ Player2D udah ada!");
        }
        
        // Check MeleeWeapon2D
        MeleeWeapon2D weapon = GetComponent<MeleeWeapon2D>();
        if (weapon == null)
        {
            Debug.Log("🔧 Menambahkan MeleeWeapon2D...");
            weapon = gameObject.AddComponent<MeleeWeapon2D>();
            Debug.Log("✅ MeleeWeapon2D berhasil ditambahkan!");
        }
        else
        {
            Debug.Log("✅ MeleeWeapon2D udah ada!");
        }
        
        // Set sprite via reflection
        if (spritePedang != null)
        {
            var weaponType = typeof(MeleeWeapon2D);
            var spriteField = weaponType.GetField("weaponSpriteAsset", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (spriteField != null)
            {
                spriteField.SetValue(weapon, spritePedang);
                Debug.Log($"✅ Sprite pedang di-set: {spritePedang.name}");
            }
            
            // Set stats
            var damageField = weaponType.GetField("damage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var rangeField = weaponType.GetField("attackRange", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cooldownField = weaponType.GetField("attackCooldown", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (damageField != null) damageField.SetValue(weapon, damage);
            if (rangeField != null) rangeField.SetValue(weapon, attackRange);
            if (cooldownField != null) cooldownField.SetValue(weapon, attackSpeed);
        }
        else
        {
            Debug.Log("💡 Sprite pedang kosong, bakal pakai sprite default.");
            Debug.Log("   Bisa drag sprite pedang ke field 'Sprite Pedang' di Inspector!");
        }
        
        Debug.Log("=================================");
        Debug.Log("🎉 SETUP SELESAI!");
        Debug.Log("🎮 Pedang siap dipake!");
        Debug.Log("   - Gerak pakai WASD");
        Debug.Log("   - Pedang otomatis ngikutin arah");
        Debug.Log("   - Auto-attack enemies");
        Debug.Log("=================================");
        
        // Destroy script ini setelah setup (ga diperlukan lagi)
        Destroy(this, 0.5f);
    }
    
    // Validasi di Inspector
    void OnValidate()
    {
        if (Application.isPlaying) return;
        
        // Info di console
        if (spritePedang != null)
        {
            Debug.Log($"💡 Sprite pedang siap: {spritePedang.name}");
        }
    }
}


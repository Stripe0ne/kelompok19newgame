using UnityEngine;

/// <summary>
/// FORCE SETUP PEDANG - Dijamin muncul!
/// Drag ke Player, Play game, pedang langsung keliatan!
/// </summary>
[ExecuteInEditMode]
public class ForceSetupPedang : MonoBehaviour
{
    [Header("⚔️ FORCE SETUP PEDANG ⚔️")]
    [Tooltip("Drag sprite pedang kamu di sini! (Opsional)")]
    public Sprite customWeaponSprite;
    
    [Tooltip("Warna pedang kalau ga pake custom sprite")]
    public Color weaponColor = Color.white;
    
    [Header("Settings")]
    public bool setupOnAwake = true;
    public Vector2 weaponOffset = new Vector2(0.7f, 0f);
    public float weaponScale = 1f;
    
    [Header("Rotation")]
    [Tooltip("Add rotation controller untuk customize rotasi")]
    public bool addRotationController = true;
    public float rotationSpeed = 20f;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private GameObject weaponVisual;
    private bool hasSetup = false;
    
    void Awake()
    {
        if (setupOnAwake && Application.isPlaying && !hasSetup)
        {
            ForceCreateWeapon();
        }
    }
    
    void Start()
    {
        if (Application.isPlaying && !hasSetup)
        {
            ForceCreateWeapon();
        }
    }
    
    [ContextMenu("Force Create Weapon NOW!")]
    public void ForceCreateWeapon()
    {
        if (hasSetup)
        {
            Log("⚠️ Weapon udah di-setup sebelumnya. Destroy dulu...");
            DestroyWeapon();
        }
        
        Log("========================================");
        Log("🗡️ FORCE CREATING WEAPON...");
        Log("========================================");
        
        // 1. Check/Add Player2D
        Player2D player = GetComponent<Player2D>();
        if (player == null)
        {
            player = gameObject.AddComponent<Player2D>();
            Log("✅ Player2D added");
        }
        else
        {
            Log("✅ Player2D exists");
        }
        
        // 2. Check/Add MeleeWeapon2D
        MeleeWeapon2D meleeWeapon = GetComponent<MeleeWeapon2D>();
        if (meleeWeapon == null)
        {
            meleeWeapon = gameObject.AddComponent<MeleeWeapon2D>();
            Log("✅ MeleeWeapon2D added");
        }
        else
        {
            Log("✅ MeleeWeapon2D exists");
        }
        
        // 3. Create BIG VISIBLE weapon visual
        weaponVisual = new GameObject("WeaponVisual_FORCED");
        weaponVisual.transform.SetParent(transform);
        weaponVisual.transform.localPosition = new Vector3(weaponOffset.x, weaponOffset.y, 0f);
        weaponVisual.transform.localScale = Vector3.one * weaponScale;
        weaponVisual.transform.localRotation = Quaternion.identity;
        
        Log($"✅ WeaponVisual created at position: {weaponVisual.transform.position}");
        
        // 4. Add VISIBLE sprite renderer
        SpriteRenderer sr = weaponVisual.AddComponent<SpriteRenderer>();
        
        // Pakai custom sprite kalau ada, kalau ga pakai default
        if (customWeaponSprite != null)
        {
            sr.sprite = customWeaponSprite;
            sr.color = Color.white; // Full color untuk custom sprite
            Log($"✅ Using CUSTOM sprite: {customWeaponSprite.name}");
        }
        else
        {
            sr.sprite = CreateBigWeaponSprite();
            sr.color = weaponColor;
            Log($"✅ Using DEFAULT sprite (placeholder)");
        }
        
        sr.sortingOrder = 10; // Di depan semua
        
        Log($"✅ SpriteRenderer added");
        Log($"   Color: {sr.color}");
        Log($"   Sorting Order: {sr.sortingOrder}");
        Log($"   Sprite: {sr.sprite.name}");
        
        // 5. Set via reflection ke MeleeWeapon2D
        var weaponType = typeof(MeleeWeapon2D);
        var visualField = weaponType.GetField("weaponVisual", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var spriteRendererField = weaponType.GetField("weaponSprite", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var spriteAssetField = weaponType.GetField("weaponSpriteAsset", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var offsetField = weaponType.GetField("weaponOffset", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (visualField != null) visualField.SetValue(meleeWeapon, weaponVisual);
        if (spriteRendererField != null) spriteRendererField.SetValue(meleeWeapon, sr);
        if (spriteAssetField != null && customWeaponSprite != null) 
            spriteAssetField.SetValue(meleeWeapon, customWeaponSprite);
        if (offsetField != null) 
            offsetField.SetValue(meleeWeapon, weaponOffset);
        
        Log("✅ Fields set via reflection to MeleeWeapon2D");
        
        // 6. Add rotation controller (optional)
        if (addRotationController)
        {
            WeaponRotationController rotController = GetComponent<WeaponRotationController>();
            if (rotController == null)
            {
                rotController = gameObject.AddComponent<WeaponRotationController>();
                rotController.rotationSpeed = rotationSpeed;
                Log("✅ WeaponRotationController added!");
            }
            else
            {
                rotController.rotationSpeed = rotationSpeed;
                Log("✅ WeaponRotationController updated!");
            }
        }
        
        hasSetup = true;
        
        Log("========================================");
        Log("🎉 PEDANG BERHASIL DIBUAT!");
        Log("========================================");
        
        if (customWeaponSprite != null)
        {
            Log($"   Sprite: {customWeaponSprite.name} (CUSTOM!)");
        }
        else
        {
            Log($"   Sprite: Default placeholder");
            Log($"   Warna: {weaponColor}");
        }
        
        Log($"   Position Offset: ({weaponOffset.x}, {weaponOffset.y})");
        Log($"   Scale: {weaponScale}");
        Log($"   Sorting Order: 10 (paling depan)");
        
        if (addRotationController)
        {
            Log($"   Rotation: ACTIVE (Speed: {rotationSpeed})");
        }
        else
        {
            Log("   Rotation: Using MeleeWeapon2D default");
        }
        
        Log("========================================");
        Log("🎮 CONTROLS:");
        Log("   WASD: Gerak player");
        Log("   Pedang otomatis rotasi ngikutin arah!");
        Log("========================================");
        
        // Destroy script ini setelah setup (ga diperlukan lagi)
        if (Application.isPlaying)
        {
            Destroy(this, 1f);
        }
    }
    
    private void DestroyWeapon()
    {
        Transform existing = transform.Find("WeaponVisual_FORCED");
        if (existing != null)
        {
            if (Application.isPlaying)
                Destroy(existing.gameObject);
            else
                DestroyImmediate(existing.gameObject);
        }
        hasSetup = false;
    }
    
    private Sprite CreateBigWeaponSprite()
    {
        // Buat texture GEDE biar keliatan!
        int width = 64;
        int height = 16;
        Texture2D tex = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        
        // Isi warna solid biar jelas keliatan
        for (int i = 0; i < colors.Length; i++)
        {
            int x = i % width;
            
            // Gagang (kiri)
            if (x < 12)
            {
                colors[i] = new Color(0.3f, 0.15f, 0f); // Coklat tua
            }
            // Bilah pedang (tengah-kanan)
            else
            {
                colors[i] = Color.white; // Putih terang
            }
        }
        
        tex.SetPixels(colors);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        
        // Pivot di gagang (kiri)
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, width, height), 
                                      new Vector2(0.2f, 0.5f), 16f);
        sprite.name = "BigWeaponSprite";
        
        return sprite;
    }
    
    private void Log(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw visual indicator
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw weapon position
        Vector3 weaponPos = transform.position + new Vector3(weaponOffset.x, weaponOffset.y, 0f);
        Gizmos.color = customWeaponSprite != null ? Color.cyan : Color.red;
        Gizmos.DrawWireSphere(weaponPos, 0.3f);
        Gizmos.DrawLine(transform.position, weaponPos);
        
        // Draw text
        #if UNITY_EDITOR
        string label = customWeaponSprite != null 
            ? $"PEDANG: {customWeaponSprite.name}" 
            : "PEDANG (Default)";
        UnityEditor.Handles.Label(weaponPos + Vector3.up * 0.5f, label);
        #endif
    }
}


using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// SCRIPT OTOMATIS SETUP 2D GAME! (Vampire Survivors style)
/// Cara Pakai:
/// 1. Copy file ini ke folder Assets/Editor/
/// 2. Di Unity menu bar: Tools → Auto Setup 2D Tower Defense
/// 3. DONE! Game langsung setup dengan sprites 2D
/// </summary>
public class AutoSetupGame2D : EditorWindow
{
    [MenuItem("Tools/Auto Setup 2D Tower Defense (Vampire Survivors Style)")]
    public static void SetupGame()
    {
        if (EditorUtility.DisplayDialog("Auto Setup 2D Game",
            "Ini akan setup 2D top-down game (Vampire Survivors style) dengan sprites sederhana. Lanjut?",
            "Setup!", "Cancel"))
        {
            CreateGameSetup();
        }
    }

    private static void CreateGameSetup()
    {
        // Clear existing objects
        ClearScene();
        
        // Create in order
        CreateBackground();
        GameObject tower = CreateTower();
        GameObject enemyPrefab = CreateEnemyPrefab();
        GameObject projectilePrefab = CreateProjectilePrefab();
        GameObject player = CreatePlayer(projectilePrefab);
        SetupCamera2D();
        GameObject waveManager = CreateWaveManager(enemyPrefab);
        CreateGameManager(tower, waveManager, player);
        CreateGameUI();
        
        EditorUtility.DisplayDialog("Setup Complete!",
            "2D Game setup complete! Press Play to test.\n\n" +
            "CONTROLS:\n" +
            "- WASD or Arrow Keys: Move player\n" +
            "- Auto-attack enemies in range\n\n" +
            "GAMEPLAY:\n" +
            "- Green triangle = Player (lu!)\n" +
            "- Red square = Tower (lindungi ini!)\n" +
            "- Blue circles = Enemies\n" +
            "- Wave 1: 4-6 enemies spawn\n\n" +
            "DEFEND THE TOWER & SURVIVE!",
            "Let's Go!");
    }

    private static void ClearScene()
    {
        var existing = GameObject.Find("Background");
        if (existing) DestroyImmediate(existing);
        existing = GameObject.Find("Tower");
        if (existing) DestroyImmediate(existing);
        existing = GameObject.Find("GameManager");
        if (existing) DestroyImmediate(existing);
        existing = GameObject.Find("WaveManager");
        if (existing) DestroyImmediate(existing);
        existing = GameObject.Find("GameUI");
        if (existing) DestroyImmediate(existing);
    }

    private static void CreateBackground()
    {
        // Create a dark background sprite
        GameObject bg = new GameObject("Background");
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        
        // Create a simple dark sprite
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.15f)); // Dark blue-gray
        tex.Apply();
        
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        sr.sprite = sprite;
        sr.sortingOrder = -10;
        
        // Scale to fill view
        bg.transform.localScale = new Vector3(50, 50, 1);
        bg.transform.position = Vector3.zero;
        
        Debug.Log("✓ Background created");
    }

    private static GameObject CreateTower()
    {
        GameObject tower = new GameObject("Tower");
        tower.tag = "Tower";
        CreateTag("Tower");
        
        // Create sprite renderer with red square
        SpriteRenderer sr = tower.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite(Color.red, 64);
        sr.sortingOrder = 1;
        
        // Position at center
        tower.transform.position = Vector3.zero;
        tower.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        
        // Add collider for detection
        BoxCollider2D collider = tower.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        
        // Add Tower2D script
        tower.AddComponent<Tower2D>();
        
        // Create health bar
        CreateHealthBarForObject(tower, "TowerHealthBar", new Vector3(0, 1.2f, 0), 
                                 new Color(0.2f, 0.8f, 0.2f));
        
        Debug.Log("✓ Tower created (red square)");
        return tower;
    }

    private static GameObject CreateProjectilePrefab()
    {
        GameObject projectile = new GameObject("Projectile");
        
        // Create sprite renderer with small yellow circle
        SpriteRenderer sr = projectile.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite(Color.yellow, 16);
        sr.sortingOrder = 3;
        
        projectile.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        
        // Add Rigidbody2D
        Rigidbody2D rb = projectile.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.isKinematic = true;
        
        // Add collider with trigger
        CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        collider.isTrigger = true;
        
        // Add Projectile2D script
        projectile.AddComponent<Projectile2D>();
        
        // Create Prefabs folder if doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(projectile, "Assets/Prefabs/Projectile2D.prefab");
        
        // Delete from scene
        DestroyImmediate(projectile);
        
        Debug.Log("✓ Projectile prefab created (yellow circle)");
        return prefab;
    }

    private static GameObject CreateEnemyPrefab()
    {
        GameObject enemy = new GameObject("Enemy");
        
        // Create sprite renderer with blue circle
        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite(new Color(0.3f, 0.5f, 0.9f), 32); // Blue
        sr.sortingOrder = 2;
        
        // Position and scale
        enemy.transform.position = Vector3.zero;
        enemy.transform.localScale = new Vector3(0.6f, 0.6f, 1);
        
        // Add Rigidbody2D
        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Add collider
        CircleCollider2D collider = enemy.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        
        // Add Enemy2D script
        enemy.AddComponent<Enemy2D>();
        
        // Create health bar
        CreateHealthBarForObject(enemy, "EnemyHealthBar", new Vector3(0, 0.8f, 0), 
                                 new Color(0.8f, 0.3f, 0.3f));
        
        // Create Prefabs folder if doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(enemy, "Assets/Prefabs/Enemy2D.prefab");
        
        // Delete from scene
        DestroyImmediate(enemy);
        
        Debug.Log("✓ Enemy prefab created (blue circle)");
        return prefab;
    }

    private static GameObject CreatePlayer(GameObject projectilePrefab)
    {
        GameObject player = new GameObject("Player");
        
        // Create sprite renderer with green triangle
        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = CreateTriangleSprite(new Color(0.2f, 0.8f, 0.2f)); // Green
        sr.sortingOrder = 5;
        
        // Position near tower
        player.transform.position = new Vector3(-2, 0, 0);
        player.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        
        // Add Rigidbody2D
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Add collider
        CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        
        // Add Player2D script
        Player2D playerScript = player.AddComponent<Player2D>();
        
        // Add Weapon2D script
        Weapon2D weapon = player.AddComponent<Weapon2D>();
        
        // Set weapon fields using reflection
        var weaponType = typeof(Weapon2D);
        SetField(weaponType, weapon, "projectilePrefab", projectilePrefab);
        SetField(weaponType, weapon, "damage", 20f);
        SetField(weaponType, weapon, "attackRange", 5f);
        SetField(weaponType, weapon, "attackCooldown", 0.5f);
        SetField(weaponType, weapon, "projectileSpeed", 10f);
        
        // Create health bar
        CreateHealthBarForObject(player, "PlayerHealthBar", new Vector3(0, 0.8f, 0), 
                                 new Color(0.2f, 0.8f, 0.2f));
        
        // Set player fields
        SetField(typeof(Player2D), playerScript, "spriteRenderer", sr);
        SetField(typeof(Player2D), playerScript, "moveSpeed", 5f);
        SetField(typeof(Player2D), playerScript, "maxHealth", 100f);
        
        Debug.Log("✓ Player created (green triangle) with weapon");
        return player;
    }

    private static void CreateHealthBarForObject(GameObject parent, string name, Vector3 position, Color fillColor)
    {
        // Create canvas for world space health bar
        GameObject canvasObj = new GameObject(name + "Canvas");
        canvasObj.transform.SetParent(parent.transform);
        canvasObj.transform.localPosition = position;
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1, 0.15f);
        canvasRect.localScale = Vector3.one;
        
        // Create slider
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(canvasObj.transform);
        sliderObj.transform.localPosition = Vector3.zero;
        
        Slider slider = sliderObj.AddComponent<Slider>();
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = Vector2.zero;
        sliderRect.anchorMax = Vector2.one;
        sliderRect.sizeDelta = Vector2.zero;
        sliderRect.localScale = Vector3.one;
        
        // Create background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.localScale = Vector3.one;
        
        // Create fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;
        fillAreaRect.localScale = Vector3.one;
        
        // Create fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = fillColor;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.localScale = Vector3.one;
        
        // Setup slider
        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;
        slider.interactable = false;
        
        // Add HealthBar2D script
        HealthBar2D healthBar = sliderObj.AddComponent<HealthBar2D>();
        
        // Set fields using reflection
        var healthBarType = typeof(HealthBar2D);
        SetField(healthBarType, healthBar, "healthSlider", slider);
        SetField(healthBarType, healthBar, "fillImage", fillImage);
        SetField(healthBarType, healthBar, "offset", position);
        
        // Connect to parent script
        if (parent.GetComponent<Tower2D>() != null)
        {
            var towerType = typeof(Tower2D);
            SetField(towerType, parent.GetComponent<Tower2D>(), "healthBar", healthBar);
            SetField(towerType, parent.GetComponent<Tower2D>(), "spriteRenderer", parent.GetComponent<SpriteRenderer>());
        }
        else if (parent.GetComponent<Enemy2D>() != null)
        {
            var enemyType = typeof(Enemy2D);
            SetField(enemyType, parent.GetComponent<Enemy2D>(), "healthBar", healthBar);
            SetField(enemyType, parent.GetComponent<Enemy2D>(), "spriteRenderer", parent.GetComponent<SpriteRenderer>());
        }
    }

    private static void SetupCamera2D()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Set to orthographic for 2D
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 8f;
            mainCamera.transform.position = new Vector3(0, 0, -10);
            mainCamera.transform.rotation = Quaternion.identity;
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        }
        Debug.Log("✓ Camera setup (2D Orthographic)");
    }

    private static GameObject CreateWaveManager(GameObject enemyPrefab)
    {
        GameObject waveManager = new GameObject("WaveManager");
        WaveManager2D wm = waveManager.AddComponent<WaveManager2D>();
        
        // Set fields
        var wmType = typeof(WaveManager2D);
        SetField(wmType, wm, "enemyPrefab", enemyPrefab);
        SetField(wmType, wm, "spawnRadius", 10f);
        SetField(wmType, wm, "timeBetweenWaves", 5f);
        SetField(wmType, wm, "timeBetweenSpawns", 0.5f);
        SetField(wmType, wm, "minEnemiesWave1", 4);
        SetField(wmType, wm, "maxEnemiesWave1", 6);
        SetField(wmType, wm, "enemiesIncreasePerWave", 2);
        SetField(wmType, wm, "baseHealth", 100f);
        SetField(wmType, wm, "baseSpeed", 2f);
        SetField(wmType, wm, "baseDamage", 10f);
        SetField(wmType, wm, "healthScalePerWave", 20f);
        SetField(wmType, wm, "speedScalePerWave", 0.2f);
        SetField(wmType, wm, "damageScalePerWave", 2f);
        
        Debug.Log("✓ WaveManager created");
        return waveManager;
    }

    private static void CreateGameManager(GameObject tower, GameObject waveManager, GameObject player)
    {
        GameObject gameManager = new GameObject("GameManager");
        GameManager2D gm = gameManager.AddComponent<GameManager2D>();
        
        // Set references
        var gmType = typeof(GameManager2D);
        SetField(gmType, gm, "gameCamera", Camera.main);
        SetField(gmType, gm, "tower", tower.GetComponent<Tower2D>());
        SetField(gmType, gm, "waveManager", waveManager.GetComponent<WaveManager2D>());
        SetField(gmType, gm, "player", player.GetComponent<Player2D>());
        SetField(gmType, gm, "cameraSize", 8f);
        SetField(gmType, gm, "cameraPosition", new Vector3(0, 0, -10));
        
        Debug.Log("✓ GameManager created");
    }

    private static void CreateGameUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("GameUI");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create texts
        CreateUIText(canvasObj, "WaveText", "Wave: 1",
                    new Vector2(10, -10), new Vector2(300, 50), TextAlignmentOptions.TopLeft);
        
        CreateUIText(canvasObj, "EnemyCountText", "Enemies: 0",
                    new Vector2(10, -70), new Vector2(300, 50), TextAlignmentOptions.TopLeft);
        
        CreateUIText(canvasObj, "TowerHealthText", "Tower HP: 1000/1000",
                    new Vector2(0, -10), new Vector2(400, 50), TextAlignmentOptions.Top);
        
        CreateUIText(canvasObj, "PlayerHealthText", "Player HP: 100/100",
                    new Vector2(-10, -10), new Vector2(300, 50), TextAlignmentOptions.TopRight);
        
        Debug.Log("✓ Game UI created");
    }

    private static void CreateUIText(GameObject parent, string name, string text, Vector2 position, Vector2 size, TextAlignmentOptions alignment)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        
        if (alignment == TextAlignmentOptions.TopLeft)
        {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
        }
        else if (alignment == TextAlignmentOptions.Top)
        {
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0.5f, 1);
        }
        else if (alignment == TextAlignmentOptions.TopRight)
        {
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
        }
        
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.color = Color.white;
        tmp.alignment = alignment;
        tmp.outlineColor = Color.black;
        tmp.outlineWidth = 0.2f;
    }

    // Helper: Create square sprite
    private static Sprite CreateSquareSprite(Color color, int size)
    {
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    // Helper: Create triangle sprite
    private static Sprite CreateTriangleSprite(Color color)
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size);
        
        // Create triangle pointing up
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Triangle math: check if point is inside triangle
                float centerX = size / 2f;
                float topY = size * 0.8f;
                float bottomY = size * 0.2f;
                float width = (topY - y) / (topY - bottomY) * (size * 0.6f);
                
                if (y >= bottomY && y <= topY &&
                    x >= centerX - width && x <= centerX + width)
                {
                    tex.SetPixel(x, y, color);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    // Helper: Create circle sprite
    private static Sprite CreateCircleSprite(Color color, int size)
    {
        Texture2D tex = new Texture2D(size, size);
        int center = size / 2;
        float radius = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (dist <= radius)
                {
                    tex.SetPixel(x, y, color);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private static void CreateTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tag))
            {
                found = true;
                break;
            }
        }
        
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = tag;
            tagManager.ApplyModifiedProperties();
        }
    }

    private static void SetField(System.Type type, object instance, string fieldName, object value)
    {
        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(instance, value);
        }
    }
}
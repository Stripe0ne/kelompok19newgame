using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;

/// <summary>
/// AUTO SETUP WITH REAL GAME ASSETS!
/// Uses actual sprites from Asset game folder
/// </summary>
public class AutoSetupGameWithAssets : EditorWindow
{
    [MenuItem("Tools/Auto Setup Game WITH REAL ASSETS")]
    public static void SetupGame()
    {
        if (EditorUtility.DisplayDialog("Auto Setup with Real Assets",
            "Ini akan setup game menggunakan ASSET REAL lu!\n\n" +
            "- MC.png untuk player\n" +
            "- Pasukan Raja iblis.png untuk enemy\n" +
            "- tower.png untuk tower\n" +
            "- Map.png untuk background\n" +
            "- MELEE combat (pedang)\n\n" +
            "Asset folder harus ada di: Assets/Asset game/\n\n" +
            "Lanjut?",
            "Setup!", "Cancel"))
        {
            CreateGameSetup();
        }
    }

    private static void CreateGameSetup()
    {
        // First, import assets from Asset game folder
        string assetGamePath = "Assets/Asset game";
        
        if (!AssetDatabase.IsValidFolder(assetGamePath))
        {
            EditorUtility.DisplayDialog("Error!", 
                "Folder 'Asset game' tidak ditemukan!\n\n" +
                "Pastikan folder 'Asset game' ada di Assets/\n" +
                "Drag folder 'Asset game' ke Unity Project window!",
                "OK");
            return;
        }
        
        // Clear existing objects
        ClearScene();
        
        // Create game with real assets
        CreateBackground();
        GameObject tower = CreateTower();
        GameObject enemyPrefab = CreateEnemyPrefab();
        GameObject player = CreatePlayer();
        SetupCamera2D();
        GameObject waveManager = CreateWaveManager(enemyPrefab);
        CreateGameUI(); // Create UI FIRST so GameManager can connect to it
        CreateGameManager(tower, waveManager, player);
        
        EditorUtility.DisplayDialog("Setup Complete!",
            "Game setup complete dengan REAL ASSETS!\n\n" +
            "CONTROLS:\n" +
            "- WASD: Move player\n" +
            "- MELEE COMBAT: Otomatis attack musuh deket\n\n" +
            "CHARACTERS:\n" +
            "- Player: MC.png (pedang!)\n" +
            "- Enemy: Pasukan Raja Iblis\n" +
            "- Tower: tower.png\n" +
            "- Map: Map.png\n\n" +
            "SIAP MAIN!",
            "Let's Go!");
    }

    private static void ClearScene()
    {
        var existing = GameObject.Find("Background");
        if (existing) DestroyImmediate(existing);
        existing = GameObject.Find("Tower");
        if (existing) DestroyImmediate(existing);
        existing = GameObject.Find("Player");
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
        // Load Map.png
        Sprite mapSprite = LoadSprite("Assets/Asset game/Lingkungan/Map.png");
        
        GameObject bg = new GameObject("Background");
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        
        if (mapSprite != null)
        {
            sr.sprite = mapSprite;
            sr.sortingOrder = -10;
            
            // Scale to fit camera view
            bg.transform.localScale = new Vector3(3, 3, 1);
            bg.transform.position = Vector3.zero;
            
            Debug.Log("✓ Background created with Map.png");
        }
        else
        {
            // Fallback to colored background
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.15f));
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
            sr.sortingOrder = -10;
            bg.transform.localScale = new Vector3(50, 50, 1);
            
            Debug.LogWarning("Map.png not found, using fallback background");
        }
    }

    private static GameObject CreateTower()
    {
        // Load tower.png
        Sprite towerSprite = LoadSprite("Assets/Asset game/Lingkungan/tower.png");
        
        GameObject tower = new GameObject("Tower");
        tower.tag = "Tower";
        CreateTag("Tower");
        
        SpriteRenderer sr = tower.AddComponent<SpriteRenderer>();
        
        if (towerSprite != null)
        {
            sr.sprite = towerSprite;
            sr.sortingOrder = 1;
            tower.transform.position = Vector3.zero;
            tower.transform.localScale = new Vector3(0.8f, 0.8f, 1); // FIXED: Smaller tower
            
            Debug.Log("✓ Tower created with tower.png");
        }
        else
        {
            // Fallback
            sr.sprite = CreateSquareSprite(Color.red, 64);
            sr.sortingOrder = 1;
            tower.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            
            Debug.LogWarning("tower.png not found, using fallback");
        }
        
        // Add collider
        BoxCollider2D collider = tower.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        
        // Add Tower2D script
        tower.AddComponent<Tower2D>();
        
        // Create health bar
        CreateHealthBarForObject(tower, "TowerHealthBar", new Vector3(0, 1.5f, 0), 
                                 new Color(0.2f, 0.8f, 0.2f));
        
        return tower;
    }

    private static GameObject CreateEnemyPrefab()
    {
        // Load Pasukan Raja iblis.png
        Sprite enemySprite = LoadSprite("Assets/Asset game/Karakter/Pasukan Raja iblis.png");
        
        GameObject enemy = new GameObject("Enemy");
        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        
        if (enemySprite != null)
        {
            sr.sprite = enemySprite;
            sr.sortingOrder = 2;
            enemy.transform.localScale = new Vector3(0.4f, 0.4f, 1); // FIXED: Smaller enemy
            
            Debug.Log("✓ Enemy created with Pasukan Raja iblis.png");
        }
        else
        {
            // Fallback
            sr.sprite = CreateCircleSprite(new Color(0.3f, 0.5f, 0.9f), 32);
            sr.sortingOrder = 2;
            enemy.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            
            Debug.LogWarning("Pasukan Raja iblis.png not found, using fallback");
        }
        
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
        CreateHealthBarForObject(enemy, "EnemyHealthBar", new Vector3(0, 1f, 0), 
                                 new Color(0.8f, 0.3f, 0.3f));
        
        // Create Prefabs folder
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(enemy, "Assets/Prefabs/Enemy2D.prefab");
        DestroyImmediate(enemy);
        
        return prefab;
    }

    private static GameObject CreatePlayer()
    {
        // Load MC.png
        Sprite playerSprite = LoadSprite("Assets/Asset game/Karakter/MC.png");
        Sprite weaponSprite = LoadSprite("Assets/Asset game/Alat/senjata.png");
        
        GameObject player = new GameObject("Player");
        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        
        if (playerSprite != null)
        {
            sr.sprite = playerSprite;
            sr.sortingOrder = 5;
            player.transform.position = new Vector3(-3, 0, 0);
            player.transform.localScale = new Vector3(0.4f, 0.4f, 1); // FIXED: Much smaller scale
            
            Debug.Log("✓ Player created with MC.png");
        }
        else
        {
            // Fallback
            sr.sprite = CreateTriangleSprite(new Color(0.2f, 0.8f, 0.2f));
            sr.sortingOrder = 5;
            player.transform.position = new Vector3(-2, 0, 0);
            player.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            
            Debug.LogWarning("MC.png not found, using fallback");
        }
        
        // Add Rigidbody2D
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Add collider
        CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        
        // Add Player2D script
        Player2D playerScript = player.AddComponent<Player2D>();
        
        // Add MELEE weapon (pedang!)
        MeleeWeapon2D meleeWeapon = player.AddComponent<MeleeWeapon2D>();
        
        // Create weapon visual if sprite available
        if (weaponSprite != null)
        {
            GameObject weaponVisual = new GameObject("Weapon");
            weaponVisual.transform.SetParent(player.transform);
            weaponVisual.transform.localPosition = new Vector3(0.8f, -0.2f, 0); // FIXED: Position to side, not overlap
            weaponVisual.transform.localScale = new Vector3(0.8f, 0.8f, 1); // FIXED: Smaller scale
            
            SpriteRenderer weaponSR = weaponVisual.AddComponent<SpriteRenderer>();
            weaponSR.sprite = weaponSprite;
            weaponSR.sortingOrder = 4; // FIXED: Behind player so doesn't cover face
            
            // Set weapon fields
            SetField(typeof(MeleeWeapon2D), meleeWeapon, "weaponVisual", weaponVisual);
            SetField(typeof(MeleeWeapon2D), meleeWeapon, "weaponSprite", weaponSR);
            
            Debug.Log("✓ Weapon visual created with senjata.png");
        }
        
        // Set melee weapon stats
        SetField(typeof(MeleeWeapon2D), meleeWeapon, "damage", 30f);
        SetField(typeof(MeleeWeapon2D), meleeWeapon, "attackRange", 2f);
        SetField(typeof(MeleeWeapon2D), meleeWeapon, "attackCooldown", 0.8f);
        SetField(typeof(MeleeWeapon2D), meleeWeapon, "attackArc", 120f);
        
        // Create health bar
        CreateHealthBarForObject(player, "PlayerHealthBar", new Vector3(0, 1.2f, 0), 
                                 new Color(0.2f, 0.8f, 0.2f));
        
        // Set player fields
        SetField(typeof(Player2D), playerScript, "spriteRenderer", sr);
        SetField(typeof(Player2D), playerScript, "moveSpeed", 5f);
        SetField(typeof(Player2D), playerScript, "maxHealth", 100f);
        
        return player;
    }

    private static void CreateHealthBarForObject(GameObject parent, string name, Vector3 position, Color fillColor)
    {
        GameObject canvasObj = new GameObject(name + "Canvas");
        canvasObj.transform.SetParent(parent.transform);
        canvasObj.transform.localPosition = position;
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1.5f, 0.2f);
        canvasRect.localScale = Vector3.one;
        
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(canvasObj.transform);
        sliderObj.transform.localPosition = Vector3.zero;
        
        Slider slider = sliderObj.AddComponent<Slider>();
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = Vector2.zero;
        sliderRect.anchorMax = Vector2.one;
        sliderRect.sizeDelta = Vector2.zero;
        
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;
        
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = fillColor;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        
        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;
        slider.interactable = false;
        
        HealthBar2D healthBar = sliderObj.AddComponent<HealthBar2D>();
        SetField(typeof(HealthBar2D), healthBar, "healthSlider", slider);
        SetField(typeof(HealthBar2D), healthBar, "fillImage", fillImage);
        SetField(typeof(HealthBar2D), healthBar, "offset", position);
        
        if (parent.GetComponent<Tower2D>() != null)
        {
            SetField(typeof(Tower2D), parent.GetComponent<Tower2D>(), "healthBar", healthBar);
            SetField(typeof(Tower2D), parent.GetComponent<Tower2D>(), "spriteRenderer", parent.GetComponent<SpriteRenderer>());
        }
        else if (parent.GetComponent<Enemy2D>() != null)
        {
            SetField(typeof(Enemy2D), parent.GetComponent<Enemy2D>(), "healthBar", healthBar);
            SetField(typeof(Enemy2D), parent.GetComponent<Enemy2D>(), "spriteRenderer", parent.GetComponent<SpriteRenderer>());
        }
        else if (parent.GetComponent<Player2D>() != null)
        {
            SetField(typeof(Player2D), parent.GetComponent<Player2D>(), "healthBar", healthBar);
        }
    }

    private static void SetupCamera2D()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
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
        
        var wmType = typeof(WaveManager2D);
        SetField(wmType, wm, "enemyPrefab", enemyPrefab);
        SetField(wmType, wm, "spawnRadius", 12f);
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
        
        var gmType = typeof(GameManager2D);
        SetField(gmType, gm, "gameCamera", Camera.main);
        SetField(gmType, gm, "tower", tower.GetComponent<Tower2D>());
        SetField(gmType, gm, "waveManager", waveManager.GetComponent<WaveManager2D>());
        SetField(gmType, gm, "player", player.GetComponent<Player2D>());
        SetField(gmType, gm, "cameraSize", 8f);
        SetField(gmType, gm, "cameraPosition", new Vector3(0, 0, -10));
        
        // Connect UI elements
        GameObject uiCanvas = GameObject.Find("GameUI");
        if (uiCanvas != null)
        {
            Transform waveText = uiCanvas.transform.Find("WaveText");
            Transform enemyText = uiCanvas.transform.Find("EnemyCountText");
            Transform towerHP = uiCanvas.transform.Find("TowerHealthText");
            Transform playerHP = uiCanvas.transform.Find("PlayerHealthText");
            
            if (waveText != null) SetField(gmType, gm, "waveText", waveText.GetComponent<TextMeshProUGUI>());
            if (enemyText != null) SetField(gmType, gm, "enemyCountText", enemyText.GetComponent<TextMeshProUGUI>());
            if (towerHP != null) SetField(gmType, gm, "towerHealthText", towerHP.GetComponent<TextMeshProUGUI>());
            if (playerHP != null) SetField(gmType, gm, "playerHealthText", playerHP.GetComponent<TextMeshProUGUI>());
        }
        
        Debug.Log("✓ GameManager created with UI connections");
    }

    private static void CreateGameUI()
    {
        GameObject canvasObj = new GameObject("GameUI");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
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

    // Helper: Load sprite from path
    private static Sprite LoadSprite(string path)
    {
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texture != null)
        {
            // Make sure texture is readable
            string assetPath = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            
            if (importer != null)
            {
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }
            }
            
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        return null;
    }

    // Fallback sprites
    private static Sprite CreateSquareSprite(Color color, int size)
    {
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private static Sprite CreateTriangleSprite(Color color)
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float centerX = size / 2f;
                float topY = size * 0.8f;
                float bottomY = size * 0.2f;
                float width = (topY - y) / (topY - bottomY) * (size * 0.6f);
                
                if (y >= bottomY && y <= topY && x >= centerX - width && x <= centerX + width)
                    tex.SetPixel(x, y, color);
                else
                    tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

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
                tex.SetPixel(x, y, dist <= radius ? color : Color.clear);
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
            if (tagsProp.GetArrayElementAtIndex(i).stringValue.Equals(tag))
            {
                found = true;
                break;
            }
        }
        
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            tagsProp.GetArrayElementAtIndex(0).stringValue = tag;
            tagManager.ApplyModifiedProperties();
        }
    }

    private static void SetField(System.Type type, object instance, string fieldName, object value)
    {
        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null) field.SetValue(instance, value);
    }
}
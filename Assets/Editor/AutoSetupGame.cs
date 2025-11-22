using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// SCRIPT OTOMATIS SETUP GAME!
/// Cara Pakai:
/// 1. Copy file ini ke folder Assets/Editor/ (buat folder Editor kalo belum ada)
/// 2. Di Unity menu bar: Tools → Auto Setup Tower Defense Game
/// 3. DONE! Game langsung setup semua
/// </summary>
public class AutoSetupGame : EditorWindow
{
    [MenuItem("Tools/Auto Setup Tower Defense Game")]
    public static void SetupGame()
    {
        if (EditorUtility.DisplayDialog("Auto Setup Game",
            "Ini akan setup semua objects dan prefabs untuk game. Lanjut?",
            "Setup!", "Cancel"))
        {
            CreateGameSetup();
        }
    }

    private static void CreateGameSetup()
    {
        // Clear existing objects if any
        ClearScene();
        
        // Create in order
        CreateGround();
        GameObject building = CreateBuilding();
        GameObject enemyPrefab = CreateEnemyPrefab();
        SetupCamera();
        GameObject waveManager = CreateWaveManager(enemyPrefab);
        CreateGameManager(building, waveManager);
        CreateGameUI();
        
        EditorUtility.DisplayDialog("Setup Complete!",
            "Game setup complete! Press Play button to test.\n\n" +
            "Wave 1 will start automatically and spawn 4-6 enemies.\n" +
            "Enemies will walk to the building and attack it.",
            "Awesome!");
    }

    private static void ClearScene()
    {
        // Optional: clean up existing game objects
        var existing = GameObject.Find("Ground");
        if (existing) DestroyImmediate(existing);
        existing = GameObject.Find("Building");
        if (existing) DestroyImmediate(existing);
        existing = GameObject.Find("GameManager");
        if (existing) DestroyImmediate(existing);
        existing = GameObject.Find("WaveManager");
        if (existing) DestroyImmediate(existing);
        existing = GameObject.Find("GameUI");
        if (existing) DestroyImmediate(existing);
    }

    private static void CreateGround()
    {
        // Create plane for ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(5, 1, 5);
        
        // Create material
        Material groundMat = new Material(Shader.Find("Standard"));
        groundMat.color = new Color(0.2f, 0.6f, 0.2f); // Green
        ground.GetComponent<Renderer>().material = groundMat;
        
        Debug.Log("✓ Ground created");
    }

    private static GameObject CreateBuilding()
    {
        // Create cube for building
        GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
        building.name = "Building";
        building.transform.position = new Vector3(0, 1, 0);
        building.transform.localScale = new Vector3(2, 2, 2);
        building.tag = "Building";
        
        // Create tag if doesn't exist
        CreateTag("Building");
        
        // Create material
        Material buildingMat = new Material(Shader.Find("Standard"));
        buildingMat.color = new Color(0.3f, 0.3f, 0.8f); // Blue
        building.GetComponent<Renderer>().material = buildingMat;
        
        // Add Building script
        building.AddComponent<Building>();
        
        // Create health bar for building
        CreateHealthBarForObject(building, "BuildingHealthBar", new Vector3(0, 3, 0), 
                                 new Color(0.2f, 0.8f, 0.2f)); // Green fill
        
        Debug.Log("✓ Building created with health bar");
        return building;
    }

    private static GameObject CreateEnemyPrefab()
    {
        // Create capsule for enemy
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemy.name = "Enemy";
        enemy.transform.position = new Vector3(0, 1, 0);
        enemy.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
        
        // Create material
        Material enemyMat = new Material(Shader.Find("Standard"));
        enemyMat.color = new Color(0.8f, 0.2f, 0.2f); // Red
        enemy.GetComponent<Renderer>().material = enemyMat;
        
        // Add Enemy script
        enemy.AddComponent<Enemy>();
        
        // Create health bar for enemy
        CreateHealthBarForObject(enemy, "EnemyHealthBar", new Vector3(0, 2, 0), 
                                 new Color(0.8f, 0.2f, 0.2f)); // Red fill
        
        // Create Prefabs folder if doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(enemy, "Assets/Prefabs/Enemy.prefab");
        
        // Delete from scene
        DestroyImmediate(enemy);
        
        Debug.Log("✓ Enemy prefab created with health bar");
        return prefab;
    }

    private static void CreateHealthBarForObject(GameObject parent, string name, Vector3 position, Color fillColor)
    {
        // Create canvas
        GameObject canvasObj = new GameObject(name + "Canvas");
        canvasObj.transform.SetParent(parent.transform);
        canvasObj.transform.localPosition = position;
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(200, 50);
        canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        
        // Create slider
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(canvasObj.transform);
        sliderObj.transform.localPosition = Vector3.zero;
        
        Slider slider = sliderObj.AddComponent<Slider>();
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = Vector2.zero;
        sliderRect.anchorMax = Vector2.one;
        sliderRect.sizeDelta = Vector2.zero;
        
        // Create background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        // Create fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;
        
        // Create fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = fillColor;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        
        // Setup slider references
        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;
        slider.interactable = false;
        
        // Add HealthBar script
        HealthBar healthBar = sliderObj.AddComponent<HealthBar>();
        
        // Use reflection to set private fields
        var healthBarType = typeof(HealthBar);
        var sliderField = healthBarType.GetField("healthSlider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fillField = healthBarType.GetField("fillImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (sliderField != null) sliderField.SetValue(healthBar, slider);
        if (fillField != null) fillField.SetValue(healthBar, fillImage);
        
        // Connect health bar to parent script
        if (parent.GetComponent<Building>() != null)
        {
            var buildingType = typeof(Building);
            var healthBarField = buildingType.GetField("healthBar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (healthBarField != null)
            {
                healthBarField.SetValue(parent.GetComponent<Building>(), healthBar);
            }
        }
        else if (parent.GetComponent<Enemy>() != null)
        {
            var enemyType = typeof(Enemy);
            var healthBarField = enemyType.GetField("healthBar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (healthBarField != null)
            {
                healthBarField.SetValue(parent.GetComponent<Enemy>(), healthBar);
            }
        }
    }

    private static void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0, 15, -10);
            mainCamera.transform.eulerAngles = new Vector3(50, 0, 0);
            mainCamera.backgroundColor = new Color(0.5f, 0.7f, 1f); // Sky blue
        }
        Debug.Log("✓ Camera positioned");
    }

    private static GameObject CreateWaveManager(GameObject enemyPrefab)
    {
        GameObject waveManager = new GameObject("WaveManager");
        WaveManager wm = waveManager.AddComponent<WaveManager>();
        
        // Set fields using reflection
        var wmType = typeof(WaveManager);
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

    private static void CreateGameManager(GameObject building, GameObject waveManager)
    {
        GameObject gameManager = new GameObject("GameManager");
        GameManager gm = gameManager.AddComponent<GameManager>();
        
        // Set references
        var gmType = typeof(GameManager);
        SetField(gmType, gm, "gameCamera", Camera.main);
        SetField(gmType, gm, "building", building.GetComponent<Building>());
        SetField(gmType, gm, "waveManager", waveManager.GetComponent<WaveManager>());
        SetField(gmType, gm, "cameraPosition", new Vector3(0, 15, -10));
        SetField(gmType, gm, "cameraRotation", new Vector3(50, 0, 0));
        
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
        
        // Create Wave Text
        CreateUIText(canvasObj, "WaveText", "Wave: 1", 
                    new Vector2(10, -10), new Vector2(300, 50), TextAlignmentOptions.TopLeft);
        
        // Create Enemy Count Text
        CreateUIText(canvasObj, "EnemyCountText", "Enemies: 0", 
                    new Vector2(10, -70), new Vector2(300, 50), TextAlignmentOptions.TopLeft);
        
        // Create Building Health Text
        CreateUIText(canvasObj, "BuildingHealthText", "Building HP: 1000/1000", 
                    new Vector2(0, -10), new Vector2(400, 50), TextAlignmentOptions.Top);
        
        Debug.Log("✓ Game UI created");
    }

    private static void CreateUIText(GameObject parent, string name, string text, Vector2 position, Vector2 size, TextAlignmentOptions alignment)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        
        // Set anchors based on alignment
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
        
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.color = Color.white;
        tmp.alignment = alignment;
        
        // Add outline for better visibility
        tmp.outlineColor = Color.black;
        tmp.outlineWidth = 0.2f;
    }

    private static void CreateTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        
        // Check if tag already exists
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
        
        // Add tag if not found
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
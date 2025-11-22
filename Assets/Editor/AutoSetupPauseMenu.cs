using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

/// <summary>
/// FULL AUTO SETUP - Tinggal klik menu aja!
/// Tools > Setup Pause Menu (Auto)
/// </summary>
public class AutoSetupPauseMenu : MonoBehaviour
{
    [MenuItem("Tools/Setup Pause Menu (Auto)")]
    public static void SetupPauseMenuAuto()
    {
        Debug.Log("Starting Auto Setup...");
        
        // Auto-find sprites
        Sprite pauseBarSprite = FindSprite("Pause bar");
        Sprite pauseLogoSprite = FindSprite("pause logo");
        Sprite resumeButtonSprite = FindSprite("Resume button");
        Sprite restartButtonSprite = FindSprite("restart button");
        Sprite homeButtonSprite = FindSprite("Home button");
        
        Sprite settingsBarSprite = FindSprite("settings bar");
        Sprite settingLogoSprite = FindSprite("setting logo");
        Sprite backButtonSprite = FindSprite("back button");
        Sprite backsoundSettingSprite = FindSprite("backsound setting");
        Sprite soundEffectSettingSprite = FindSprite("sound effect setting");
        Sprite soundBarIntensitySprite = FindSprite("sound & bgm bar intensity");
        
        // Create Canvas
        Canvas canvas = FindOrCreateCanvas();
        
        // Create Pause Panel
        GameObject pausePanel = CreatePauseMenu(canvas.transform, pauseBarSprite, pauseLogoSprite, 
            resumeButtonSprite, restartButtonSprite, settingLogoSprite, homeButtonSprite);
        
        // Create Settings Panel
        GameObject settingsPanel = CreateSettingsMenu(canvas.transform, settingsBarSprite, settingLogoSprite,
            backButtonSprite, backsoundSettingSprite, soundEffectSettingSprite, soundBarIntensitySprite);
        
        // Setup PauseMenu Script
        SetupPauseMenuScript(pausePanel, settingsPanel);
        
        // Setup AudioManager
        SetupAudioManager();
        
        Debug.Log("✅ AUTO SETUP COMPLETE! Press ESC or P to test!");
        
        EditorUtility.DisplayDialog("Setup Complete!", 
            "Pause Menu & Settings sudah siap!\n\nTekan ESC atau P untuk pause game.", "OK");
    }
    
    private static Sprite FindSprite(string spriteName)
    {
        string[] guids = AssetDatabase.FindAssets(spriteName + " t:Sprite");
        
        if (guids.Length == 0)
        {
            Debug.LogWarning($"Sprite '{spriteName}' not found!");
            return null;
        }
        
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
        
        // Try to find exact match first
        Sprite exactMatch = sprites.OfType<Sprite>().FirstOrDefault(s => s.name == spriteName);
        if (exactMatch != null) return exactMatch;
        
        // Return first sprite found
        Sprite sprite = sprites.OfType<Sprite>().FirstOrDefault();
        if (sprite != null)
        {
            Debug.Log($"Found sprite: {sprite.name} at {path}");
        }
        return sprite;
    }
    
    private static Canvas FindOrCreateCanvas()
    {
        Canvas existingCanvas = Object.FindObjectOfType<Canvas>();
        if (existingCanvas != null)
        {
            return existingCanvas;
        }

        GameObject canvasObj = new GameObject("Canvas");
        Canvas newCanvas = canvasObj.AddComponent<Canvas>();
        newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        newCanvas.sortingOrder = 100; // Render di atas semua game objects
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Add EventSystem if not exists
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        return newCanvas;
    }

    private static GameObject CreatePauseMenu(Transform parent, Sprite pauseBarSprite, Sprite pauseLogoSprite,
        Sprite resumeButtonSprite, Sprite restartButtonSprite, Sprite settingLogoSprite, Sprite homeButtonSprite)
    {
        // Main Pause Panel
        GameObject panel = new GameObject("PausePanel");
        panel.transform.SetParent(parent, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.8f);
        panelBg.raycastTarget = true;
        
        // Pause Bar
        GameObject pauseBar = new GameObject("PauseBar");
        pauseBar.transform.SetParent(panel.transform, false);
        
        RectTransform barRect = pauseBar.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.5f, 0.5f);
        barRect.anchorMax = new Vector2(0.5f, 0.5f);
        barRect.pivot = new Vector2(0.5f, 0.5f);
        barRect.anchoredPosition = Vector2.zero;
        barRect.localPosition = Vector3.zero;
        barRect.sizeDelta = new Vector2(50, 35);
        
        Image barImage = pauseBar.AddComponent<Image>();
        if (pauseBarSprite != null) barImage.sprite = pauseBarSprite;
        
        // Buttons (cuma Resume & Restart, NO LOGO)
        GameObject resumeBtn = CreateButton("ResumeButton", pauseBar.transform, resumeButtonSprite, 
            new Vector2(0.5f, 0.5f), new Vector2(30, 10));
        resumeBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 8);
        
        GameObject restartBtn = CreateButton("RestartButton", pauseBar.transform, restartButtonSprite, 
            new Vector2(0.5f, 0.5f), new Vector2(30, 10));
        restartBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -8);
        
        panel.SetActive(false);
        return panel;
    }

    private static GameObject CreateSettingsMenu(Transform parent, Sprite settingsBarSprite, Sprite settingLogoSprite,
        Sprite backButtonSprite, Sprite backsoundSettingSprite, Sprite soundEffectSettingSprite, Sprite soundBarIntensitySprite)
    {
        GameObject panel = new GameObject("SettingsPanel");
        panel.transform.SetParent(parent, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.8f);
        panelBg.raycastTarget = true;
        
        GameObject settingsBar = new GameObject("SettingsBar");
        settingsBar.transform.SetParent(panel.transform, false);
        
        RectTransform barRect = settingsBar.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.5f, 0.5f);
        barRect.anchorMax = new Vector2(0.5f, 0.5f);
        barRect.pivot = new Vector2(0.5f, 0.5f);
        barRect.anchoredPosition = Vector2.zero;
        barRect.localPosition = Vector3.zero;
        barRect.sizeDelta = new Vector2(60, 95);
        
        Image barImage = settingsBar.AddComponent<Image>();
        if (settingsBarSprite != null) barImage.sprite = settingsBarSprite;
        
        if (settingLogoSprite != null)
        {
            CreateImage("SettingsLogo", settingsBar.transform, settingLogoSprite, 
                new Vector2(0.5f, 0.85f), new Vector2(18, 9));
        }
        
        if (backsoundSettingSprite != null)
        {
            CreateImage("MusicIcon", settingsBar.transform, backsoundSettingSprite, 
                new Vector2(0.18f, 0.6f), new Vector2(9, 9));
        }
        
        CreateSlider("MusicSlider", settingsBar.transform, soundBarIntensitySprite, 
            new Vector2(0.6f, 0.6f), new Vector2(36, 6));
        
        if (soundEffectSettingSprite != null)
        {
            CreateImage("SFXIcon", settingsBar.transform, soundEffectSettingSprite, 
                new Vector2(0.18f, 0.45f), new Vector2(9, 9));
        }
        
        CreateSlider("SFXSlider", settingsBar.transform, soundBarIntensitySprite, 
            new Vector2(0.6f, 0.45f), new Vector2(36, 6));
        
        CreateButton("BackButton", settingsBar.transform, backButtonSprite, 
            new Vector2(0.5f, 0.25f), new Vector2(28, 10));
        
        panel.SetActive(false);
        return panel;
    }

    private static GameObject CreateButton(string name, Transform parent, Sprite sprite, Vector2 anchorPos, Vector2 size)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.localPosition = Vector3.zero;
        rect.sizeDelta = size;
        
        Image img = btnObj.AddComponent<Image>();
        if (sprite != null)
        {
            img.sprite = sprite;
        }
        else
        {
            img.color = new Color(0.2f, 0.2f, 0.8f);
        }
        
        btnObj.AddComponent<Button>();
        return btnObj;
    }

    private static GameObject CreateSlider(string name, Transform parent, Sprite backgroundSprite, Vector2 anchorPos, Vector2 size)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);
        
        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchorMin = anchorPos;
        sliderRect.anchorMax = anchorPos;
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.anchoredPosition = Vector2.zero;
        sliderRect.sizeDelta = size;
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        if (backgroundSprite != null)
        {
            bgImg.sprite = backgroundSprite;
        }
        else
        {
            bgImg.color = new Color(0.3f, 0.3f, 0.3f);
        }
        
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = new Vector2(-20, 0);
        
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.2f, 0.8f, 0.2f);
        
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.sizeDelta = new Vector2(-20, 0);
        
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(3, 7);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImg;
        
        return sliderObj;
    }

    private static GameObject CreateImage(string name, Transform parent, Sprite sprite, Vector2 anchorPos, Vector2 size)
    {
        GameObject imgObj = new GameObject(name);
        imgObj.transform.SetParent(parent, false);
        
        RectTransform rect = imgObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.localPosition = Vector3.zero;
        rect.sizeDelta = size;
        
        Image img = imgObj.AddComponent<Image>();
        if (sprite != null) img.sprite = sprite;
        
        return imgObj;
    }

    private static void SetupPauseMenuScript(GameObject pausePanel, GameObject settingsPanel)
    {
        PauseMenu existingPauseMenu = Object.FindObjectOfType<PauseMenu>();
        if (existingPauseMenu == null)
        {
            GameObject pauseManagerObj = new GameObject("PauseManager");
            existingPauseMenu = pauseManagerObj.AddComponent<PauseMenu>();
        }

        var pauseMenuType = typeof(PauseMenu);
        var pausePanelField = pauseMenuType.GetField("pausePanel", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var settingsPanelField = pauseMenuType.GetField("settingsPanel", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var musicSliderField = pauseMenuType.GetField("musicSlider", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var sfxSliderField = pauseMenuType.GetField("sfxSlider", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (pausePanelField != null) pausePanelField.SetValue(existingPauseMenu, pausePanel);
        if (settingsPanelField != null) settingsPanelField.SetValue(existingPauseMenu, settingsPanel);
        
        Slider musicSlider = settingsPanel.transform.Find("SettingsBar/MusicSlider")?.GetComponent<Slider>();
        Slider sfxSlider = settingsPanel.transform.Find("SettingsBar/SFXSlider")?.GetComponent<Slider>();
        
        if (musicSliderField != null && musicSlider != null) musicSliderField.SetValue(existingPauseMenu, musicSlider);
        if (sfxSliderField != null && sfxSlider != null) sfxSliderField.SetValue(existingPauseMenu, sfxSlider);

        ConnectButtonEvents(existingPauseMenu, pausePanel, settingsPanel);
    }

    private static void ConnectButtonEvents(PauseMenu pauseMenu, GameObject pausePanel, GameObject settingsPanel)
    {
        Button resumeBtn = pausePanel.transform.Find("PauseBar/ResumeButton")?.GetComponent<Button>();
        Button restartBtn = pausePanel.transform.Find("PauseBar/RestartButton")?.GetComponent<Button>();

        if (resumeBtn != null) resumeBtn.onClick.AddListener(pauseMenu.ResumeGame);
        if (restartBtn != null) restartBtn.onClick.AddListener(pauseMenu.RestartGame);

        Button backBtn = settingsPanel?.transform.Find("SettingsBar/BackButton")?.GetComponent<Button>();
        if (backBtn != null) backBtn.onClick.AddListener(pauseMenu.CloseSettings);
    }

    private static void SetupAudioManager()
    {
        AudioManager existingAudioManager = Object.FindObjectOfType<AudioManager>();
        if (existingAudioManager == null)
        {
            GameObject audioManagerObj = new GameObject("AudioManager");
            audioManagerObj.AddComponent<AudioManager>();
            Debug.Log("✅ AudioManager created!");
        }
    }
}


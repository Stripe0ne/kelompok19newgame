using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to auto-setup UI elements
/// Just drag sprites to this and it will create the UI for you
/// </summary>
public class UISetupHelper : MonoBehaviour
{
    [Header("Pause UI Sprites")]
    [SerializeField] private Sprite pauseBarSprite;
    [SerializeField] private Sprite pauseLogoSprite;
    [SerializeField] private Sprite resumeButtonSprite;
    [SerializeField] private Sprite restartButtonSprite;
    [SerializeField] private Sprite homeButtonSprite;
    
    [Header("Settings UI Sprites")]
    [SerializeField] private Sprite settingsBarSprite;
    [SerializeField] private Sprite settingLogoSprite;
    [SerializeField] private Sprite backButtonSprite;
    [SerializeField] private Sprite backsoundSettingSprite;
    [SerializeField] private Sprite soundEffectSettingSprite;
    [SerializeField] private Sprite soundBarIntensitySprite;

    [Header("Setup Options")]
    [SerializeField] private bool setupOnStart = false;

    private Canvas canvas;
    private GameObject pausePanel;
    private GameObject settingsPanel;

    private void Start()
    {
        if (setupOnStart)
        {
            SetupUI();
        }
    }

    [ContextMenu("Setup Complete UI")]
    public void SetupUI()
    {
        canvas = FindOrCreateCanvas();
        
        pausePanel = CreatePauseMenu(canvas.transform);
        settingsPanel = CreateSettingsMenu(canvas.transform);
        
        // Setup PauseMenu script
        SetupPauseMenuScript();
        
        Debug.Log("UI Setup Complete! Check your Canvas.");
    }

    private Canvas FindOrCreateCanvas()
    {
        Canvas existingCanvas = FindObjectOfType<Canvas>();
        if (existingCanvas != null)
        {
            return existingCanvas;
        }

        GameObject canvasObj = new GameObject("Canvas");
        Canvas newCanvas = canvasObj.AddComponent<Canvas>();
        newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        return newCanvas;
    }

    private GameObject CreatePauseMenu(Transform parent)
    {
        // Main Pause Panel
        GameObject panel = new GameObject("PausePanel");
        panel.transform.SetParent(parent, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        
        // Background dimming
        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.7f);
        
        // Pause Bar (center container)
        GameObject pauseBar = new GameObject("PauseBar");
        pauseBar.transform.SetParent(panel.transform, false);
        
        RectTransform barRect = pauseBar.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.5f, 0.5f);
        barRect.anchorMax = new Vector2(0.5f, 0.5f);
        barRect.sizeDelta = new Vector2(400, 550);
        
        Image barImage = pauseBar.AddComponent<Image>();
        if (pauseBarSprite != null)
        {
            barImage.sprite = pauseBarSprite;
        }
        
        // Pause Logo
        if (pauseLogoSprite != null)
        {
            CreateImage("PauseLogo", pauseBar.transform, pauseLogoSprite, 
                new Vector2(0.5f, 0.82f), new Vector2(120, 60));
        }
        
        // Resume Button
        CreateButton("ResumeButton", pauseBar.transform, resumeButtonSprite, 
            new Vector2(0.5f, 0.62f), new Vector2(200, 60), "Resume");
        
        // Restart Button
        CreateButton("RestartButton", pauseBar.transform, restartButtonSprite, 
            new Vector2(0.5f, 0.48f), new Vector2(200, 60), "Restart");
        
        // Settings Button
        CreateButton("SettingsButton", pauseBar.transform, settingLogoSprite, 
            new Vector2(0.5f, 0.34f), new Vector2(200, 60), "Settings");
        
        // Home Button
        CreateButton("HomeButton", pauseBar.transform, homeButtonSprite, 
            new Vector2(0.5f, 0.2f), new Vector2(200, 60), "Home");
        
        panel.SetActive(false);
        
        return panel;
    }

    private GameObject CreateSettingsMenu(Transform parent)
    {
        // Main Settings Panel
        GameObject panel = new GameObject("SettingsPanel");
        panel.transform.SetParent(parent, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        
        // Background dimming
        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.7f);
        
        // Settings Bar (center container)
        GameObject settingsBar = new GameObject("SettingsBar");
        settingsBar.transform.SetParent(panel.transform, false);
        
        RectTransform barRect = settingsBar.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.5f, 0.5f);
        barRect.anchorMax = new Vector2(0.5f, 0.5f);
        barRect.sizeDelta = new Vector2(450, 600);
        
        Image barImage = settingsBar.AddComponent<Image>();
        if (settingsBarSprite != null)
        {
            barImage.sprite = settingsBarSprite;
        }
        
        // Settings Logo
        if (settingLogoSprite != null)
        {
            CreateImage("SettingsLogo", settingsBar.transform, settingLogoSprite, 
                new Vector2(0.5f, 0.85f), new Vector2(120, 60));
        }
        
        // Music Icon
        if (backsoundSettingSprite != null)
        {
            CreateImage("MusicIcon", settingsBar.transform, backsoundSettingSprite, 
                new Vector2(0.18f, 0.6f), new Vector2(50, 50));
        }
        
        // Music Slider
        CreateSlider("MusicSlider", settingsBar.transform, soundBarIntensitySprite, 
            new Vector2(0.6f, 0.6f), new Vector2(220, 30));
        
        // SFX Icon
        if (soundEffectSettingSprite != null)
        {
            CreateImage("SFXIcon", settingsBar.transform, soundEffectSettingSprite, 
                new Vector2(0.18f, 0.45f), new Vector2(50, 50));
        }
        
        // SFX Slider
        CreateSlider("SFXSlider", settingsBar.transform, soundBarIntensitySprite, 
            new Vector2(0.6f, 0.45f), new Vector2(220, 30));
        
        // Back Button
        CreateButton("BackButton", settingsBar.transform, backButtonSprite, 
            new Vector2(0.5f, 0.25f), new Vector2(180, 55), "Back");
        
        panel.SetActive(false);
        
        return panel;
    }

    private GameObject CreateButton(string name, Transform parent, Sprite sprite, Vector2 anchorPos, Vector2 size, string label)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
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
        
        Button btn = btnObj.AddComponent<Button>();
        
        // Optional: Add text label
        // GameObject textObj = new GameObject("Text");
        // textObj.transform.SetParent(btnObj.transform, false);
        // TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        // text.text = label;
        // text.alignment = TextAlignmentOptions.Center;
        // text.fontSize = 36;
        
        return btnObj;
    }

    private GameObject CreateSlider(string name, Transform parent, Sprite backgroundSprite, Vector2 anchorPos, Vector2 size)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);
        
        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchorMin = anchorPos;
        sliderRect.anchorMax = anchorPos;
        sliderRect.sizeDelta = size;
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        
        // Background
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
        
        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = new Vector2(-20, 0);
        
        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.2f, 0.8f, 0.2f);
        
        // Handle Slide Area
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.sizeDelta = new Vector2(-20, 0);
        
        // Handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(15, 30);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImg;
        
        return sliderObj;
    }

    private GameObject CreateImage(string name, Transform parent, Sprite sprite, Vector2 anchorPos, Vector2 size)
    {
        GameObject imgObj = new GameObject(name);
        imgObj.transform.SetParent(parent, false);
        
        RectTransform rect = imgObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.sizeDelta = size;
        
        Image img = imgObj.AddComponent<Image>();
        if (sprite != null)
        {
            img.sprite = sprite;
        }
        
        return imgObj;
    }

    private void SetupPauseMenuScript()
    {
        // Check if PauseMenu already exists
        PauseMenu existingPauseMenu = FindObjectOfType<PauseMenu>();
        if (existingPauseMenu == null)
        {
            GameObject pauseManagerObj = new GameObject("PauseManager");
            existingPauseMenu = pauseManagerObj.AddComponent<PauseMenu>();
        }

        // Auto-assign references using reflection
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

        // Connect button events
        ConnectButtonEvents();
        
        Debug.Log("PauseMenu script setup complete!");
    }

    private void ConnectButtonEvents()
    {
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu == null) return;

        // Pause Panel Buttons
        Button resumeBtn = pausePanel.transform.Find("PauseBar/ResumeButton")?.GetComponent<Button>();
        Button restartBtn = pausePanel.transform.Find("PauseBar/RestartButton")?.GetComponent<Button>();
        Button settingsBtn = pausePanel.transform.Find("PauseBar/SettingsButton")?.GetComponent<Button>();
        Button homeBtn = pausePanel.transform.Find("PauseBar/HomeButton")?.GetComponent<Button>();

        if (resumeBtn != null) resumeBtn.onClick.AddListener(pauseMenu.ResumeGame);
        if (restartBtn != null) restartBtn.onClick.AddListener(pauseMenu.RestartGame);
        if (settingsBtn != null) settingsBtn.onClick.AddListener(pauseMenu.OpenSettings);
        if (homeBtn != null) homeBtn.onClick.AddListener(pauseMenu.GoToMainMenu);

        // Settings Panel Buttons
        Button backBtn = settingsPanel.transform.Find("SettingsBar/BackButton")?.GetComponent<Button>();
        if (backBtn != null) backBtn.onClick.AddListener(pauseMenu.CloseSettings);

        Debug.Log("Button events connected!");
    }
}


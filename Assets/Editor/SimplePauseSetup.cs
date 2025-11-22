using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// SUPER SIMPLE - Bikin pause menu cuma 2 button
/// Tools > Simple Pause Setup
/// </summary>
public class SimplePauseSetup : MonoBehaviour
{
    [MenuItem("Tools/Simple Pause Setup")]
    public static void CreateSimplePauseMenu()
    {
        // Load sprites
        Sprite pauseBarSprite = FindSprite("Pause bar");
        Sprite resumeButtonSprite = FindSprite("Resume button");
        Sprite restartButtonSprite = FindSprite("restart button");
        
        // 1. Bikin Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();
        raycaster.ignoreReversedGraphics = true;
        raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        
        // 2. Bikin EventSystem
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // 3. Bikin Pause Panel (background gelap)
        GameObject pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvasObj.transform, false);
        
        RectTransform panelRect = pausePanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        
        Image panelBg = pausePanel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.8f);
        
        // 3.5 Bikin Pause Bar (container dengan sprite)
        GameObject pauseBar = new GameObject("PauseBar");
        pauseBar.transform.SetParent(pausePanel.transform, false);
        
        RectTransform barRect = pauseBar.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.5f, 0.5f);
        barRect.anchorMax = new Vector2(0.5f, 0.5f);
        barRect.sizeDelta = new Vector2(200, 150);
        barRect.anchoredPosition = Vector2.zero;
        
        Image barImg = pauseBar.AddComponent<Image>();
        if (pauseBarSprite != null)
        {
            barImg.sprite = pauseBarSprite;
        }
        else
        {
            barImg.color = new Color(0.6f, 0.4f, 0.2f, 0.9f); // Coklat kalo sprite gak ada
        }
        
        // 4. Bikin Resume Button
        GameObject resumeBtn = new GameObject("ResumeButton");
        resumeBtn.transform.SetParent(pauseBar.transform, false);
        
        RectTransform resumeRect = resumeBtn.AddComponent<RectTransform>();
        resumeRect.anchorMin = new Vector2(0.5f, 0.5f);
        resumeRect.anchorMax = new Vector2(0.5f, 0.5f);
        resumeRect.sizeDelta = new Vector2(120, 40);
        resumeRect.anchoredPosition = new Vector2(0, 25);
        
        Image resumeImg = resumeBtn.AddComponent<Image>();
        resumeImg.raycastTarget = true; // PENTING biar bisa diklik!
        if (resumeButtonSprite != null)
        {
            resumeImg.sprite = resumeButtonSprite;
        }
        else
        {
            resumeImg.color = new Color(0.2f, 0.8f, 0.2f);
            
            // Text kalo gak ada sprite
            GameObject resumeText = new GameObject("Text");
            resumeText.transform.SetParent(resumeBtn.transform, false);
            RectTransform resumeTextRect = resumeText.AddComponent<RectTransform>();
            resumeTextRect.anchorMin = Vector2.zero;
            resumeTextRect.anchorMax = Vector2.one;
            resumeTextRect.sizeDelta = Vector2.zero;
            
            Text resumeTxt = resumeText.AddComponent<Text>();
            resumeTxt.text = "RESUME";
            resumeTxt.alignment = TextAnchor.MiddleCenter;
            resumeTxt.fontSize = 20;
            resumeTxt.color = Color.white;
            resumeTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            resumeTxt.raycastTarget = false; // Text jangan block raycast
        }
        
        Button resumeBtnComponent = resumeBtn.AddComponent<Button>();
        resumeBtnComponent.targetGraphic = resumeImg;
        
        // 5. Bikin Restart Button
        GameObject restartBtn = new GameObject("RestartButton");
        restartBtn.transform.SetParent(pauseBar.transform, false);
        
        RectTransform restartRect = restartBtn.AddComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.5f, 0.5f);
        restartRect.anchorMax = new Vector2(0.5f, 0.5f);
        restartRect.sizeDelta = new Vector2(120, 40);
        restartRect.anchoredPosition = new Vector2(0, -25);
        
        Image restartImg = restartBtn.AddComponent<Image>();
        restartImg.raycastTarget = true; // PENTING biar bisa diklik!
        if (restartButtonSprite != null)
        {
            restartImg.sprite = restartButtonSprite;
        }
        else
        {
            restartImg.color = new Color(0.8f, 0.2f, 0.2f);
            
            // Text kalo gak ada sprite
            GameObject restartText = new GameObject("Text");
            restartText.transform.SetParent(restartBtn.transform, false);
            RectTransform restartTextRect = restartText.AddComponent<RectTransform>();
            restartTextRect.anchorMin = Vector2.zero;
            restartTextRect.anchorMax = Vector2.one;
            restartTextRect.sizeDelta = Vector2.zero;
            
            Text restartTxt = restartText.AddComponent<Text>();
            restartTxt.text = "RESTART";
            restartTxt.alignment = TextAnchor.MiddleCenter;
            restartTxt.fontSize = 20;
            restartTxt.color = Color.white;
            restartTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            restartTxt.raycastTarget = false; // Text jangan block raycast
        }
        
        Button restartBtnComponent = restartBtn.AddComponent<Button>();
        restartBtnComponent.targetGraphic = restartImg;
        
        // 6. Setup PauseMenu Script
        GameObject pauseManager = FindObjectOfType<PauseMenu>()?.gameObject;
        PauseMenu pauseMenuScript;
        
        if (pauseManager == null)
        {
            pauseManager = new GameObject("PauseManager");
            pauseMenuScript = pauseManager.AddComponent<PauseMenu>();
        }
        else
        {
            pauseMenuScript = pauseManager.GetComponent<PauseMenu>();
        }
        
        // Connect via reflection
        var type = typeof(PauseMenu);
        var pausePanelField = type.GetField("pausePanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (pausePanelField != null) pausePanelField.SetValue(pauseMenuScript, pausePanel);
        
        // Connect buttons - PENTING!
        Button resumeButton = resumeBtn.GetComponent<Button>();
        Button restartButton = restartBtn.GetComponent<Button>();
        
        if (resumeButton != null && pauseMenuScript != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(pauseMenuScript.ResumeGame);
            resumeButton.interactable = true;
            Debug.Log("✅ Resume button connected and interactable!");
        }
        else
        {
            Debug.LogError("❌ Resume button or PauseMenu script is NULL!");
        }
        
        if (restartButton != null && pauseMenuScript != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(pauseMenuScript.RestartGame);
            restartButton.interactable = true;
            Debug.Log("✅ Restart button connected and interactable!");
        }
        else
        {
            Debug.LogError("❌ Restart button or PauseMenu script is NULL!");
        }
        
        // 7. Setup AudioManager
        if (FindObjectOfType<AudioManager>() == null)
        {
            GameObject audioManager = new GameObject("AudioManager");
            audioManager.AddComponent<AudioManager>();
        }
        
        // Hide pause panel by default
        pausePanel.SetActive(false);
        
        // Test button functionality
        Debug.Log("✅ Simple Pause Menu Created!");
        Debug.Log($"Canvas: {canvas != null}, Raycaster: {canvasObj.GetComponent<GraphicRaycaster>() != null}");
        Debug.Log($"Resume Button: {resumeButton != null}, Restart Button: {restartButton != null}");
        Debug.Log($"PauseMenu Script: {pauseMenuScript != null}");
        
        EditorUtility.DisplayDialog("Success!", 
            "Pause menu berhasil dibuat!\n\n✅ Resume button\n✅ Restart button\n\nTekan ESC atau P untuk pause!\n\nCek Console untuk debug info.", "OK");
    }
    
    private static Sprite FindSprite(string spriteName)
    {
        string[] guids = AssetDatabase.FindAssets(spriteName + " t:Sprite");
        
        if (guids.Length == 0)
        {
            Debug.LogWarning($"Sprite '{spriteName}' not found! Using default color.");
            return null;
        }
        
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
        
        Sprite sprite = System.Array.Find(sprites, obj => obj is Sprite) as Sprite;
        if (sprite != null)
        {
            Debug.Log($"Found sprite: {sprite.name} at {path}");
        }
        return sprite;
    }
}


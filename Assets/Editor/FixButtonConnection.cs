using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Fix button connection kalo gak jalan
/// Tools > Fix Button Connection
/// </summary>
public class FixButtonConnection : MonoBehaviour
{
    [MenuItem("Tools/Fix Button Connection")]
    public static void FixButtons()
    {
        // Cari PauseManager
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu == null)
        {
            Debug.LogError("❌ PauseMenu script not found!");
            EditorUtility.DisplayDialog("Error", "PauseManager tidak ditemukan!\nPastikan ada GameObject dengan PauseMenu script.", "OK");
            return;
        }
        
        // Cari Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas not found!");
            return;
        }
        
        // Cari buttons
        Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
        Button resumeButton = null;
        Button restartButton = null;
        
        foreach (Button btn in buttons)
        {
            if (btn.name == "ResumeButton") resumeButton = btn;
            if (btn.name == "RestartButton") restartButton = btn;
        }
        
        if (resumeButton == null || restartButton == null)
        {
            Debug.LogError("❌ Buttons not found!");
            EditorUtility.DisplayDialog("Error", "Resume atau Restart button tidak ditemukan!", "OK");
            return;
        }
        
        // Clear old listeners
        resumeButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        
        // Add PERSISTENT listeners (ke-save di scene)
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            resumeButton.onClick, 
            pauseMenu.ResumeGame
        );
        
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            restartButton.onClick, 
            pauseMenu.RestartGame
        );
        
        // Mark as dirty untuk save
        EditorUtility.SetDirty(resumeButton);
        EditorUtility.SetDirty(restartButton);
        EditorUtility.SetDirty(pauseMenu);
        
        Debug.Log("✅ Buttons connected successfully!");
        Debug.Log($"Resume Button: {resumeButton != null}, Restart Button: {restartButton != null}");
        Debug.Log($"Resume Listeners: {resumeButton.onClick.GetPersistentEventCount()}");
        Debug.Log($"Restart Listeners: {restartButton.onClick.GetPersistentEventCount()}");
        
        EditorUtility.DisplayDialog("Success!", 
            "Button connection berhasil diperbaiki!\n\nCoba test sekarang:\n1. Play game\n2. Tekan ESC\n3. Klik button", "OK");
    }
}


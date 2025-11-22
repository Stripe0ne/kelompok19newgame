using UnityEngine;
using UnityEditor;

/// <summary>
/// Init script untuk pastiin tools ke-load
/// Otomatis jalan waktu Unity startup
/// </summary>
[InitializeOnLoad]
public class WeaponToolsInit
{
    static WeaponToolsInit()
    {
        EditorApplication.delayCall += OnEditorReady;
    }
    
    private static void OnEditorReady()
    {
        // Log ketika tools ready
        Debug.Log("========================================");
        Debug.Log("🗡️ WEAPON SETUP TOOLS LOADED!");
        Debug.Log("========================================");
        Debug.Log("📌 Cara pakai:");
        Debug.Log("   1. Menu: Tools → Setup Pedang Player 🗡️");
        Debug.Log("   2. Klik kanan Player → Setup Pedang di GameObject Ini 🗡️");
        Debug.Log("   3. Drag AutoSetupPedang.cs ke Player");
        Debug.Log("========================================");
    }
    
    [MenuItem("Tools/🔄 Refresh Editor Scripts", priority = 100)]
    public static void RefreshEditorScripts()
    {
        AssetDatabase.Refresh();
        Debug.Log("✅ Editor scripts refreshed!");
        Debug.Log("Coba cek menu 'Tools' sekarang!");
    }
    
    [MenuItem("Tools/📖 Help - Cara Setup Pedang", priority = 101)]
    public static void ShowHelp()
    {
        EditorUtility.DisplayDialog(
            "🗡️ Cara Setup Pedang",
            "CARA 1 - PAKAI TOOLS WINDOW:\n" +
            "  • Menu: Tools → Setup Pedang Player 🗡️\n" +
            "  • Klik 'Auto-Find Player'\n" +
            "  • Klik 'SETUP PEDANG SEKARANG!'\n\n" +
            
            "CARA 2 - KLIK KANAN:\n" +
            "  • Klik kanan Player di Hierarchy\n" +
            "  • Pilih: Setup Pedang di GameObject Ini 🗡️\n\n" +
            
            "CARA 3 - DRAG & DROP (PALING MUDAH!):\n" +
            "  • Drag file AutoSetupPedang.cs ke Player\n" +
            "  • Play game\n" +
            "  • DONE!\n\n" +
            
            "File location: Assets/Scripts2D/\n\n" +
            "🎮 Setelah setup, Play game aja!\n" +
            "Pedang otomatis muncul di tangan player!",
            "OK");
    }
}


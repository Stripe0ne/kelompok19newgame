using UnityEngine;
using UnityEditor;

/// <summary>
/// TOOLS OTOMATIS buat setup pedang di player
/// Tinggal klik menu aja bro!
/// Menu: Tools → Setup Pedang Player
/// </summary>
public class WeaponSetupTool : EditorWindow
{
    private GameObject playerObject;
    private Sprite weaponSprite;
    
    [MenuItem("Tools/Setup Pedang Player 🗡️")]
    public static void ShowWindow()
    {
        WeaponSetupTool window = GetWindow<WeaponSetupTool>("Setup Pedang");
        window.minSize = new Vector2(400, 350);
        window.Show();
    }
    
    void OnGUI()
    {
        GUILayout.Space(10);
        
        // Header keren
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = 18;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.normal.textColor = Color.white;
        
        GUI.backgroundColor = new Color(0.2f, 0.3f, 0.5f);
        GUILayout.Box("", GUILayout.Height(60), GUILayout.ExpandWidth(true));
        Rect headerRect = GUILayoutUtility.GetLastRect();
        GUI.Label(headerRect, "⚔️ SETUP PEDANG PLAYER ⚔️", headerStyle);
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Tools ini bakal otomatis setup pedang di player kamu.\n" +
            "Pedang bakal muncul di tangan dan ngikutin arah gerak player!",
            MessageType.Info);
        
        GUILayout.Space(15);
        
        // Player field
        EditorGUILayout.LabelField("1️⃣ Pilih Player GameObject:", EditorStyles.boldLabel);
        playerObject = (GameObject)EditorGUILayout.ObjectField("Player", playerObject, typeof(GameObject), true);
        
        GUILayout.Space(10);
        
        // Weapon sprite field
        EditorGUILayout.LabelField("2️⃣ Sprite Pedang (Opsional):", EditorStyles.boldLabel);
        weaponSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", weaponSprite, typeof(Sprite), false);
        
        EditorGUILayout.HelpBox("Kalo kosong, bakal pake sprite default placeholder.", MessageType.None);
        
        GUILayout.Space(20);
        
        // Big setup button
        GUI.backgroundColor = new Color(0.3f, 1f, 0.3f);
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 14;
        buttonStyle.fontStyle = FontStyle.Bold;
        
        if (GUILayout.Button("🚀 SETUP PEDANG SEKARANG!", buttonStyle, GUILayout.Height(50)))
        {
            SetupWeaponOnPlayer();
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(15);
        
        // Quick action buttons
        EditorGUILayout.LabelField("Quick Actions:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🎯 Auto-Find Player di Scene", GUILayout.Height(30)))
        {
            AutoFindPlayer();
        }
        
        if (GUILayout.Button("🔍 Select Player di Hierarchy", GUILayout.Height(30)))
        {
            if (playerObject != null)
            {
                Selection.activeGameObject = playerObject;
                EditorGUIUtility.PingObject(playerObject);
            }
            else
            {
                EditorUtility.DisplayDialog("Info", "Pilih Player dulu bro!", "OK");
            }
        }
        
        GUILayout.Space(15);
        
        // Info box
        EditorGUILayout.HelpBox(
            "📌 CARA PAKAI:\n\n" +
            "1. Drag Player GameObject ke field 'Player'\n" +
            "   (atau klik 'Auto-Find Player')\n\n" +
            "2. (Opsional) Drag sprite pedang ke field 'Sprite'\n\n" +
            "3. Klik tombol hijau 'SETUP PEDANG SEKARANG!'\n\n" +
            "4. DONE! Play game langsung jalan! ✅",
            MessageType.None);
    }
    
    private void AutoFindPlayer()
    {
        // Cari GameObject dengan Player2D component
        Player2D player = FindObjectOfType<Player2D>();
        
        if (player != null)
        {
            playerObject = player.gameObject;
            EditorUtility.DisplayDialog("Ketemu!", $"✅ Player ditemukan: {playerObject.name}", "OK");
        }
        else
        {
            // Cari GameObject bernama "Player"
            GameObject foundPlayer = GameObject.Find("Player");
            if (foundPlayer != null)
            {
                playerObject = foundPlayer;
                EditorUtility.DisplayDialog("Ketemu!", $"✅ Player ditemukan: {playerObject.name}", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Ga Ketemu", "❌ Player ga ketemu di scene.\n\nCoba cari manual atau pastikan ada GameObject bernama 'Player'.", "OK");
            }
        }
    }
    
    private void SetupWeaponOnPlayer()
    {
        if (playerObject == null)
        {
            EditorUtility.DisplayDialog(
                "Error", 
                "❌ Player GameObject belum dipilih!\n\nKlik 'Auto-Find Player' atau drag Player ke field.",
                "OK");
            return;
        }
        
        // Check Player2D component
        Player2D player2D = playerObject.GetComponent<Player2D>();
        if (player2D == null)
        {
            bool addPlayer = EditorUtility.DisplayDialog(
                "Player2D Not Found",
                $"GameObject '{playerObject.name}' ga punya script Player2D.\n\nMau ditambahin otomatis?",
                "Ya, Tambahin!",
                "Batal");
            
            if (addPlayer)
            {
                player2D = playerObject.AddComponent<Player2D>();
                Debug.Log("✅ Player2D script added!");
            }
            else
            {
                return;
            }
        }
        
        // Check MeleeWeapon2D component
        MeleeWeapon2D meleeWeapon = playerObject.GetComponent<MeleeWeapon2D>();
        
        if (meleeWeapon == null)
        {
            meleeWeapon = playerObject.AddComponent<MeleeWeapon2D>();
            Debug.Log("✅ MeleeWeapon2D added!");
        }
        else
        {
            Debug.Log("⚠️ MeleeWeapon2D udah ada, update settings...");
        }
        
        // Set weapon sprite via SerializedObject
        if (weaponSprite != null)
        {
            SerializedObject serializedWeapon = new SerializedObject(meleeWeapon);
            SerializedProperty spriteProp = serializedWeapon.FindProperty("weaponSpriteAsset");
            
            if (spriteProp != null)
            {
                spriteProp.objectReferenceValue = weaponSprite;
                serializedWeapon.ApplyModifiedProperties();
                Debug.Log($"✅ Weapon sprite assigned: {weaponSprite.name}");
            }
        }
        
        // Mark dirty untuk save
        EditorUtility.SetDirty(playerObject);
        
        // Select player
        Selection.activeGameObject = playerObject;
        
        // Success dialog
        string spriteInfo = weaponSprite != null ? $"Sprite: {weaponSprite.name}" : "Sprite: Default placeholder";
        
        EditorUtility.DisplayDialog(
            "🎉 BERHASIL!",
            $"Pedang berhasil di-setup di '{playerObject.name}'!\n\n" +
            "✅ MeleeWeapon2D component added\n" +
            $"✅ {spriteInfo}\n\n" +
            "🎮 Sekarang Play game aja, pedang langsung muncul!\n\n" +
            "Gerak WASD → Pedang ikut rotasi\n" +
            "Dekat enemy → Auto-attack!",
            "Mantap!");
        
        Debug.Log("========================================");
        Debug.Log("🎉 SETUP PEDANG SELESAI!");
        Debug.Log($"   Player: {playerObject.name}");
        Debug.Log($"   {spriteInfo}");
        Debug.Log("🎮 Langsung PLAY aja bro!");
        Debug.Log("========================================");
    }
}

/// <summary>
/// Context menu untuk setup cepat (klik kanan di GameObject)
/// </summary>
public class WeaponContextMenu
{
    [MenuItem("GameObject/Setup Pedang di GameObject Ini 🗡️", false, 0)]
    public static void SetupWeaponOnSelectedGameObject()
    {
        GameObject selected = Selection.activeGameObject;
        
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "❌ Pilih GameObject dulu di Hierarchy!", "OK");
            return;
        }
        
        // Check Player2D
        Player2D player = selected.GetComponent<Player2D>();
        if (player == null)
        {
            bool add = EditorUtility.DisplayDialog(
                "Warning",
                $"GameObject '{selected.name}' ga punya Player2D script.\n\nMau ditambahin otomatis?",
                "Ya",
                "Batal");
            
            if (!add) return;
            
            player = selected.AddComponent<Player2D>();
            Debug.Log($"✅ Player2D added to {selected.name}");
        }
        
        // Add MeleeWeapon2D
        MeleeWeapon2D weapon = selected.GetComponent<MeleeWeapon2D>();
        if (weapon == null)
        {
            weapon = selected.AddComponent<MeleeWeapon2D>();
            
            EditorUtility.DisplayDialog(
                "Success!",
                $"✅ Pedang berhasil ditambahkan ke '{selected.name}'!\n\n" +
                "🎮 Play game untuk test!\n\n" +
                "Assign sprite pedang di Inspector > MeleeWeapon2D\n" +
                "untuk custom sprite (opsional).",
                "OK");
            
            Debug.Log($"✅ MeleeWeapon2D added to {selected.name}");
        }
        else
        {
            EditorUtility.DisplayDialog("Info", "⚠️ MeleeWeapon2D udah ada di GameObject ini!", "OK");
        }
        
        EditorUtility.SetDirty(selected);
    }
}


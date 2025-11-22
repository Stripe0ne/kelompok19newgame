using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Inspector untuk MeleeWeapon2D
/// Bikin Inspector lebih user-friendly
/// </summary>
[CustomEditor(typeof(MeleeWeapon2D))]
public class MeleeWeaponEditor : Editor
{
    private SerializedProperty damage;
    private SerializedProperty attackRange;
    private SerializedProperty attackCooldown;
    private SerializedProperty attackArc;
    private SerializedProperty swingSpeed;
    private SerializedProperty weaponVisual;
    private SerializedProperty weaponSprite;
    private SerializedProperty weaponSpriteAsset;
    private SerializedProperty weaponOffset;
    private SerializedProperty weaponPivotOffset;
    
    private void OnEnable()
    {
        // Get all serialized properties
        damage = serializedObject.FindProperty("damage");
        attackRange = serializedObject.FindProperty("attackRange");
        attackCooldown = serializedObject.FindProperty("attackCooldown");
        attackArc = serializedObject.FindProperty("attackArc");
        swingSpeed = serializedObject.FindProperty("swingSpeed");
        weaponVisual = serializedObject.FindProperty("weaponVisual");
        weaponSprite = serializedObject.FindProperty("weaponSprite");
        weaponSpriteAsset = serializedObject.FindProperty("weaponSpriteAsset");
        weaponOffset = serializedObject.FindProperty("weaponOffset");
        weaponPivotOffset = serializedObject.FindProperty("weaponPivotOffset");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        MeleeWeapon2D weapon = (MeleeWeapon2D)target;
        
        // Header dengan background
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = 16;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.normal.textColor = Color.white;
        
        GUI.backgroundColor = new Color(0.8f, 0.3f, 0.2f);
        GUILayout.Box("", GUILayout.Height(50), GUILayout.ExpandWidth(true));
        Rect headerRect = GUILayoutUtility.GetLastRect();
        GUI.Label(headerRect, "⚔️ MELEE WEAPON (PEDANG) ⚔️", headerStyle);
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(10);
        
        // Info box
        EditorGUILayout.HelpBox(
            "Pedang ini otomatis nyerang enemy dalam range.\n" +
            "Rotasi pedang ngikutin arah player (WASD).",
            MessageType.Info);
        
        GUILayout.Space(10);
        
        // Visual Section
        EditorGUILayout.LabelField("🎨 VISUAL SETTINGS", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        
        EditorGUILayout.PropertyField(weaponSpriteAsset, new GUIContent("Sprite Pedang", "Drag sprite pedang ke sini"));
        EditorGUILayout.PropertyField(weaponOffset, new GUIContent("Weapon Offset", "Jarak dari center player"));
        EditorGUILayout.PropertyField(weaponPivotOffset, new GUIContent("Pivot Offset", "Titik putar pedang"));
        
        if (weaponVisual != null)
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(weaponVisual, new GUIContent("Visual Object (Auto)", "Created automatically"));
            GUI.enabled = true;
        }
        
        EditorGUI.indentLevel--;
        
        GUILayout.Space(10);
        
        // Combat Stats Section
        EditorGUILayout.LabelField("⚔️ COMBAT STATS", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        
        EditorGUILayout.PropertyField(damage, new GUIContent("Damage", "Damage per hit"));
        EditorGUILayout.PropertyField(attackRange, new GUIContent("Attack Range", "Jangkauan serangan"));
        EditorGUILayout.PropertyField(attackCooldown, new GUIContent("Attack Cooldown", "Delay antar serangan (detik)"));
        EditorGUILayout.PropertyField(attackArc, new GUIContent("Attack Arc", "Sudut serangan (degrees)"));
        EditorGUILayout.PropertyField(swingSpeed, new GUIContent("Swing Speed", "Durasi animasi swing"));
        
        EditorGUI.indentLevel--;
        
        GUILayout.Space(10);
        
        // Stats display saat play mode
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("🎮 GAME RUNNING - Weapon Active!", MessageType.None);
            
            EditorGUILayout.LabelField("Current Stats:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"  Damage: {damage.floatValue}");
            EditorGUILayout.LabelField($"  Range: {attackRange.floatValue}");
            EditorGUILayout.LabelField($"  Cooldown: {attackCooldown.floatValue}s");
        }
        else
        {
            // Test button saat edit mode
            GUI.backgroundColor = new Color(0.3f, 1f, 0.3f);
            if (GUILayout.Button("▶ PLAY GAME TO TEST", GUILayout.Height(35)))
            {
                if (EditorApplication.isPlaying == false)
                {
                    EditorApplication.isPlaying = true;
                }
            }
            GUI.backgroundColor = Color.white;
        }
        
        GUILayout.Space(10);
        
        // Tips section
        EditorGUILayout.HelpBox(
            "📌 TIPS:\n\n" +
            "• Sprite Pedang: Drag sprite ke field atas (facing kanan)\n" +
            "• Weapon Offset: Jarak dari center player (default: 0.5)\n" +
            "• Damage: Makin tinggi makin sakit (default: 30)\n" +
            "• Attack Range: Lingkaran merah saat select player di scene\n" +
            "• Attack Cooldown: Makin kecil makin cepat nyerang\n" +
            "• Attack Arc: 90° = depan aja, 180° = setengah lingkaran\n" +
            "• Swing Speed: Durasi animasi ayunan (0.15s = cepat)",
            MessageType.None);
        
        serializedObject.ApplyModifiedProperties();
    }
}


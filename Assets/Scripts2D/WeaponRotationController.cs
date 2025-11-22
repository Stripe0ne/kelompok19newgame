using UnityEngine;

/// <summary>
/// Controller untuk rotasi pedang
/// Attach ke Player untuk customize cara pedang rotasi
/// </summary>
public class WeaponRotationController : MonoBehaviour
{
    [Header("⚙️ ROTATION SETTINGS")]
    [Tooltip("Cara rotasi pedang")]
    public RotationMode rotationMode = RotationMode.FollowMovement;
    
    [Header("Follow Movement Settings")]
    [Tooltip("Smooth rotation speed (higher = faster)")]
    [Range(1f, 50f)]
    public float rotationSpeed = 20f;
    
    [Tooltip("Pedang selalu update posisi atau fixed offset?")]
    public bool dynamicPosition = true;
    
    [Header("Follow Mouse Settings")]
    [Tooltip("Mouse sensitivity untuk rotasi")]
    [Range(0.1f, 5f)]
    public float mouseSensitivity = 1f;
    
    [Header("Manual Control")]
    [Tooltip("Manual rotation angle (untuk Fixed mode)")]
    [Range(0f, 360f)]
    public float manualAngle = 0f;
    
    [Header("Debug")]
    public bool showDebugRays = true;
    
    private Transform weaponVisual;
    private Player2D player;
    private MeleeWeapon2D meleeWeapon;
    private Vector2 lastDirection = Vector2.right;
    
    public enum RotationMode
    {
        FollowMovement,     // Ikut arah WASD (default)
        FollowMouse,        // Ikut posisi mouse
        Fixed,              // Fixed angle
        Disabled            // Ga rotasi
    }
    
    void Start()
    {
        player = GetComponent<Player2D>();
        meleeWeapon = GetComponent<MeleeWeapon2D>();
        
        // Find weapon visual
        FindWeaponVisual();
        
        if (weaponVisual == null)
        {
            Debug.LogWarning("⚠️ WeaponVisual not found! Setup pedang dulu!");
        }
        else
        {
            Debug.Log($"✅ WeaponRotationController active! Mode: {rotationMode}");
        }
    }
    
    void Update()
    {
        if (weaponVisual == null)
        {
            // Try find again
            FindWeaponVisual();
            return;
        }
        
        // Update rotation based on mode
        switch (rotationMode)
        {
            case RotationMode.FollowMovement:
                RotateFollowMovement();
                break;
                
            case RotationMode.FollowMouse:
                RotateFollowMouse();
                break;
                
            case RotationMode.Fixed:
                RotateFixed();
                break;
                
            case RotationMode.Disabled:
                // Do nothing
                break;
        }
    }
    
    private void FindWeaponVisual()
    {
        // Try find WeaponVisual child
        Transform found = transform.Find("WeaponVisual");
        if (found == null)
        {
            found = transform.Find("WeaponVisual_FORCED");
        }
        
        if (found != null)
        {
            weaponVisual = found;
        }
    }
    
    private void RotateFollowMovement()
    {
        if (player == null) return;
        
        // Get movement direction from player
        Vector2 moveDir = player.GetLastMoveDirection();
        
        if (moveDir.magnitude > 0.1f)
        {
            lastDirection = moveDir.normalized;
        }
        
        // Calculate target angle
        float targetAngle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
        
        // Smooth rotation
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        weaponVisual.rotation = Quaternion.Lerp(
            weaponVisual.rotation, 
            targetRotation, 
            Time.deltaTime * rotationSpeed
        );
        
        // Update position if dynamic
        if (dynamicPosition)
        {
            float distance = weaponVisual.localPosition.magnitude;
            Vector2 newPos = lastDirection * distance;
            weaponVisual.localPosition = Vector2.Lerp(
                weaponVisual.localPosition,
                newPos,
                Time.deltaTime * rotationSpeed
            );
        }
        
        // Flip sprite if moving left
        SpriteRenderer sr = weaponVisual.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipY = lastDirection.x < -0.1f;
        }
    }
    
    private void RotateFollowMouse()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        
        // Direction from player to mouse
        Vector2 direction = (mousePos - transform.position).normalized;
        
        // Calculate angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Smooth rotation
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        weaponVisual.rotation = Quaternion.Lerp(
            weaponVisual.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed * mouseSensitivity
        );
        
        // Update position
        if (dynamicPosition)
        {
            float distance = weaponVisual.localPosition.magnitude;
            weaponVisual.localPosition = direction * distance;
        }
        
        // Flip sprite
        SpriteRenderer sr = weaponVisual.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipY = direction.x < -0.1f;
        }
        
        lastDirection = direction;
    }
    
    private void RotateFixed()
    {
        // Set fixed angle
        weaponVisual.rotation = Quaternion.Euler(0, 0, manualAngle);
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugRays || weaponVisual == null) return;
        
        // Draw direction ray
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, lastDirection * 2f);
        
        // Draw weapon to player line
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, weaponVisual.position);
    }
    
    // Public methods untuk customize dari luar
    public void SetRotationMode(RotationMode mode)
    {
        rotationMode = mode;
        Debug.Log($"Rotation mode changed to: {mode}");
    }
    
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = Mathf.Clamp(speed, 1f, 50f);
    }
    
    public void SetManualAngle(float angle)
    {
        manualAngle = angle;
        if (rotationMode == RotationMode.Fixed)
        {
            RotateFixed();
        }
    }
}


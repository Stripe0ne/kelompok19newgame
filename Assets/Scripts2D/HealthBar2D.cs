using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Health bar component for 2D enemies and tower
/// </summary>
public class HealthBar2D : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private Vector3 offset = new Vector3(0, 0.5f, 0);
    
    private Camera mainCamera;
    private Transform parentTransform;
    private float maxHealth;

    private void Start()
    {
        mainCamera = Camera.main;
        parentTransform = transform.parent;
        
        // Setup health gradient if not assigned
        if (healthGradient == null)
        {
            healthGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.red, 0f);
            colorKeys[1] = new GradientColorKey(Color.yellow, 0.5f);
            colorKeys[2] = new GradientColorKey(Color.green, 1f);
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(1f, 1f);
            
            healthGradient.SetKeys(colorKeys, alphaKeys);
        }
    }

    private void LateUpdate()
    {
        // Follow parent object (enemy or tower)
        if (parentTransform != null)
        {
            transform.position = parentTransform.position + offset;
        }
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        if (healthSlider != null)
        {
            healthSlider.maxValue = health;
            healthSlider.value = health;
        }
        
        UpdateHealthColor();
    }

    public void SetHealth(float health)
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
        
        UpdateHealthColor();
    }

    private void UpdateHealthColor()
    {
        if (fillImage != null && healthSlider != null && maxHealth > 0)
        {
            float normalizedHealth = healthSlider.value / maxHealth;
            fillImage.color = healthGradient.Evaluate(normalizedHealth);
        }
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}
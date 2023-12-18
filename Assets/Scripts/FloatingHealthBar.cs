using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 offset;

    private PlayerHealthSystem healthSystem;
    private Slider healthBarSlider;
    private Gradient gradient;
    private void Awake()
    {
        SetGradient();
        healthSystem = GetComponentInParent<PlayerHealthSystem>();
        healthBarSlider = GetComponent<Slider>();

        healthSystem.GetCurrentHealth().OnValueChanged += (float previousHealth, float newHealth) =>
        {
            UpdateHealthBar();
        };
        healthSystem.GetMaxHealth().OnValueChanged += (float previousHealth, float newHealth) =>
        {
            UpdateHealthBar();
        };

    }

    private void Start()
    {
    }
    private void OnEnable()
    {
       
        UpdateHealthBar(); // Player connects with full hp
    }
    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        transform.position = targetTransform.position + offset;
    }

    public void UpdateHealthBar()
    {
        float maxHealth = healthSystem.GetMaxHealth().Value;
        if (maxHealth != 0)
        {
            float currentHealth = healthSystem.GetCurrentHealth().Value;
            healthBarSlider.value = currentHealth / maxHealth;
            fillImage.color = gradient.Evaluate(currentHealth / maxHealth);
        }

         
    }

    private void SetGradient()
    {
        gradient = new Gradient();

        // Blend color from red at 0% to green at 100%
        GradientColorKey[] colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(Color.red, 0.0f);
        colors[1] = new GradientColorKey(Color.green, 1.0f);

        GradientAlphaKey[] alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 0.0f);
        gradient.SetKeys(colors, alphas);
    }

}

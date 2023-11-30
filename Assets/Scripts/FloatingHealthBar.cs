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



    }

    private void Start()
    {
    }
    private void OnEnable()
    {
        healthSystem.GetCurrentHealth().OnValueChanged += (float previousHealth, float newHealth) =>
        {
            UpdateHealthBar(newHealth);
        };
        UpdateHealthBar(healthSystem.GetMaxHealth()); // Player connects with full hp
    }
    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        transform.position = targetTransform.position + offset;
    }

    public void UpdateHealthBar(float newHealth)
    {
        float maxHealth = healthSystem.GetMaxHealth();
        healthBarSlider.value = newHealth / maxHealth;
        fillImage.color = gradient.Evaluate(newHealth / maxHealth);
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

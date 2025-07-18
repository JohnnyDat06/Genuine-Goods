using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider slider;
    [SerializeField] private float smoothSpeed = 5f;

    private float displayedHealth;

    private void Start()
    {
        slider.maxValue = playerHealth.startHealth;
        displayedHealth = playerHealth.currentHeath;
        slider.value = displayedHealth;
    }

    private void Update()
    {
        displayedHealth = Mathf.Lerp(displayedHealth, playerHealth.currentHeath, Time.deltaTime * smoothSpeed);
        slider.value = displayedHealth;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarUI : MonoBehaviour
{
    public Image healthImage;
    public HealthComponent playerHealth;

    private void Start()
    {
        playerHealth.on_health_changed.AddListener(UpdateHealthBar);
    }

    private void UpdateHealthBar(int currentHealth)
    {
        healthImage.fillAmount = currentHealth / playerHealth.max_health;
    }
}

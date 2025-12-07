using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image healthImage;
    public HealthComponent playerHealth;

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.on_health_changed.AddListener(UpdateHealthBar);
            UpdateHealthBar(playerHealth.health);
        }
    }

    private void UpdateHealthBar(int currentHealth)
    {
        healthImage.fillAmount = currentHealth / playerHealth.max_health;
    }
}

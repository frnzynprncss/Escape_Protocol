using UnityEngine;
using UnityEngine.UI;
// Removed UIElements to prevent conflicts with standard UI

public class HealthBarScript : MonoBehaviour
{
    public HealthComponent character;
    public Slider health_slider;

    private void Start()
    {
        if (character != null)
        {
            health_slider.maxValue = character.max_health;
            health_slider.value = character.health;

            character.on_health_changed.AddListener(update_health);

            // Initial update
            update_health(character.health);
        }
    }

    public void update_health(int health)
    {
        health_slider.value = health;
    }
}
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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

            update_health(character.health);
        }
    }

    public void update_health(int health)
    {
        DOTween.To(() => health_slider.value, x => health_slider.value = x, health, 0.3f)
        .SetEase(Ease.OutCubic)
        .SetLink(gameObject);
    }
}

using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    public UnityEvent on_death;
    public UnityEvent<AttackComponent> on_damage_recieved;
    public UnityEvent<int> on_health_changed;

    public int max_health;
    public int health { get; private set; }

    private void Awake()
    {
        health = max_health;
    }

    public void take_damage(AttackComponent attack)
    {
        if (attack == null)
        {
            Debug.LogWarning($"[{gameObject.transform.parent.name} Health Component] No AttackComponent Provided!");
            return;
        }
        if (health <= 0 ) return;

        health -= attack.attack_damage;
        health = Math.Max(health, 0);

        on_health_changed.Invoke(health);
        on_damage_recieved.Invoke(attack);

        if (health <= 0)
        {
            on_death.Invoke();
        }
    }

    public void heal(int heal_power)
    {
        health += heal_power;
        health = Math.Min(health, max_health);
        on_health_changed.Invoke(health);
    }

    public void kill()
    {
        Destroy(gameObject);
    }
}

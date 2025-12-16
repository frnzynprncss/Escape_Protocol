using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{
    public UnityEvent on_death;
    public UnityEvent<AttackComponent> on_damage_recieved;
    public UnityEvent<int> on_health_changed;

    [Header("UI Settings")]
    [Tooltip("If true, this component will search for the Player's HUD. Keep false for Enemies.")]
    public bool isPlayer = false;
    public Image health_bar_fill;

    [Header("Blood Effect Settings")]
    [Tooltip("Optional: assign a ParticleSystem prefab to spawn when taking damage.")]
    public ParticleSystem bloodEffectPrefab;
    [Tooltip("Offset from the object position to spawn blood.")]
    public Vector3 bloodSpawnOffset = Vector3.zero;

    public int max_health;
    public int health { get; private set; }

    [Header("Multiplayer Settings")]
    public int playerID = 1;

    private void Awake()
    {
        health = max_health;

        if (isPlayer && health_bar_fill == null)
        {
            string targetName = "HealthBar_P" + playerID;
            GameObject uiObj = GameObject.Find(targetName);

            if (uiObj != null)
                health_bar_fill = uiObj.GetComponent<Image>();
            else
                Debug.LogError($"Could not find a health bar named '{targetName}' for Player {playerID}!");
        }

        update_ui();
    }

    public void take_damage(AttackComponent attack)
    {
        if (attack == null) return;
        if (health <= 0) return;

        health -= attack.attack_damage;
        health = Math.Max(health, 0);

        // 🔥 Spawn blood particle
        spawn_blood();

        update_ui();
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
        update_ui();
        on_health_changed.Invoke(health);
    }

    public void Revive()
    {
        health = max_health;
        update_ui();
        on_health_changed.Invoke(health);
    }

    public void kill()
    {
        Destroy(gameObject);
    }

    private void update_ui()
    {
        if (health_bar_fill != null)
        {
            health_bar_fill.fillAmount = (float)health / max_health;
        }
    }

    private void spawn_blood()
    {
        if (bloodEffectPrefab != null)
        {
            ParticleSystem blood = Instantiate(bloodEffectPrefab, transform.position + bloodSpawnOffset, Quaternion.identity);
            blood.Play();
            Destroy(blood.gameObject, blood.main.duration + blood.main.startLifetime.constantMax);
        }
    }
}

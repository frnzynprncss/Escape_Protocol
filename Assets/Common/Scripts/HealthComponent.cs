using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // --- NEW: Required for UI Image ---

public class HealthComponent : MonoBehaviour
{
    public UnityEvent on_death;
    public UnityEvent<AttackComponent> on_damage_recieved;
    public UnityEvent<int> on_health_changed;

    [Header("UI Settings")] // --- NEW ---
    public Image HealthBar_Fill; // --- NEW: Drag your Green Fill Image here ---

    public int max_health;
    public int health { get; private set; }

    [Header("Multiplayer Settings")]
    public int playerID = 1; // Set this to 1 for Player 1, and 2 for Player 2

    private void Awake()
    {
        health = max_health;

        // --- NEW AUTO-LINKING LOGIC ---
        if (HealthBar_Fill == null)
        {
            // We verify which player this is, and look for the specific bar name
            string targetName = "HealthBar_P" + playerID; // Becomes "HealthBar_P1" or "HealthBar_P2"

            GameObject uiObj = GameObject.Find(targetName);

            if (uiObj != null)
            {
                HealthBar_Fill = uiObj.GetComponent<Image>();
            }
            else
            {
                Debug.LogError($"Could not find a health bar named '{targetName}' for Player {playerID}!");
            }
        }
        // -----------------------------

        update_ui();
    }

    public void take_damage(AttackComponent attack)
    {
        if (attack == null)
        {
            Debug.LogWarning($"[{gameObject.transform.parent.name} Health Component] No AttackComponent Provided!");
            return;
        }
        if (health <= 0) return;

        health -= attack.attack_damage;
        health = Math.Max(health, 0);

        update_ui(); // --- NEW: Update UI when damaged ---

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

        update_ui(); // --- NEW: Update UI when healed ---

        on_health_changed.Invoke(health);
    }

    public void kill()
    {
        Destroy(gameObject);
    }

    // --- NEW HELPER FUNCTION ---
    private void update_ui()
    {
        // Check if the slot is empty (useful if you use this script on enemies without health bars)
        if (HealthBar_Fill != null)
        {
            // We must cast (float) because 'int / int' results in a whole number (0 or 1)
            // Example: 50 / 100 = 0.5 (Correct) vs 0 (Incorrect)
            HealthBar_Fill.fillAmount = (float)health / max_health;
        }
    }
}
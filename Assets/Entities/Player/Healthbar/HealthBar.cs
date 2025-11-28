using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 100f;
    

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Mob"))
        {
            takeDamage(5f);
        }
    }

    public void takeDamage(float dmg)
    {
        healthAmount -= dmg;
        healthAmount = Mathf.Clamp(healthAmount, 0f, 100f);
        healthBar.fillAmount = healthAmount / 100f;
    }
}

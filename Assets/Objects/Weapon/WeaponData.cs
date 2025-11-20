using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Resources/WeaponData")]
public class WeaponData : ScriptableObject
{
    public Sprite weapon_sprite;
    public int weapon_damage;
    public float weapon_spread;
    public float bullet_amount;
    
    public int max_ammo;
    public int current_ammo { get; private set; }

    public float fire_rate;

    public void resupply()
    {
        current_ammo = max_ammo;
    }
}

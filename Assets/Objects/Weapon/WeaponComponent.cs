using System.Collections;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    public WeaponData weapon_data;
    public BulletScript BulletPrefab;
    public GameObject weapon_pivot;

    [SerializeField] private AttackComponent attack;
    [SerializeField] private PlayerMovement controller;
    [SerializeField] private PlayerControl shoot_input;
    [SerializeField] private SpriteRenderer weapon_sprite;
    [SerializeField] private LayerMask target_layers;

    private bool can_fire = true;
    private bool is_shooting = false;

    private void Start()
    {
        equip_weapon(weapon_data);
    }

    private void Update()
    {
        if (Input.GetKey(shoot_input.shoot)) is_shooting = true;
        else is_shooting = false;

        if (is_shooting) fire();

        rotate_weapon();
    }

    public void equip_weapon(WeaponData weapon)
    {
        weapon_sprite.sprite = weapon.weapon_sprite;
        attack.set_attack(weapon.weapon_damage);
    }

    private void fire()
    {
        if (can_fire == false) return;

        StartCoroutine(weapon_cooldown());
        IEnumerator weapon_cooldown()
        {
            can_fire = false;

            for (int shot = 0; shot < weapon_data.bullet_amount; shot++)
            {
                float spread = Random.Range(-weapon_data.weapon_spread, weapon_data.weapon_spread);
                Quaternion spread_rotation = transform.rotation * Quaternion.Euler(0, 0, spread);

                shoot_bullet_projectile(spread_rotation);
            }

            yield return new WaitForSeconds(weapon_data.fire_rate);
            can_fire = true;
        }
    }

    private void shoot_bullet_projectile(Quaternion spread)
    {
        BulletScript bullet = Instantiate(BulletPrefab, transform.position, transform.rotation);
        bullet.set_attack_component(attack);
        bullet.set_direction(spread);
    }

    private void shoot_bullet_raycast(Quaternion spread)
    {
        Vector3 direction = spread * transform.right;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 999f, target_layers);
        Debug.DrawRay(transform.position, direction, Color.green);

        if (hit.collider == null) return;
        Debug.DrawRay(transform.position, direction, Color.red);

        HealthComponent hit_health = hit.collider.gameObject.GetComponent<HealthComponent>();
        if (hit_health == null) return;

        hit_health.take_damage(attack);
    }

    private void rotate_weapon()
    {
        if (controller.get_input() == Vector2.zero) return;

        float angle_radians = Mathf.Atan2(controller.get_input().y, controller.get_input().x);
        float angle_degrees = angle_radians * Mathf.Rad2Deg;
        
        Quaternion weapon_rotation = Quaternion.Euler(0f, 0f, angle_degrees);
        weapon_pivot.transform.rotation = weapon_rotation;

        if (angle_degrees > 90f || angle_degrees < -90f) weapon_sprite.flipY = true;
        else weapon_sprite.flipY = false;
    }
}

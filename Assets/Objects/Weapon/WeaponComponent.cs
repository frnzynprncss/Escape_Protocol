using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponComponent : MonoBehaviour, IRespawnReset
{
    public WeaponData weapon_data;
    public BulletScript BulletPrefab;

    [SerializeField] private AttackComponent attack;
    [SerializeField] private PlayerControl shoot_input;
    [SerializeField] private SpriteRenderer weapon_sprite;
    [SerializeField] private LayerMask target_layers;

    [Header("Audio")]
    [SerializeField] private AudioSource audio_source;
    [SerializeField] private AudioClip shoot_sfx;

    private bool can_fire = true;
    private bool is_shooting = false;
    private Coroutine fireRoutine;

    private void Start()
    {
        equip_weapon(weapon_data);

        if (audio_source == null)
            audio_source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        is_shooting = Input.GetKey(shoot_input.shoot);

        if (is_shooting)
            fire();
    }

    public void equip_weapon(WeaponData weapon)
    {
        weapon_sprite.sprite = weapon.weapon_sprite;
        attack.set_attack(weapon.weapon_damage);
    }

    private void fire()
    {
        if (!can_fire)
            return;

        if (audio_source != null && shoot_sfx != null)
            audio_source.PlayOneShot(shoot_sfx);

        fireRoutine = StartCoroutine(weapon_cooldown());
    }

    private IEnumerator weapon_cooldown()
    {
        can_fire = false;

        for (int shot = 0; shot < weapon_data.bullet_amount; shot++)
        {
            float spread = Random.Range(
                -weapon_data.weapon_spread,
                weapon_data.weapon_spread
            );

            Quaternion spread_rotation =
                transform.rotation * Quaternion.Euler(0, 0, spread);

            shoot_bullet_projectile(spread_rotation);
        }

        yield return new WaitForSeconds(weapon_data.fire_rate);

        can_fire = true;
        fireRoutine = null;
    }

    private void shoot_bullet_projectile(Quaternion spread)
    {
        BulletScript bullet =
            Instantiate(BulletPrefab, transform.position, transform.rotation);

        bullet.set_attack_component(attack);
        bullet.set_direction(spread);
    }

    private void shoot_bullet_raycast(Quaternion spread)
    {
        Vector3 direction = spread * transform.right;

        RaycastHit2D hit =
            Physics2D.Raycast(transform.position, direction, 999f, target_layers);

        if (hit.collider == null)
            return;

        HealthComponent hit_health =
            hit.collider.GetComponent<HealthComponent>();

        if (hit_health != null)
            hit_health.take_damage(attack);
    }

    // =========================
    // RESPAWN RESET (FIX)
    // =========================
    public void OnRespawn()
    {
        StopAllCoroutines();
        can_fire = true;
        is_shooting = false;
        fireRoutine = null;
    }
}

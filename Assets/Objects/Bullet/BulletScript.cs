using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public string target;
    public float bullet_speed;
    
    public AttackComponent attack { get; private set; }

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void FixedUpdate()
    {
        transform.Translate(transform.right * bullet_speed * Time.deltaTime);
    }

    // These set functions are set on the WeaponComponent Script on Instantiation
    public void set_attack_component(AttackComponent attack_data)
    {
        attack = attack_data;
    }

    public void set_direction(Quaternion dir)
    {
        transform.rotation = dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag(target)) return;

        // REMINDER: Make sure that the Hitbox object is a child of the game object for this to work
        HealthComponent health_comp = collision.gameObject.GetComponent<HealthComponent>();
        if (health_comp != null)
        {
            health_comp.take_damage(attack);
        }
        
        Destroy(gameObject);
    }
}

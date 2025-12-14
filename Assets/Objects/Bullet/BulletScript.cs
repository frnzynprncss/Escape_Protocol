using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public string target;
    public float bullet_speed;
    public Rigidbody2D body;
    public AttackComponent attack;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void FixedUpdate()
    {
        body.velocity = transform.right * bullet_speed * Time.deltaTime;
    }

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
        // Check if the bullet hit a wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.CompareTag(target))
        {
            collision.gameObject.GetComponentInParent<HealthComponent>()?.take_damage(attack);
            Destroy(gameObject);
        }
    }
}
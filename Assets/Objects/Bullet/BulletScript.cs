using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public string target;
    public float bullet_speed;
    
    public Vector3 direction;
    public AttackComponent attack { get; private set; }


    private void FixedUpdate()
    {
        transform.Translate(direction * bullet_speed * Time.deltaTime);
    }

    public void set_attack_component(AttackComponent attack_data)
    {
        attack = attack_data;
    }

    public void set_direction(Vector3 dir)
    {
        direction = (transform.position - dir).normalized;
    }
}

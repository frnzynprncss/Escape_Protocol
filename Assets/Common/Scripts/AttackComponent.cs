using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    public int attack_damage;
    public Vector3 attack_position;

    public AttackComponent set_attack(int value)
    {
        attack_damage = value;
        return this;
    }

    public AttackComponent set_attack_pos(Vector3 value)
    {
        attack_position = value;
        return this;
    }
}
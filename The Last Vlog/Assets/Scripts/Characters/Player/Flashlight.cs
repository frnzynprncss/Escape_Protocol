using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] LayerMask collision_layers;
    [SerializeField] float flashlight_range;
    [SerializeField] float flashlight_angle;

    private void Update()
    {
        emit_flash();
    }

    private void emit_flash()
    {

    }
}

using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] ItemComponent item;
    [SerializeField] int amount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        inventory.add_item(item, amount);
        Destroy(gameObject);
    }
}

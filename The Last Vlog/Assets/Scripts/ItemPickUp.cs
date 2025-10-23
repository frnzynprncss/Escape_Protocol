using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] string item_name;
    [SerializeField] int amount;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        inventory.add_item(item_name, amount);
        Destroy(gameObject);
    }
}

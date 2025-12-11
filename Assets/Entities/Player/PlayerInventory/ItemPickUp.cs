using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] ItemComponent item;
    [SerializeField] int amount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            Debug.Log("Collided with: " + other.name + " (Not player, ignored)");
            return;
        }

        Debug.Log("➡ Player touched item pickup: " + item.item_name + " x" + amount);

        PlayerInventoryHolder holder = other.GetComponent<PlayerInventoryHolder>();
        if (holder == null)
        {
            Debug.LogError(" ERROR: Player has NO PlayerInventoryHolder component!");
            return;
        }

        if (holder.inventory == null)
        {
            Debug.LogError(" ERROR: PlayerInventoryHolder has NO PlayerInventory assigned!");
            return;
        }

        Debug.Log("✔ Item added to inventory: " + item.item_name);
        holder.inventory.add_item(item, amount);

        Debug.Log("🗑 Destroying pickup object...");
        Destroy(gameObject);
    }
}

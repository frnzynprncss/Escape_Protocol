using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Find the inventory on the player that touched the item
        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory != null)
        {
            // Try to add the item. If successful (under the 3-item limit), destroy the object.
            if (inventory.add_item(itemName, 1))
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory full! Item stays on ground.");
            }
        }
    }
}
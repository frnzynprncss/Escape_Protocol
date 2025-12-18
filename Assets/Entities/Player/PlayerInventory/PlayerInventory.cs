using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<string> inventoryItems = new List<string>();

    [Header("Settings")]
    public int maxItems = 3;

    public InventoryUI myUI;

    // Must be 'public bool' to fix the CS0029 error in your image
    public bool add_item(string itemName, int amount)
    {
        if (inventoryItems.Count >= maxItems)
        {
            Debug.LogWarning("Inventory Full!");
            return false; // Tells the collectible NOT to destroy itself
        }

        for (int i = 0; i < amount; i++)
        {
            if (inventoryItems.Count < maxItems)
            {
                inventoryItems.Add(itemName);
            }
        }

        if (myUI != null)
        {
            myUI.playerInventory = this;
            myUI.RefreshAllSlots();
        }

        return true; // Tells the collectible it's okay to destroy itself
    }

    public bool ContainsItem(string itemName)
    {
        return inventoryItems.Contains(itemName);
    }

    public void remove_item(string itemName, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (inventoryItems.Contains(itemName))
                inventoryItems.Remove(itemName);
        }
        if (myUI != null) myUI.RefreshAllSlots();
    }
}
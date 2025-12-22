using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
<<<<<<< HEAD
    public UnityEvent<ItemComponent> item_added;
    public UnityEvent<ItemComponent> item_removed;

    public Dictionary<string, ItemComponent> inventory = new Dictionary<string, ItemComponent>();
=======
    public List<string> inventoryItems = new List<string>();

    [Header("Settings")]
    public int maxItems = 3;
>>>>>>> origin/NoError

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

<<<<<<< HEAD
    public void add_item(ItemComponent item, int amount)
    {
        if (inventory.ContainsKey(item.item_name))
        {
            inventory[item.item_name].amount += amount;
        }
        else
        {
            
            ItemComponent newItem = ScriptableObject.CreateInstance<ItemComponent>();
            newItem.set_name(item.item_name)
                   .set_sprite(item.image)
                   .set_amount(amount);

            inventory.Add(item.item_name, newItem);
        }

        item_added.Invoke(inventory[item.item_name]);
    }

    public void remove_item(string item_name, int amount)
    {
        if (!inventory.ContainsKey(item_name)) return;

        inventory[item_name].amount -= amount;
        item_removed.Invoke(inventory[item_name]);

        if (inventory[item_name].amount <= 0)
        {
            inventory.Remove(item_name);
=======
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
>>>>>>> origin/NoError
        }
        if (myUI != null) myUI.RefreshAllSlots();
    }
<<<<<<< HEAD

    public void clear_items()
    {
        inventory.Clear();
    }
=======
>>>>>>> origin/NoError
}
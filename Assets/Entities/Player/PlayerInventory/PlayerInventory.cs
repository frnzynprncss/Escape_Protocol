using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Inventory", menuName = "Resources/Inventory")]
public class PlayerInventory : ScriptableObject
{
    // Public UnityEvents allow other scripts (like InventoryUI) to subscribe
    public UnityEvent<ItemComponent> item_added = new UnityEvent<ItemComponent>();
    public UnityEvent<ItemComponent> item_removed = new UnityEvent<ItemComponent>();

    // The core data structure for the inventory
    public Dictionary<string, ItemComponent> inventory = new Dictionary<string, ItemComponent>();

    public void print_items()
    {
        foreach (var item in inventory)
        {
            Debug.Log(item.Key + " " + item.Value.amount);
        }
    }

    // Handles adding new items or increasing the stack of an existing item
    public void add_item(ItemComponent item, int amount)
    {
        ItemComponent itemToUpdate;
        
        if (inventory.ContainsKey(item.item_name))
        {
            // Case 1: Item already exists (Stacking)
            itemToUpdate = inventory[item.item_name];
            itemToUpdate.amount += amount;
        }
        else
        {
            // Case 2: New item (Cloning and adding)
            // Create a clone to be owned by this inventory instance
            ItemComponent newItem = ScriptableObject.CreateInstance<ItemComponent>();
            newItem.set_name(item.item_name)
                    .set_sprite(item.image)
                    .set_amount(amount);

            inventory.Add(item.item_name, newItem);
            itemToUpdate = newItem;
        }

        // Notify UI of the change in amount/new item
        item_added.Invoke(itemToUpdate);
    }

    // Handles consuming or removing items
    public void remove_item(string item_name, int amount)
    {
        if (!inventory.ContainsKey(item_name)) return;

        ItemComponent itemToProcess = inventory[item_name];
        itemToProcess.amount -= amount;

        if (itemToProcess.amount <= 0)
        {
            // Case 1: Item is completely depleted (amount <= 0)
            
            // 1. Notify UI to clear the slot
            item_removed.Invoke(itemToProcess);

            // 2. Remove reference from dictionary
            inventory.Remove(item_name);

            // 3. IMPORTANT: Destroy the ScriptableObject instance to prevent memory leak
            Destroy(itemToProcess);
        }
        else
        {
            // Case 2: Item amount decreased, but still exists
            
            // Notify UI to update the displayed amount (using item_added as previously defined)
            item_added.Invoke(itemToProcess);
        }
    }

    public void clear_items()
    {
        // Must destroy all cloned ScriptableObjects before clearing the dictionary
        foreach (var item in inventory.Values)
        {
            Destroy(item);
        }
        inventory.Clear();
    }
    public bool ContainsItem(string item_name)
{
    return inventory.ContainsKey(item_name) && inventory[item_name].amount > 0;
}

// NEW: Check if the inventory contains ANY of the items in a list
public bool ContainsAny(string[] item_names)
{
    foreach (string name in item_names)
    {
        if (ContainsItem(name))
        {
            return true;
        }
    }
    return false;
}
}
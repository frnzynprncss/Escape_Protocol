using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Inventory", menuName = "Resources/Inventory")]
public class PlayerInventory : ScriptableObject
{
    public UnityEvent<ItemComponent> item_added;
    public UnityEvent<ItemComponent> item_removed;

    public Dictionary<string, ItemComponent> inventory = new Dictionary<string, ItemComponent>();

    public void print_items()
    {
        foreach (var item in inventory)
        {
            Debug.Log(item.Key + " " + item.Value.amount);
        }
    }

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
        }
    }

    public void clear_items()
    {
        inventory.Clear();
    }
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Resources/Inventory")]
public class PlayerInventory : ScriptableObject
{
    Dictionary<string, ItemComponent> inventory = new Dictionary<string, ItemComponent>();

    public void print_items()
    {
        foreach(var item in inventory)
        {
            Debug.Log(item.Key + " " + item.Value.amount);
        }
    }

    public void add_item(string item_name, int amount)
    {
        if (inventory.ContainsKey(item_name))
        {
            inventory[item_name].amount += amount;
        }
        else
        {
            inventory.Add(item_name, new ItemComponent().set_amount(1));
        }
    }

    public void remove_item(string item_name, int amount)
    {
        if (!inventory.ContainsKey(item_name)) return;

        inventory[item_name].amount -= amount;
        
        if (inventory[item_name].amount  <= 0)
        {
            inventory.Remove(item_name);
        }
    }

    public void clear_items()
    {
        inventory.Clear();
    }
}

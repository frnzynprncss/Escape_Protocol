using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public PlayerInventory playerInventory;
    public InventorySlot[] slots;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.item_added.AddListener(UpdateInventoryUI); 
            playerInventory.item_removed.AddListener(RemoveItemFromUI); 
        }
    }

    private void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.item_added.RemoveListener(UpdateInventoryUI);
            playerInventory.item_removed.RemoveListener(RemoveItemFromUI);
        }
    }

    private void Start()
    {
        RefreshAllSlots(); 
    }

    public void UpdateInventoryUI(ItemComponent item)
    {
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty() && slot.CanStack(item))
            {
                slot.SetAmount(item.amount); 
                return;
            }
        }

        if (item.amount > 0)
        {
            foreach (var slot in slots)
            {
                if (slot.IsEmpty())
                {
                    ItemComponent clone = ScriptableObject.CreateInstance<ItemComponent>();
                    clone.set_name(item.item_name)
                        .set_sprite(item.image)
                        .set_amount(item.amount);

                    slot.SetItem(clone);
                    return;
                }
            }
        }
    }

    public void RemoveItemFromUI(ItemComponent item)
    {
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty() && slot.GetItemName() == item.item_name) 
            {
                slot.ClearSlot();
                return;
            }
        }
    }

    public void RefreshAllSlots()
    {
        foreach (var slot in slots)
            slot.ClearSlot();

        if (playerInventory == null) return;

        foreach (var invItem in playerInventory.inventory.Values)
            UpdateInventoryUI(invItem);
    }
}
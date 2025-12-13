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
        Instance = this;
    }

    private void OnEnable()
    {
        if (playerInventory != null)
            playerInventory.item_added.AddListener(UpdateInventoryUI);
    }

    private void OnDisable()
    {
        if (playerInventory != null)
            playerInventory.item_added.RemoveListener(UpdateInventoryUI);
    }

    private void Start()
    {
        RefreshAllSlots();
    }

    public void UpdateInventoryUI(ItemComponent item)
    {
        // Try to stack in an existing slot
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty() && slot.CanStack(item))
            {
                slot.SetAmount(item.amount); // Fix: set exact amount
                return;
            }
        }

        // Find an empty slot
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

    public void RefreshAllSlots()
    {
        foreach (var slot in slots)
            slot.ClearSlot(); // placeholder sprite remains visible

        if (playerInventory == null) return;

        foreach (var invItem in playerInventory.inventory.Values)
            UpdateInventoryUI(invItem);
    }
}

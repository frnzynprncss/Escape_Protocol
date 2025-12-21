using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Dependencies")]
    [Tooltip("You MUST drag the Player object here!")]
    public PlayerInventory playerInventory;

    [Tooltip("Drag all your Slot GameObjects here")]
    public InventorySlot[] slots;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // Use IEnumerator Start to wait 1 frame for PlayerInventory to initialize
    private IEnumerator Start()
    {
        // Safety Check
        if (playerInventory == null)
        {
            Debug.LogError("CRITICAL ERROR: 'Player Inventory' is missing in InventoryUI! Drag your Player object to this field.");
            yield break;
        }

        // Wait for other scripts to load
        yield return null;
        RefreshAllSlots();
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

    public void UpdateInventoryUI(ItemComponent item)
    {
        // 1. Try to stack first
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty() && slot.CanStack(item))
            {
                slot.SetAmount(item.amount);
                return;
            }
        }

        // 2. Find empty slot
        foreach (var slot in slots)
        {
            if (slot.IsEmpty())
            {
                // Fix: Pass the item directly, do not create a clone
                slot.SetItem(item);
                return;
            }
        }

        Debug.Log("Inventory Full! Could not display item.");
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
        foreach (var slot in slots) slot.ClearSlot();

        if (playerInventory != null && playerInventory.inventory != null)
        {
            foreach (var invItem in playerInventory.inventory.Values)
                UpdateInventoryUI(invItem);
        }
    }
}
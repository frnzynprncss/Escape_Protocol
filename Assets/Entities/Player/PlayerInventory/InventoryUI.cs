using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ItemSpriteMapping
{
    public string itemName;
    public Sprite itemSprite;
}

public class InventoryUI : MonoBehaviour
{
    // We removed 'static Instance' to allow for two separate player UIs
    public PlayerInventory playerInventory;

    [Header("UI Setup")]
    public GameObject slotPrefab; // Your UI Image prefab
    public Transform container;   // The object with a Grid Layout Group

    [Header("Item Sprites")]
    public List<ItemSpriteMapping> itemDatabase;

    void Update()
    {
<<<<<<< HEAD
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
=======
        RefreshAllSlots();
>>>>>>> origin/NoError
    }

    public void RefreshAllSlots()
    {
<<<<<<< HEAD
        foreach (var slot in slots)
            slot.ClearSlot(); // placeholder sprite remains visible

=======
>>>>>>> origin/NoError
        if (playerInventory == null) return;

        // Clear existing UI icons
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Create a sprite icon for every item name in the inventory
        foreach (string itemName in playerInventory.inventoryItems)
        {
            Sprite foundSprite = GetSpriteByName(itemName);
            if (foundSprite != null)
            {
                GameObject newSlot = Instantiate(slotPrefab, container);

                // Ensure it's scaled correctly and set the image
                newSlot.transform.localScale = Vector3.one;
                newSlot.GetComponent<Image>().sprite = foundSprite;
            }
        }
    }

    private Sprite GetSpriteByName(string name)
    {
        foreach (var mapping in itemDatabase)
        {
            if (mapping.itemName == name) return mapping.itemSprite;
        }
        Debug.LogWarning("No sprite found in database for: " + name);
        return null;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("UI References")]
    public Image icon;
    public TMP_Text amountText;
    public Sprite placeholderSprite; // Assign your empty slot background here

    [Header("Debug / Testing")]
    [Tooltip("Drag an Item from Project Window here to test the UI")]
    [SerializeField] private ItemComponent testItem;

    private ItemComponent _currentItem;

    // This runs immediately when you change something in the Inspector
    private void OnValidate()
    {
        if (icon == null) return;

        // Allows you to preview items by dragging them into 'Test Item'
        if (testItem != null)
        {
            icon.sprite = testItem.image;
            icon.enabled = true;
            if (amountText != null)
                amountText.text = testItem.amount > 1 ? testItem.amount.ToString() : "";
        }
        else
        {
            // Revert to placeholder if test item is removed
            if (placeholderSprite != null) icon.sprite = placeholderSprite;
            if (amountText != null) amountText.text = "";
        }
    }

    private void Awake()
    {
        // If you left a test item in, load it when game starts
        if (testItem != null)
        {
            SetItem(testItem);
        }
        else
        {
            ClearSlot();
        }
    }

    public void SetItem(ItemComponent newItem)
    {
        _currentItem = newItem;

        if (_currentItem != null)
        {
            icon.sprite = _currentItem.image;
            icon.enabled = true;
            amountText.text = _currentItem.amount > 1 ? _currentItem.amount.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        _currentItem = null;
        testItem = null; // Clear debug item

        if (placeholderSprite != null)
        {
            icon.sprite = placeholderSprite;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false; // Hide white square if no placeholder
        }

        if (amountText != null) amountText.text = "";
    }

    public void SetAmount(int amount)
    {
        if (_currentItem == null) return;
        _currentItem.amount = amount;
        if (amountText != null)
            amountText.text = _currentItem.amount > 1 ? _currentItem.amount.ToString() : "";
    }

    public bool IsEmpty() => _currentItem == null;

    public bool CanStack(ItemComponent newItem) =>
        _currentItem != null && _currentItem.item_name == newItem.item_name;

    public string GetItemName() =>
        _currentItem != null ? _currentItem.item_name : string.Empty;
}
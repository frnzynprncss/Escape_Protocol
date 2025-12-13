using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text amountText;

    public Sprite placeholderSprite; // Assign in inspector (empty slot background)

    private ItemComponent item;

    private void Awake()
    {
        if (icon != null && placeholderSprite != null)
            icon.sprite = placeholderSprite; // Always show placeholder at start
    }

    public void SetItem(ItemComponent newItem)
    {
        item = newItem;
        if (item != null)
        {
            icon.sprite = item.image;
            amountText.text = item.amount > 1 ? item.amount.ToString() : "";
        }
        else
        {
            icon.sprite = placeholderSprite; // Show placeholder if no item
            amountText.text = "";
        }
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = placeholderSprite; // Keep image visible
        amountText.text = "";
    }

    public bool IsEmpty() => item == null;

    public bool CanStack(ItemComponent newItem) => item != null && item.item_name == newItem.item_name;

    public void AddAmount(int amount)
    {
        if (item == null) return;
        item.amount += amount;
        amountText.text = item.amount > 1 ? item.amount.ToString() : "";
    }

    public void SetAmount(int amount)
    {
        if (item == null) return;
        item.amount = amount;
        amountText.text = item.amount > 1 ? item.amount.ToString() : "";
    }
}

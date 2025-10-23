using UnityEngine;
using UnityEngine.UI;

public class InventoryScreen : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Image[] item_slot;

    private int item_index;

    private void Start()
    {
        inventory.item_added.AddListener(add_item);
    }

    private void add_item(ItemComponent item)
    {
        item_slot[item_index].sprite = item.image;
        item_slot[item_index].color = Color.white;

        if (item_index >= item_slot.Length - 1) return;
        item_index++;
    }

    private void remove_item(ItemComponent item)
    {
        if (item.amount <= 0)
        {
            item_slot[item_index].sprite = null;
            item_slot[item_index].color = Color.clear;

            item_index--;
        }
    }
}

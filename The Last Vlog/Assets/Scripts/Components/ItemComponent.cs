using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Resources/Item")]
public class ItemComponent : ScriptableObject
{
    public Sprite image;
    public string item_name;
    public int amount;

    public ItemComponent set_name(string name)
    {
        item_name = name;
        return this;
    }

    public ItemComponent set_amount(int value)
    {
        amount = value;
        return this;
    }

    public ItemComponent set_sprite(Sprite sprite)
    {
        image = sprite;
        return this;
    }
}

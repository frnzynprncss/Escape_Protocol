using UnityEngine;

public class PlayerInventoryHolder : MonoBehaviour
{
    // The Generator looks for this specifically
    public PlayerInventory inventory;

    private void Awake() => inventory = GetComponent<PlayerInventory>();
}
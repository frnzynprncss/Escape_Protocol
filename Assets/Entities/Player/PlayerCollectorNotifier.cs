using UnityEngine;

public class PlayerCollectorNotifier : MonoBehaviour
{
    private SpaceShipInteraction managerScript;
    private PlayerInventory playerInventory;
    public bool collected = false;

    private void Start()
    {
        managerScript = FindObjectOfType<SpaceShipInteraction>();
        playerInventory = GetComponent<PlayerInventory>(); // AUTO DETECT
    }

   private void OnTriggerEnter2D(Collider2D other)
{
    if (managerScript == null || playerInventory == null) return;

    if (!other.CompareTag("CollectibleItem")) return;

    managerScript.CollectItemFromNotifier(other.gameObject, playerInventory);
}
}

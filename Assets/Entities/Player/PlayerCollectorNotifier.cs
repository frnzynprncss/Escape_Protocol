using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollectorNotifier : MonoBehaviour
{
    public SpaceShipInteraction managerScript;
    public PlayerInventory playerInventory;

    private void Start()
    {
        managerScript = FindObjectOfType<SpaceShipInteraction>();

        if (managerScript == null)
            Debug.LogError("No SpaceShipInteraction found in the scene!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("CollectibleItem")) return;

        // FIX: call the existing overload with ONE argument
        managerScript.CollectItemFromNotifier(other.gameObject);
    }
}

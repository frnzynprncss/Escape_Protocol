using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollectorNotifier : MonoBehaviour
{
    public SpaceShipInteraction managerScript;
    private void Start()
    {
        // Find the SpaceShipInteraction in the scene automatically
        managerScript = FindObjectOfType<SpaceShipInteraction>();

        if (managerScript == null)
            Debug.LogError("No SpaceShipInteraction found in the scene!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("CollectibleItem")) return;

        managerScript.CollectItemFromNotifier(other.gameObject);
    }
}

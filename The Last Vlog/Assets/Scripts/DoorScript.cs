using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory playerInventory;
    public string requiredItemName = "Crowbar";

    [Header("Settings")]
    public float interactRadius = 2f;
    public KeyCode interactKey = KeyCode.E;

    private Transform player;
    private bool isOpen = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || isOpen) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactRadius && Input.GetKeyDown(interactKey))
        {
            TryOpenDoor();
        }
    }

    void TryOpenDoor()
    {
        if (playerInventory.HasItem(requiredItemName))
        {
            Debug.Log("Door opened using " + requiredItemName + "!");
            playerInventory.remove_item(requiredItemName, 1);
            isOpen = true;
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("You need a " + requiredItemName + " to open this door!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}

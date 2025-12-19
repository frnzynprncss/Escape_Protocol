using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenShip : MonoBehaviour
{
    public PlayerInventory Player_1;
    public PlayerInventory Player_2;

    public GameObject CompleteShip;

    private bool isPlayer1Ready = false;
    private bool isPlayer2Ready = false;

    private bool player1InRange = false;
    private bool player2InRange = false;

    void Start()
    {
        Player_1 = GameObject.Find("Player1").GetComponent<PlayerInventory>();
        Player_2 = GameObject.Find("Player2").GetComponent<PlayerInventory>();

        if (CompleteShip != null)
            CompleteShip.SetActive(false);

        gameObject.SetActive(true);
    }

    void Update()
    {
        // Check if players are in range and pressed their keys
        if (player1InRange && Input.GetKeyDown(KeyCode.E) && Player_1.inventoryItems.Count >= 3)
            isPlayer1Ready = true;

        if (player2InRange && Input.GetKeyDown(KeyCode.Keypad1) && Player_2.inventoryItems.Count >= 3)
            isPlayer2Ready = true;

        // If both players are ready, remove items and replace ship
        if (isPlayer1Ready && isPlayer2Ready)
        {
            foreach (string item in Player_1.inventoryItems) { Player_1.remove_item(item, 1); }
            foreach (string item in Player_2.inventoryItems) { Player_2.remove_item(item, 1); }

            isPlayer1Ready = false;
            isPlayer2Ready = false;

            gameObject.SetActive(false);

            if (CompleteShip != null)
                CompleteShip.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player1")
            player1InRange = true;
        if (collision.gameObject.name == "Player2")
            player2InRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player1")
            player1InRange = false;
        if (collision.gameObject.name == "Player2")
            player2InRange = false;
    }
}

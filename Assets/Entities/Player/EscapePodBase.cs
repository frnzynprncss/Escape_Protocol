using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EscapePodBase : MonoBehaviour
{
   [Header("UI References (World Space Canvas)")]
    [SerializeField] private TextMeshProUGUI fuelText;
    [SerializeField] private TextMeshProUGUI partsText;
    [SerializeField] private GameObject cardCheckmark; // Drag an Image/Icon here

    [Header("Game Requirements")]
    [SerializeField] private int requiredFuel = 3;
    [SerializeField] private int requiredParts = 3;

    private int currentFuel = 0;
    private int currentParts = 0;
    private bool cardInstalled = false;

    private void Start()
    {
        UpdateUI();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
	//need to change the script's inventory name
        TemporaryInventory playerInv = other.GetComponent<TemporaryInventory>();

        if (playerInv == null) return;

        // PLAYER 1 (G Key)
        if (playerInv.IsPlayer1() && Input.GetKeyDown(KeyCode.G))
        {
            TryDepositItems(playerInv);
        }

        // PLAYER 2 (Keypad 1)
        else if (playerInv.IsPlayer2() && Input.GetKeyDown(KeyCode.Keypad1))
        {
            TryDepositItems(playerInv);
        }
    }

    private void TryDepositItems(TemporaryInventory player)
    {
        bool itemDeposited = false;

        // Checks Fuel
        if (player.fuelCount > 0 && currentFuel < requiredFuel)
        {
            // Take 1 fuel at a time, or all at once? Let's take 1 for now.
            player.fuelCount--; 
            currentFuel++;
            itemDeposited = true;
            Debug.Log($"{player.name} deposited Fuel.");
        }

        // Checks Parts
        if (player.spaceshipParts > 0 && currentParts < requiredParts)
        {
            player.spaceshipParts--;
            currentParts++;
            itemDeposited = true;
            Debug.Log($"{player.name} deposited a Part.");
        }

        // Checks Access Card
        if (player.hasAccessCard && !cardInstalled)
        {
            player.hasAccessCard = false; // Remove card from player
            cardInstalled = true;
            itemDeposited = true;
            Debug.Log($"{player.name} inserted the Access Card.");
        }

        if (itemDeposited)
        {
            UpdateUI();
            CheckWinCondition();
        }
    }

    private void UpdateUI()
    {
        // Updates text
        if(fuelText != null) 
            fuelText.text = $"{currentFuel}/{requiredFuel}";
        
        if(partsText != null) 
            partsText.text = $"{currentParts}/{requiredParts}";

        // Show/Hide the card icon
        if(cardCheckmark != null) 
            cardCheckmark.SetActive(cardInstalled);
    }

    private void CheckWinCondition()
    {
        if (currentFuel >= requiredFuel && currentParts >= requiredParts && cardInstalled)
        {
            Debug.Log("ESCAPE PROTOCOL INITIATED - YOU WIN!");
            // Add your Win Scene loading logic here
        }
    }

}

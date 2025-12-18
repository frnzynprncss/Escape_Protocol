using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpaceShipInteraction : MonoBehaviour
{
    // --- NEW REFERENCE ADDED ---
    public GameManager gameManager;
    // ---------------------------

    public PlayerInventory playerInventory;

    public string fuelItemName = "Fuel";
    public int fuelCapacity = 3;

    public string spacePart1Name = "SpacePart1";
    public string spacePart2Name = "SpacePart2";
    public string spacePart3Name = "SpacePart3";

    public string accessCardName = "AccessCard";

    public Collectible fuelDefinition;
    public Collectible spacePart1Definition;
    public Collectible spacePart2Definition;
    public Collectible spacePart3Definition;
    public Collectible accessCardDefinition;

    public GameObject completedSpaceshipVisual;  // Prefab for completed ship

    [HideInInspector] public GameObject partsVisualInstance;       // Assigned by generator
    [HideInInspector] public GameObject completeVisualInstance;    // Assigned by generator

    public TextMeshProUGUI returnIndicatorText;

    private int currentFuel = 0;
    private bool allGoalItemsCollected = false;
    private bool allPartsAttached = false;
    private bool isRepaired = false;

    private void Start()
    {
        if (partsVisualInstance != null)
            partsVisualInstance.SetActive(true);

        if (completeVisualInstance != null)
            completeVisualInstance.SetActive(false);

        if (returnIndicatorText != null)
            returnIndicatorText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("CollectibleItem")) return;
        CollectItemFromNotifier(other.gameObject);
    }

    public void CollectItemFromNotifier(GameObject collectibleObject)
    {
        string objectName = collectibleObject.name.Replace("(Clone)", "");
        Collectible itemToCollect = null;

        if (objectName.Contains("Fuel")) itemToCollect = fuelDefinition;
        else if (objectName.Contains("SpacePart1")) itemToCollect = spacePart1Definition;
        else if (objectName.Contains("SpacePart2")) itemToCollect = spacePart2Definition;
        else if (objectName.Contains("SpacePart3")) itemToCollect = spacePart3Definition;
        else if (objectName.Contains("AccessCard")) itemToCollect = accessCardDefinition;

        if (itemToCollect != null)
            CollectItem(itemToCollect, collectibleObject);
    }

    private void CollectItem(Collectible definition, GameObject collectibleObject)
    {
        playerInventory.add_item(definition.name, 1);
        Destroy(collectibleObject);
        Debug.Log($"Collected 1 x {definition.name}.");
    }

    private void Update()
    {
        // CheckCompletionStatus is used to trigger the "Return" text, not the win state.
        CheckCompletionStatus();

        if (Input.GetKeyDown(KeyCode.E) && !isRepaired)
        {
            // Priority 1: Add Fuel (if missing fuel and has fuel)
            if (currentFuel < fuelCapacity && playerInventory.ContainsItem(fuelItemName))
                AddFuel();
            // Priority 2: Add Parts (if missing parts and has parts)
            else if (!allPartsAttached && (playerInventory.ContainsItem(spacePart1Name) || playerInventory.ContainsItem(spacePart2Name) || playerInventory.ContainsItem(spacePart3Name)))
                AddParts();
            // Priority 3: Final step (only if everything is done and has the access card)
            else if (currentFuel >= fuelCapacity && allPartsAttached && playerInventory.ContainsItem(accessCardName))
            {
                playerInventory.remove_item(accessCardName, 1);
                FinishSpaceship();
            }
            // Priority 4: Give feedback
            else
                CheckStatusFeedback();
        }
    }

    private void CheckCompletionStatus()
    {
        if (allGoalItemsCollected || playerInventory == null) return;

        // Check if player has *all* required items in inventory to signal return
        if (playerInventory.ContainsItem(fuelItemName) && // NOTE: Assumes player needs at least 1 fuel to start return signal
            playerInventory.ContainsItem(spacePart1Name) &&
            playerInventory.ContainsItem(spacePart2Name) &&
            playerInventory.ContainsItem(spacePart3Name) &&
            playerInventory.ContainsItem(accessCardName))
        {
            allGoalItemsCollected = true;
            if (returnIndicatorText != null)
            {
                returnIndicatorText.gameObject.SetActive(true);
                returnIndicatorText.text = "Go back to your spawn point (Spaceship)!";
            }
            Debug.Log("All items collected! Return to the ship to repair.");
        }
    }

    private void AddFuel()
    {
        if (playerInventory.ContainsItem(fuelItemName))
        {
            playerInventory.remove_item(fuelItemName, 1);
            currentFuel++;
            Debug.Log($"Fuel added. Current: {currentFuel}/{fuelCapacity}.");
        }
    }

    private void AddParts()
    {
        bool partUsed = false;

        if (playerInventory.ContainsItem(spacePart1Name))
        {
            playerInventory.remove_item(spacePart1Name, 1);
            Debug.Log("Attached SpacePart1");
            partUsed = true;
        }
        else if (playerInventory.ContainsItem(spacePart2Name))
        {
            playerInventory.remove_item(spacePart2Name, 1);
            Debug.Log("Attached SpacePart2");
            partUsed = true;
        }
        else if (playerInventory.ContainsItem(spacePart3Name))
        {
            playerInventory.remove_item(spacePart3Name, 1);
            Debug.Log("Attached SpacePart3");
            partUsed = true;
        }

        // Check if all parts are now attached (meaning they are NOT in the inventory)
        if (!playerInventory.ContainsItem(spacePart1Name) &&
            !playerInventory.ContainsItem(spacePart2Name) &&
            !playerInventory.ContainsItem(spacePart3Name))
        {
            allPartsAttached = true;
            if (partUsed)
                Debug.Log("All spaceship parts attached!");
        }
    }

    private void CheckStatusFeedback()
    {
        if (!allPartsAttached)
            Debug.Log("Spaceship needs remaining parts before fueling.");
        else if (currentFuel < fuelCapacity)
            Debug.Log($"Spaceship needs {fuelCapacity - currentFuel} more fuel units.");
        else if (!playerInventory.ContainsItem(accessCardName))
            Debug.Log("Spaceship is ready! Final step requires Access Card.");
        else
            Debug.Log("Spaceship is ready for launch. Press 'E' again to initiate!");
    }

    private void FinishSpaceship()
    {
        isRepaired = true;

        if (partsVisualInstance != null)
            partsVisualInstance.SetActive(false);

        if (completeVisualInstance != null)
            completeVisualInstance.SetActive(true);

        if (returnIndicatorText != null)
            returnIndicatorText.gameObject.SetActive(false);

        Debug.Log("Spaceship completed and activated! Calling WinGame...");

        // --- FINAL CALL TO WIN GAME ---
        if (gameManager != null)
        {
            gameManager.WinGame();
        }
        else
        {
            Debug.LogError("GameManager reference is missing in SpaceShipInteraction!");
        }
        // ------------------------------
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpaceShipInteraction : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;

    public PlayerInventory P1;
    public PlayerInventory P2;

    [Header("Spaceship Requirements")]
    public string fuelItemName = "Fuel";
    public int fuelCapacity = 3;

    public string spacePart1Name = "SpacePart1";
    public string spacePart2Name = "SpacePart2";
    public string spacePart3Name = "SpacePart3";

    public string accessCardName = "AccessCard";

    [Header("Collectible Definitions")]
    public Collectible fuelDefinition;
    public Collectible spacePart1Definition;
    public Collectible spacePart2Definition;
    public Collectible spacePart3Definition;
    public Collectible accessCardDefinition;

    [Header("Visuals")]
    public GameObject completedSpaceshipVisual;  // Prefab or visual for completed ship
    [HideInInspector] public GameObject partsVisualInstance;       // Assigned by generator
    [HideInInspector] public GameObject completeVisualInstance;    // Assigned by generator

    [Header("UI")]
    public TextMeshProUGUI returnIndicatorText;

    private int currentFuel = 0;
    private bool allPartsAttached = false;
    private bool isRepaired = false;

    private void Awake()
    {
        // Auto-find references if not assigned
        if (P1 == null) P1 = GameObject.Find("Player1")?.GetComponent<PlayerInventory>();
        if (P2 == null) P2 = GameObject.Find("Player2")?.GetComponent<PlayerInventory>();
        if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
    }

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

        // Determine which player collected it
        PlayerInventory player = other.GetComponentInParent<PlayerInventory>();
        if (player == null) player = P1; // fallback

        CollectItemFromNotifier(other.gameObject, player);
    }

    public void CollectItemFromNotifier(GameObject collectibleObject, PlayerInventory player)
    {
        string objectName = collectibleObject.name.Replace("(Clone)", "");
        Collectible itemToCollect = null;

        if (objectName.Contains("Fuel")) itemToCollect = fuelDefinition;
        else if (objectName.Contains("SpacePart1")) itemToCollect = spacePart1Definition;
        else if (objectName.Contains("SpacePart2")) itemToCollect = spacePart2Definition;
        else if (objectName.Contains("SpacePart3")) itemToCollect = spacePart3Definition;
        else if (objectName.Contains("AccessCard")) itemToCollect = accessCardDefinition;

        if (itemToCollect != null)
            CollectItem(itemToCollect, collectibleObject, player);
    }

    private void CollectItem(Collectible definition, GameObject collectibleObject, PlayerInventory player)
    {
        player.add_item(collectibleObject.name, 1);
        Destroy(collectibleObject);
        Debug.Log($"Collected 1 x {collectibleObject.name} for {player.name}.");
    }

    private void Update()
    {
        if (isRepaired) return;

        // Handle each player's interaction
        HandlePlayerInteraction(P1, KeyCode.E);
        HandlePlayerInteraction(P2, KeyCode.Keypad1);

        // Update return indicator
        CheckReturnIndicator();
    }

    private void HandlePlayerInteraction(PlayerInventory player, KeyCode key)
    {
        if (player == null) return;

        if (Input.GetKeyDown(key))
        {
            // Priority 1: Add Fuel
            if (currentFuel < fuelCapacity && player.ContainsItem(fuelItemName))
                AddFuel(player);
            // Priority 2: Add Parts
            else if (!allPartsAttached && (player.ContainsItem(spacePart1Name) ||
                                           player.ContainsItem(spacePart2Name) ||
                                           player.ContainsItem(spacePart3Name)))
                AddParts(player);
            // Priority 3: Final step
            else if (currentFuel >= fuelCapacity && allPartsAttached && player.ContainsItem(accessCardName))
            {
                player.remove_item(accessCardName, 1);
                FinishSpaceship();
            }
            // Priority 4: Feedback
            else
                CheckStatusFeedback(player);
        }
    }

    private void AddFuel(PlayerInventory player)
    {
        player.remove_item(fuelItemName, 1);
        currentFuel++;
        Debug.Log($"Fuel added by {player.name}. Current fuel: {currentFuel}/{fuelCapacity}.");
    }

    private void AddParts(PlayerInventory player)
    {
        bool partUsed = false;

        if (player.ContainsItem(spacePart1Name))
        {
            player.remove_item(spacePart1Name, 1);
            Debug.Log($"{player.name} attached SpacePart1");
            partUsed = true;
        }
        else if (player.ContainsItem(spacePart2Name))
        {
            player.remove_item(spacePart2Name, 1);
            Debug.Log($"{player.name} attached SpacePart2");
            partUsed = true;
        }
        else if (player.ContainsItem(spacePart3Name))
        {
            player.remove_item(spacePart3Name, 1);
            Debug.Log($"{player.name} attached SpacePart3");
            partUsed = true;
        }

        // Check if all parts attached across both players
        allPartsAttached = !(P1.ContainsItem(spacePart1Name) || P1.ContainsItem(spacePart2Name) || P1.ContainsItem(spacePart3Name) ||
                             P2.ContainsItem(spacePart1Name) || P2.ContainsItem(spacePart2Name) || P2.ContainsItem(spacePart3Name));

        if (allPartsAttached && partUsed)
            Debug.Log("All spaceship parts attached!");
    }

    private void CheckReturnIndicator()
    {
        if (returnIndicatorText == null) return;

        bool allItemsCollected = (currentFuel > 0 || P1.ContainsItem(fuelItemName) || P2.ContainsItem(fuelItemName)) &&
                                 (P1.ContainsItem(spacePart1Name) || P2.ContainsItem(spacePart1Name)) &&
                                 (P1.ContainsItem(spacePart2Name) || P2.ContainsItem(spacePart2Name)) &&
                                 (P1.ContainsItem(spacePart3Name) || P2.ContainsItem(spacePart3Name)) &&
                                 (P1.ContainsItem(accessCardName) || P2.ContainsItem(accessCardName));

        returnIndicatorText.gameObject.SetActive(allItemsCollected);
        if (allItemsCollected)
            returnIndicatorText.text = "Go back to your spawn point (Spaceship)!";
    }

    private void CheckStatusFeedback(PlayerInventory player)
    {
        if (!allPartsAttached)
            Debug.Log($"{player.name}: Spaceship needs remaining parts before fueling.");
        else if (currentFuel < fuelCapacity)
            Debug.Log($"{player.name}: Spaceship needs {fuelCapacity - currentFuel} more fuel units.");
        else if (!player.ContainsItem(accessCardName))
            Debug.Log($"{player.name}: Spaceship is ready! Final step requires Access Card.");
        else
            Debug.Log($"{player.name}: Spaceship is ready for launch. Press key again to initiate!");
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

        if (gameManager != null)
        {
            gameManager.WinGame();
        }
        else
        {
            Debug.LogError("GameManager reference is missing in SpaceShipInteraction!");
        }
    }
}

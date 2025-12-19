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

    [Header("Visuals")]
    public GameObject completedSpaceshipVisual;
    [HideInInspector] public GameObject partsVisualInstance;
    [HideInInspector] public GameObject completeVisualInstance;

    [Header("UI")]
    public TextMeshProUGUI returnIndicatorText;

    private int currentFuel = 0;
    private bool part1Attached = false;
    private bool part2Attached = false;
    private bool part3Attached = false;
    private bool isRepaired = false;

    private bool AllPartsAttached => part1Attached && part2Attached && part3Attached;

    private void Awake()
    {
        if (P1 == null) P1 = GameObject.Find("Player1")?.GetComponent<PlayerInventory>();
        if (P2 == null) P2 = GameObject.Find("Player2")?.GetComponent<PlayerInventory>();
        if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        if (partsVisualInstance != null) partsVisualInstance.SetActive(true);
        if (completeVisualInstance != null) completeVisualInstance.SetActive(false);
        if (returnIndicatorText != null) returnIndicatorText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("CollectibleItem")) return;
        PlayerInventory player = other.GetComponentInParent<PlayerInventory>();
        if (player == null) player = P1;
        CollectItemFromNotifier(other.gameObject, player);
    }

    public void CollectItemFromNotifier(GameObject collectibleObject, PlayerInventory player)
    {
        if (player == null) return;
        Collectible itemToCollect = collectibleObject.GetComponent<Collectible>();
        if (itemToCollect == null) return;
        CollectItem(itemToCollect, collectibleObject, player);
    }

    private void CollectItem(Collectible definition, GameObject collectibleObject, PlayerInventory player)
    {
        if (definition.collected) return;
        bool success = player.add_item(definition.itemName, 1);
        if (success)
        {
            definition.collected = true;
            Destroy(collectibleObject);
            Debug.Log($"{player.name} collected {definition.itemName}");
        }
    }

    private void Update()
    {
        if (isRepaired) return;

        HandlePlayerInteraction(P1, KeyCode.E);
        HandlePlayerInteraction(P2, KeyCode.Keypad1);
        CheckReturnIndicator();
    }

    private void HandlePlayerInteraction(PlayerInventory player, KeyCode key)
    {
        if (player == null) return;

        if (Input.GetKeyDown(key))
        {
            // Try adding parts
            if (!AllPartsAttached && (player.ContainsItem(spacePart1Name) || player.ContainsItem(spacePart2Name) || player.ContainsItem(spacePart3Name)))
            {
                AddParts(player);
            }
            // Add fuel if all parts are attached
            else if (AllPartsAttached && currentFuel < fuelCapacity && player.ContainsItem(fuelItemName))
            {
                AddFuel(player);
            }
            // Use Access Card to finish spaceship
            else if (AllPartsAttached && currentFuel >= fuelCapacity && player.ContainsItem(accessCardName))
            {
                player.remove_item(accessCardName, 1);
                FinishSpaceship();
            }
            else
            {
                CheckStatusFeedback(player);
            }

            // Update UI immediately
            CheckReturnIndicator();
        }
    }

    private void AddFuel(PlayerInventory player)
    {
        player.remove_item(fuelItemName, 1);
        currentFuel++;
        Debug.Log($"Fuel added. Progress: {currentFuel}/{fuelCapacity}");
    }

    private void AddParts(PlayerInventory player)
    {
        Debug.Log($"AddParts called. Inventory: {string.Join(", ", player.inventoryItems)}");

        if (player.ContainsItem(spacePart1Name) && !part1Attached)
        {
            player.remove_item(spacePart1Name, 1);
            part1Attached = true;
            Debug.Log("Part 1 Attached");
        }

        if (player.ContainsItem(spacePart2Name) && !part2Attached)
        {
            player.remove_item(spacePart2Name, 1);
            part2Attached = true;
            Debug.Log("Part 2 Attached");
        }

        if (player.ContainsItem(spacePart3Name) && !part3Attached)
        {
            player.remove_item(spacePart3Name, 1);
            part3Attached = true;
            Debug.Log("Part 3 Attached");
        }

        Debug.Log($"AllPartsAttached: {AllPartsAttached}");
    }

    private void CheckReturnIndicator()
    {
        if (returnIndicatorText == null) return;

        bool hasAll = (P1.ContainsItem(fuelItemName) || P2.ContainsItem(fuelItemName) || currentFuel > 0) &&
                      (P1.ContainsItem(spacePart1Name) || P2.ContainsItem(spacePart1Name) || part1Attached) &&
                      (P1.ContainsItem(spacePart2Name) || P2.ContainsItem(spacePart2Name) || part2Attached) &&
                      (P1.ContainsItem(spacePart3Name) || P2.ContainsItem(spacePart3Name) || part3Attached) &&
                      (P1.ContainsItem(accessCardName) || P2.ContainsItem(accessCardName));

        returnIndicatorText.gameObject.SetActive(hasAll && !isRepaired);
        if (hasAll) returnIndicatorText.text = "All items found! Return to Spaceship!";
    }

    private void CheckStatusFeedback(PlayerInventory player)
    {
        if (!AllPartsAttached)
            Debug.Log("Ship still needs mechanical parts.");
        else if (currentFuel < fuelCapacity)
            Debug.Log($"Ship needs {fuelCapacity - currentFuel} more fuel units.");
        else
            Debug.Log("Insert Access Card to launch!");
    }

    private void FinishSpaceship()
    {
        isRepaired = true;

        if (partsVisualInstance != null) partsVisualInstance.SetActive(false);
        if (completeVisualInstance != null) completeVisualInstance.SetActive(true);
        if (returnIndicatorText != null) returnIndicatorText.gameObject.SetActive(false);

        if (gameManager != null) gameManager.WinGame();

        Debug.Log("Spaceship repaired! Game Won!");
    }
}

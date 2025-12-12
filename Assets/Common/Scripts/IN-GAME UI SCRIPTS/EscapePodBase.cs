using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EscapePodBase : MonoBehaviour
{
    [Header("UI References (World Space Canvas)")]
    [SerializeField] private TextMeshProUGUI fuelText;
    [SerializeField] private TextMeshProUGUI partsText;
    [SerializeField] private GameObject cardCheckmark;

    [Header("Game Requirements")]
    [SerializeField] private int requiredFuel = 3;
    [SerializeField] private int requiredParts = 3;

    // Current Progress
    private int currentFuel = 0;
    private int currentParts = 0;
    private bool cardInstalled = false;

    private void Start()
    {
        UpdateUI();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // CHANGED: We now look for the new script name "DungeonPlayerInventory"
        PlayerBase player_base = other.GetComponent<PlayerBase>();

        if (player_base == null) return;

        // --- PLAYER 1 LOGIC (G Key) ---
        if (player_base.IsPlayer1() && Input.GetKeyDown(KeyCode.G))
        {
            TryDepositItems(player_base);
        }

        // --- PLAYER 2 LOGIC (Keypad 1) ---
        else if (player_base.IsPlayer2() && Input.GetKeyDown(KeyCode.Keypad1))
        {
            TryDepositItems(player_base);
        }
    }

    private void TryDepositItems(PlayerBase player)
    {
        bool itemDeposited = false;

        // 1. Check Fuel
        if (player.fuelCount > 0 && currentFuel < requiredFuel)
        {
            player.fuelCount--;
            currentFuel++;
            itemDeposited = true;
            Debug.Log($"{player.name} deposited Fuel.");
        }

        // 2. Check Parts
        if (player.spaceshipParts > 0 && currentParts < requiredParts)
        {
            player.spaceshipParts--;
            currentParts++;
            itemDeposited = true;
            Debug.Log($"{player.name} deposited a Part.");
        }

        // 3. Check Access Card
        if (player.hasAccessCard && !cardInstalled)
        {
            player.hasAccessCard = false;
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
        if (fuelText != null)
            fuelText.text = $"{currentFuel}/{requiredFuel}";

        if (partsText != null)
            partsText.text = $"{currentParts}/{requiredParts}";

        if (cardCheckmark != null)
            cardCheckmark.SetActive(cardInstalled);
    }

    private void CheckWinCondition()
    {
        if (currentFuel >= requiredFuel && currentParts >= requiredParts && cardInstalled)
        {
            Debug.Log("ESCAPE PROTOCOL INITIATED - YOU WIN!");
            // Add SceneManager.LoadScene("WinScreen"); here later
        }
    }
}
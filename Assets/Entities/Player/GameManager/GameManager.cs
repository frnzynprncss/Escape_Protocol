using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerInventory P1;
    public PlayerInventory P2;

    public TextMeshProUGUI returnIndicatorText;
    public GameObject WinPanel;
    // --- REMOVED: public GameObject LosePanel; ---

    // This list is now only used for initial collection check if needed, 
    // but the main goal check is moved to SpaceShipInteraction.
    // public string[] requiredItemNames = { "Fuel", "Wheel", "EnginePart", "Screen" }; 

    private bool goalAchieved = false;

    private void Start()
    {
        if (P1 == null) P1 = GameObject.Find("Player1")?.GetComponent<PlayerInventory>();
        if (P2 == null) P2 = GameObject.Find("Player2")?.GetComponent<PlayerInventory>();
        
        // Set all UI panels and indicators to inactive at start
        if (WinPanel != null) WinPanel.SetActive(false);
        // --- REMOVED: if (LosePanel != null) LosePanel.SetActive(false); ---
        if (returnIndicatorText != null)
            returnIndicatorText.gameObject.SetActive(false);
    }

    // NOTE: CheckCompletionStatus is no longer called/needed here.

    public void WinGame()
    {
        if (goalAchieved) return;

        goalAchieved = true;

        if (WinPanel != null)
        {
            WinPanel.SetActive(true);
        }

        Debug.Log("Congratulations! You won the game!");

        // Optional: Stop time or disable player movement here
        // Time.timeScale = 0f;
    }
}
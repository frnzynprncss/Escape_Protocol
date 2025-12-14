using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerInventory playerInventory; 
    public TextMeshProUGUI returnIndicatorText;

    // List of all critical items needed for the goal
    public string[] requiredItemNames = { "Fuel", "Wheel", "EnginePart", "Screen" }; 
    
    private bool goalAchieved = false;

    private void Start()
    {
        if (returnIndicatorText != null)
            returnIndicatorText.gameObject.SetActive(false);
    }

    public void CheckCompletionStatus()
    {
        if (goalAchieved || playerInventory == null) return;

        foreach (string requiredItem in requiredItemNames)
        {
            if (!playerInventory.ContainsItem(requiredItem))
            {
                return; // Missing at least one item, exit early
            }
        }

        // All items are collected
        goalAchieved = true;
        if (returnIndicatorText != null)
            returnIndicatorText.gameObject.SetActive(true);

        Debug.Log("Goal achieved! Return to the ship.");
    }
}

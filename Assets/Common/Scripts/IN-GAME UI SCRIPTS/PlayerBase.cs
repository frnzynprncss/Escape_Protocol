using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    // Inventory Data
    public int fuelCount = 0;
    public int spaceshipParts = 0;
    public bool hasAccessCard = false;

    // Helpers to identify the player
    public bool IsPlayer1()
    {
        // Checks if the GameObject name contains "Player1" or "P1"
        return gameObject.name.Contains("Player1") || gameObject.name.Contains("P1");
    }

    public bool IsPlayer2()
    {
        return gameObject.name.Contains("Player2") || gameObject.name.Contains("P2");
    }
}
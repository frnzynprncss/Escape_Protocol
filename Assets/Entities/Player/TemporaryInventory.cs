using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryInventory : MonoBehaviour
{
     public int fuelCount = 0;
    public int spaceshipParts = 0;
    public bool hasAccessCard = false;

    // Helper to check identity based on your Generator's spawning logic
    public bool IsPlayer1() => gameObject.name.Contains("Player1") || gameObject.name.Contains("P1");
    public bool IsPlayer2() => gameObject.name.Contains("Player2") || gameObject.name.Contains("P2");
}

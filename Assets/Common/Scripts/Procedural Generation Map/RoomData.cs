using UnityEngine;
using System.Collections.Generic;

public enum RoomType { Normal, Spawn, Boss, Hidden, Fuel, Ship }

public class Room
{
    public Vector2Int RoomCenterPos { get; set; }
    public HashSet<Vector2Int> FloorTiles { get; set; }
    public RoomType Type { get; set; }
    public BoundsInt Bounds { get; set; } // This was missing before
}
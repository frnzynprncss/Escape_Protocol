/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    //PCG Parameters
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;
    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.8f;

    //Prefabs
    [SerializeField] private GameObject player1Prefab, player2Prefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject fuelItemPrefab;

    //PCG Data
    private Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary
        = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

    private HashSet<Vector2Int> floorPositions, corridorPositions;

    //Gizmos Data
    private List<Color> roomColors = new List<Color>();
    private List<Room> cachedRooms = new List<Room>();
    [SerializeField]
    private bool showRoomGizmo = false, showCorridorsGizmo = false; // FIXED: Case consistency

    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        ClearRoomData();

        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

        // 1. Create Normal Rooms
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        // 2. Find Dead Ends
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        // 3. Create Hidden Rooms at Dead Ends
        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        // 4. Combine floors
        floorPositions.UnionWith(roomPositions);

        // 5. Widen Corridors
        for (int i = 0; i < corridorCount; i++)
        {
            corridors[i] = IncreaseCorridorBrush3by3(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        // Process the data we collected to determine logic
        //List<Room> finalRoomList = ProcessRoomTypes(deadEnds, roomsDictionary);

        cachedRooms = ProcessRoomTypes(deadEnds, roomsDictionary);

        // Visualize or Spawn items based on type
        SpawnObjectsInRooms(cachedRooms);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    //added for spawning
    private void SpawnObjectsInRooms(List<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            // Calculate the center of the room in World Space
            // (Assuming your tilemap grid is 1x1 units)
            Vector2 roomCenterWorld = (Vector2)room.RoomCenterPos + new Vector2(0.5f, 0.5f);

            switch (room.Type)
            {
                case RoomType.Spawn:
                    // Spawn Players slightly offset from each other
                    Instantiate(player1Prefab, roomCenterWorld + Vector2.left, Quaternion.identity);
                    Instantiate(player2Prefab, roomCenterWorld + Vector2.right, Quaternion.identity);
                    Debug.Log("Spawn Room Created at: " + room.RoomCenterPos);
                    break;

                case RoomType.Exit:
                    Instantiate(bossPrefab, roomCenterWorld, Quaternion.identity);
                    // Add exit door logic here later
                    Debug.Log("Boss Room Created at: " + room.RoomCenterPos);
                    break;

                case RoomType.Hidden:
                    Instantiate(fuelItemPrefab, roomCenterWorld, Quaternion.identity);
                    Debug.Log("Hidden Fuel Room Created at: " + room.RoomCenterPos);
                    break;

                case RoomType.Normal:
                    // Spawn random enemies
                    // Randomly decide if this room gets an enemy
                    if (UnityEngine.Random.value > 0.5f)
                    {
                        Instantiate(enemyPrefab, roomCenterWorld, Quaternion.identity);
                    }
                    break;
            }
        }
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (roomFloors.Contains(position) == false)
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighboursCount++;
            }
            if (neighboursCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);

            // FIXED: Arguments were swapped. Definition is (Pos, Floor), you had (Floor, Pos)
            SaveRoomData(roomPosition, roomFloor);

            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }

    private void ClearRoomData()
    {
        roomsDictionary.Clear();
        roomColors.Clear();
        cachedRooms.Clear();
    }

    private void SaveRoomData(Vector2Int roomPosition, HashSet<Vector2Int> roomFloor)
    {
        roomsDictionary[roomPosition] = roomFloor;
        roomColors.Add(UnityEngine.Random.ColorHSV());
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            corridors.Add(corridor); // FIXED: You forgot to Add the new corridor to the list!
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
        corridorPositions = new HashSet<Vector2Int>(floorPositions);

        return corridors; // FIXED: Missing return statement
    }

    public List<Vector2Int> IncreaseCorridorBrush3by3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }

    public List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int previousDirection = Vector2Int.zero;
        for (int i = 1; i < corridor.Count; i++)
        {
            Vector2Int directionFromCell = corridor[i] - corridor[i - 1];
            if (previousDirection != Vector2Int.zero &&
                directionFromCell != previousDirection)
            {
                //handle corner
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                    }
                }
                previousDirection = directionFromCell;
            }
            else
            {
                //Add a single cell in the direction + 90 degrees
                Vector2Int newCorridorTileOffset
                    = GetDirection90From(directionFromCell);

                // FIXED: 'i - 5' will crash the game (IndexOutOfRange). 
                // Changed to 'i - 1' to reference the current tile being processed.
                newCorridor.Add(corridor[i - 1]);
                newCorridor.Add(corridor[i - 1] + newCorridorTileOffset);
            }
        }
        return newCorridor;
    }

    private Vector2Int GetDirection90From(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
            return Vector2Int.right;
        if (direction == Vector2Int.right)
            return Vector2Int.down;
        if (direction == Vector2Int.down)
            return Vector2Int.left;
        if (direction == Vector2Int.left)
            return Vector2Int.up;
        return Vector2Int.zero;
    }

    // FIXED: Added the OnDrawGizmos to visualize the data collected
    private void OnDrawGizmos()
    {
        // 1. Draw Corridors (Magenta)
        if (showCorridorsGizmo && corridorPositions != null)
        {
            Gizmos.color = Color.magenta;
            foreach (var pos in corridorPositions)
            {
                Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0) + new Vector3(0.5f, 0.5f, 0), Vector3.one);
            }
        }

        // 2. Draw Rooms based on Type
        if (showRoomGizmo && cachedRooms != null)
        {
            foreach (Room room in cachedRooms)
            {
                // Pick color based on Room Type
                switch (room.Type)
                {
                    case RoomType.Spawn:
                        Gizmos.color = Color.green; // Spawn is Green
                        break;
                    case RoomType.Exit:
                        Gizmos.color = Color.red;   // Boss is Red
                        break;
                    case RoomType.Hidden:
                        Gizmos.color = Color.blue;  // Hidden/Fuel is Blue
                        break;
                    case RoomType.Normal:
                        Gizmos.color = Color.white; // Normal/Enemy is White
                        break;
                }

                // Draw the individual floor tiles of the room
                foreach (var pos in room.FloorTiles)
                {
                    Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0) + new Vector3(0.5f, 0.5f, 0), Vector3.one);
                }

                // Draw a big sphere at the logical center so you can spot it easily
                Gizmos.DrawSphere(new Vector3(room.RoomCenterPos.x, room.RoomCenterPos.y, 0) + new Vector3(0.5f, 0.5f, 0), 1.5f);
            }
        }
    }

    //added
    private List<Room> ProcessRoomTypes(List<Vector2Int> deadEnds, Dictionary<Vector2Int, HashSet<Vector2Int>> roomData)
    {
        List<Room> processedRooms = new List<Room>();

        // 1. Separate Dead End Rooms (Hidden) from Normal Rooms
        // We look at the dictionary keys. If a key is inside the deadEnds list, it's a Hidden room.
        List<Vector2Int> allRoomCenters = roomData.Keys.ToList();

        // This calculates distance from start (0,0)
        Vector2Int startPoint = Vector2Int.zero;

        // Variables to find Spawn and Boss
        Vector2Int spawnRoomCenter = Vector2Int.zero;
        Vector2Int bossRoomCenter = Vector2Int.zero;
        float minDst = float.MaxValue;
        float maxDst = float.MinValue;

        // First Pass: Find Spawn (Closest) and Boss (Furthest)
        // We only check rooms that are NOT dead ends for Spawn/Boss to ensure they are connected well
        foreach (var roomCenter in allRoomCenters)
        {
            // Skip dead ends for Spawn/Boss logic (optional, but usually better design)
            if (deadEnds.Contains(roomCenter)) continue;

            float dist = Vector2.Distance(roomCenter, startPoint);

            if (dist < minDst)
            {
                minDst = dist;
                spawnRoomCenter = roomCenter;
            }

            if (dist > maxDst)
            {
                maxDst = dist;
                bossRoomCenter = roomCenter;
            }
        }

        // Second Pass: Assign Types
        foreach (var kvp in roomData)
        {
            Room newRoom = new Room();
            newRoom.RoomCenterPos = kvp.Key;
            newRoom.FloorTiles = kvp.Value;

            if (kvp.Key == spawnRoomCenter)
            {
                newRoom.Type = RoomType.Spawn;
            }
            else if (kvp.Key == bossRoomCenter)
            {
                newRoom.Type = RoomType.Exit;
            }
            else if (deadEnds.Contains(kvp.Key))
            {
                newRoom.Type = RoomType.Hidden;
            }
            else
            {
                newRoom.Type = RoomType.Normal; // Enemy Room
            }

            processedRooms.Add(newRoom);
        }

        return processedRooms;
    }
}
*/
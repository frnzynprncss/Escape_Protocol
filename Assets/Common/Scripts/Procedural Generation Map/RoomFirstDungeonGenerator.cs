using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [Header("Room First Parameters")]
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;
    [SerializeField]
    [Range(1, 5)]
    private int corridorWidth = 5;

    [Header("Game Data / Spawning")]
    [SerializeField] private bool showRoomGizmo = false;
    [SerializeField] private bool showCorridorsGizmo = false;

    [Header("Prefabs")]
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject fuelItemPrefab;

    [Header("Spawn Settings")]
    [Range(0, 1)][SerializeField] private float enemySpawnChance = 0.5f; // Adjust how many enemies appear
    [SerializeField] private int exactFuelCount = 3; // Exactly 3 fuels

    // Internal Data
    private List<Room> cachedRooms = new List<Room>();
    private HashSet<Vector2Int> corridorPositions = new HashSet<Vector2Int>();

    // Container to hold spawned objects so we can delete them easily
    private GameObject dungeonContainer;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        // 1. Clean up OLD objects from the scene before generating new ones
        ClearOldGeneratedObjects();

        // 2. Clear data lists
        cachedRooms.Clear();
        corridorPositions.Clear();

        // 3. Generate the Layout
        var roomsListBounds = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        // 4. Create Floors
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        if (randomWalkRooms)
            floor = CreateRoomsRandomly(roomsListBounds);
        else
            floor = CreateSimpleRooms(roomsListBounds);

        // 5. Connect Rooms
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in cachedRooms) roomCenters.Add(room.RoomCenterPos);

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        corridorPositions = corridors;
        floor.UnionWith(corridors);

        // 6. PROCESS LOGIC (Decide which room is which)
        ProcessRoomTypes();

        // 7. SPAWN OBJECTS (Instantiate the prefabs)
        SpawnObjectsInRooms();

        // 8. Visuals
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    private void ClearOldGeneratedObjects()
    {
        // Check if we have a container from a previous run
        if (dungeonContainer != null)
        {
            // Destroy the container and all players/enemies inside it
            DestroyImmediate(dungeonContainer);
        }

        // Create a new fresh container
        dungeonContainer = new GameObject("GeneratedDungeonContent");
    }

    private void ProcessRoomTypes()
    {
        if (cachedRooms.Count == 0) return;

        // 1. Set Spawn (Closest to 0,0)
        Vector2Int startPoint = Vector2Int.zero;
        Room spawnRoom = cachedRooms.OrderBy(x => Vector2.Distance(x.RoomCenterPos, startPoint)).First();
        spawnRoom.Type = RoomType.Spawn;

        // 2. Set Boss (Furthest from Spawn)
        Room bossRoom = cachedRooms.OrderByDescending(x => Vector2.Distance(x.RoomCenterPos, spawnRoom.RoomCenterPos)).First();
        bossRoom.Type = RoomType.Boss;

        // 3. Set Fuel Rooms (Exactly 3, or fewer if map is tiny)
        List<Room> availableRooms = cachedRooms.Where(x => x != spawnRoom && x != bossRoom).ToList();

        // Shuffle the available rooms and take exactly 'exactFuelCount' (e.g., 3)
        int fuelsToSpawn = Mathf.Min(availableRooms.Count, exactFuelCount);
        List<Room> fuelRooms = availableRooms.OrderBy(x => Guid.NewGuid()).Take(fuelsToSpawn).ToList();

        foreach (var room in fuelRooms)
        {
            room.Type = RoomType.Hidden;
        }

        // All other rooms remain "Normal" (Enemy rooms) by default
    }

    private void SpawnObjectsInRooms()
    {
        //Safety check to prevent crashing if prefabs aren't assigned
        if (player1Prefab == null || bossPrefab == null)
        {
            Debug.LogError("Please assign Player/Boss prefabs in the Inspector!");
            return;
        }

        foreach (Room room in cachedRooms)
        {
            // Calculate center
            Vector2 roomCenterWorld = (Vector2)room.RoomCenterPos + new Vector2(0.5f, 0.5f);

            GameObject spawnedObject = null;
            GameObject secondPlayer = null;

            switch (room.Type)
            {
                case RoomType.Spawn:
                    // Spawn Player 1
                    spawnedObject = Instantiate(player1Prefab, roomCenterWorld + Vector2.left, Quaternion.identity);
                    // Spawn Player 2
                    secondPlayer = Instantiate(player2Prefab, roomCenterWorld + Vector2.right, Quaternion.identity);

                    // Put them inside the container
                    if (secondPlayer != null) secondPlayer.transform.SetParent(dungeonContainer.transform);
                    break;

                case RoomType.Boss:
                    spawnedObject = Instantiate(bossPrefab, roomCenterWorld, Quaternion.identity);
                    break;

                case RoomType.Hidden:
                    spawnedObject = Instantiate(fuelItemPrefab, roomCenterWorld, Quaternion.identity);
                    break;

                case RoomType.Normal:
                    // Chance to spawn enemy (controlled by slider)
                    if (Random.value < enemySpawnChance)
                    {
                        spawnedObject = Instantiate(enemyPrefab, roomCenterWorld, Quaternion.identity);
                    }
                    break;
            }
            // If we spawned something main, put it in the container for easy cleanup later
            if (spawnedObject != null)
            {
                spawnedObject.transform.SetParent(dungeonContainer.transform);
            }
        }
    }

    // populates the 'cachedRooms' list
    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            HashSet<Vector2Int> validFloor = new HashSet<Vector2Int>();
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    validFloor.Add(position);
                }
            }
            Room newRoom = new Room
            {
                RoomCenterPos = roomCenter,
                FloorTiles = validFloor,
                Bounds = roomBounds,
                Type = RoomType.Normal
            };
            cachedRooms.Add(newRoom);
            floor.UnionWith(validFloor);
        }
        return floor;
    }

    // populates the 'cachedRooms' list
    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            HashSet<Vector2Int> currentRoomFloor = new HashSet<Vector2Int>();

            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    currentRoomFloor.Add(position);
                }
            }

            // Save to Data
            Room newRoom = new Room
            {
                RoomCenterPos = new Vector2Int(Mathf.RoundToInt(room.center.x), Mathf.RoundToInt(room.center.y)),
                FloorTiles = currentRoomFloor,
                Bounds = room,
                Type = RoomType.Normal
            };
            cachedRooms.Add(newRoom);

            floor.UnionWith(currentRoomFloor);
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;

        PaintCorridorPoint(corridor, position);

        while (position.y != destination.y)
        {
            if (destination.y > position.y)
                position += Vector2Int.up;
            else if (destination.y < position.y)
                position += Vector2Int.down;

            PaintCorridorPoint(corridor, position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
                position += Vector2Int.right;
            else if (destination.x < position.x)
                position += Vector2Int.left;

            PaintCorridorPoint(corridor, position);
        }
        return corridor;
    }

    private void PaintCorridorPoint(HashSet<Vector2Int> corridor, Vector2Int position)
    {
        for (int x = -corridorWidth / 2; x < (corridorWidth + 1) / 2; x++)
        {
            for (int y = -corridorWidth / 2; y < (corridorWidth + 1) / 2; y++)
            {
                corridor.Add(position + new Vector2Int(x, y));
            }
        }
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private void OnDrawGizmos()
    {
        // Draw Corridors
        if (showCorridorsGizmo && corridorPositions != null)
        {
            Gizmos.color = Color.magenta;
            foreach (var pos in corridorPositions)
            {
                Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0) + new Vector3(0.5f, 0.5f, 0), Vector3.one);
            }
        }

        // Draw Rooms
        if (showRoomGizmo && cachedRooms != null)
        {
            foreach (Room room in cachedRooms)
            {
                switch (room.Type)
                {
                    case RoomType.Spawn: Gizmos.color = Color.green; break;
                    case RoomType.Boss: Gizmos.color = Color.red; break;
                    case RoomType.Hidden: Gizmos.color = Color.blue; break;
                    case RoomType.Normal: Gizmos.color = Color.white; break;
                }

                foreach (var pos in room.FloorTiles)
                {
                    Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0) + new Vector3(0.5f, 0.5f, 0), Vector3.one);
                }
                // Draw center point
                Gizmos.DrawSphere(new Vector3(room.RoomCenterPos.x, room.RoomCenterPos.y, 0) + new Vector3(0.5f, 0.5f, 0), 1.0f);
            }
        }
    }
}
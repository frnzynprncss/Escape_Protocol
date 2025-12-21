using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    // --- Singleton Instance ---
    public static RoomFirstDungeonGenerator Instance;

    // --- Master list of all safe tiles ---
    private HashSet<Vector2Int> allValidFloors = new HashSet<Vector2Int>();

    [Header("escape Prefab")]
    [SerializeField] private GameObject escapeBasePrefab;

    [Header("Room First Parameters")]
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField][Range(0, 10)] private int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;
    [SerializeField][Range(1, 5)] private int corridorWidth = 5;

    [Header("Game Data / Spawning")]
    [SerializeField] private bool showRoomGizmo = false;
    [SerializeField] private bool showCorridorsGizmo = false;

    [Header("Prefabs")]
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;

    // --- MODIFIED: Changed single GameObject to an Array ---
    [Tooltip("Add your 4 mob prefabs here")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [SerializeField] private GameObject[] spaceshipPartPrefabs = new GameObject[3];
    [SerializeField] private GameObject accessCardPrefab;
    [SerializeField] private GameObject fuelItemPrefab;
    [SerializeField] private GameObject spaceshipPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Minimum enemies in rooms closest to player")]
    [SerializeField] private int minEnemiesPerRoom = 0;
    [Tooltip("Maximum enemies in rooms furthest from player")]
    [SerializeField] private int maxEnemiesPerRoom = 5;
    [Tooltip("Extra enemies to add specifically in Item/Hidden rooms")]
    [SerializeField] private int bonusEnemiesInItemRooms = 2;
    [SerializeField] private int exactFuelCount = 3;

    [Header("Dungeon Parent")]
    [SerializeField] private Transform dungeonParent;

    private List<Room> cachedRooms = new List<Room>();
    private HashSet<Vector2Int> corridorPositions = new HashSet<Vector2Int>();
    private GameObject dungeonContainer;
    private List<Room> dedicatedFuelRooms = new List<Room>();
    private Transform _spawnedP1;
    private Transform _spawnedP2;

    public static event Action<Transform, Transform> OnPlayersSpawned;

    private SpaceShipInteraction spaceshipManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RunProceduralGeneration();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RunProceduralGeneration();
        }
    }

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        ClearOldGeneratedObjects();
        if (tilemapVisualizer != null) tilemapVisualizer.Clear();
        cachedRooms.Clear();
        corridorPositions.Clear();
        allValidFloors.Clear();

        var roomsListBounds = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)),
            minRoomWidth, minRoomHeight
        );

        HashSet<Vector2Int> floor = randomWalkRooms ? CreateRoomsRandomly(roomsListBounds) : CreateSimpleRooms(roomsListBounds);
        List<Vector2Int> roomCenters = cachedRooms.ConvertAll(r => r.RoomCenterPos);
        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        corridorPositions = corridors;
        floor.UnionWith(corridors);

        allValidFloors = new HashSet<Vector2Int>(floor);

        ProcessRoomTypes();
        SpawnObjectsInRooms();

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

#if UNITY_EDITOR
        var cam = FindObjectOfType<GameCamera>();
        if (cam != null)
        {
            var players = GetSpawnedPlayers();
            cam.SetPlayers(players.p1, players.p2);
        }
#endif
    }

    public bool IsPositionOnFloor(Vector3 worldPos)
    {
        Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y));
        return allValidFloors.Contains(gridPos);
    }

    private void ClearOldGeneratedObjects()
    {
        if (dungeonParent == null) return;
        var existingContainer = dungeonParent.Find("GeneratedDungeonContent");
        if (existingContainer != null)
        {
            if (Application.isPlaying) Destroy(existingContainer.gameObject);
            else DestroyImmediate(existingContainer.gameObject);
        }
        dungeonContainer = new GameObject("GeneratedDungeonContent");
        dungeonContainer.transform.SetParent(dungeonParent, false);
    }

    private void ProcessRoomTypes()
    {
        dedicatedFuelRooms.Clear();
        if (cachedRooms.Count == 0) return;

        Vector2Int startPoint = Vector2Int.zero;
        Room spawnRoom = cachedRooms.OrderBy(x => Vector2.Distance(x.RoomCenterPos, startPoint)).First();
        spawnRoom.Type = RoomType.Spawn;

        Room accessCardRoom = cachedRooms.Except(new List<Room> { spawnRoom })
                                         .OrderByDescending(x => Guid.NewGuid()).First();
        accessCardRoom.Type = RoomType.Boss;

        List<Room> availableRooms = cachedRooms.Except(new List<Room> { spawnRoom, accessCardRoom }).ToList();
        int partsToSpawn = Mathf.Min(availableRooms.Count, Math.Max(1, spaceshipPartPrefabs.Length));
        List<Room> spaceshipPartRooms = availableRooms.OrderBy(x => Guid.NewGuid()).Take(partsToSpawn).ToList();
        foreach (var room in spaceshipPartRooms) room.Type = RoomType.Hidden;

        List<Room> fuelAvailableRooms = availableRooms.Except(spaceshipPartRooms).ToList();
        int fuelsToSpawn = Mathf.Min(fuelAvailableRooms.Count, exactFuelCount);
        dedicatedFuelRooms = fuelAvailableRooms.OrderBy(x => Guid.NewGuid()).Take(fuelsToSpawn).ToList();
    }

    private void SpawnObjectsInRooms()
    {
        _spawnedP1 = null;
        _spawnedP2 = null;

        if (player1Prefab == null || escapeBasePrefab == null)
        {
            Debug.LogError("Assign all Prefabs!");
            return;
        }

        Room spawnRoom = cachedRooms.FirstOrDefault(r => r.Type == RoomType.Spawn);
        Vector2 spawnRoomCenter = spawnRoom != null ? (Vector2)spawnRoom.RoomCenterPos : Vector2.zero;

        float maxDistance = 0f;
        foreach (var room in cachedRooms)
        {
            float dist = Vector2.Distance(room.RoomCenterPos, spawnRoomCenter);
            if (dist > maxDistance) maxDistance = dist;
        }

        GameObject spawnedObject = null;
        int hiddenPartIndex = 0;

        foreach (Room room in cachedRooms)
        {
            Vector2 roomCenterWorld = (Vector2)room.RoomCenterPos + new Vector2(0.5f, 0.5f);
            spawnedObject = null;

            float currentDist = Vector2.Distance(room.RoomCenterPos, spawnRoomCenter);
            float difficultyRatio = (maxDistance > 0) ? currentDist / maxDistance : 0;
            int enemiesToSpawn = Mathf.RoundToInt(Mathf.Lerp(minEnemiesPerRoom, maxEnemiesPerRoom, difficultyRatio));

            bool isHighValueRoom = room.Type == RoomType.Hidden || room.Type == RoomType.Boss || dedicatedFuelRooms.Contains(room);

            if (isHighValueRoom)
            {
                enemiesToSpawn = maxEnemiesPerRoom + bonusEnemiesInItemRooms;
            }

            switch (room.Type)
            {
                case RoomType.Spawn:
                    var baseObj = Instantiate(escapeBasePrefab, roomCenterWorld, Quaternion.identity);
                    baseObj.transform.SetParent(dungeonContainer.transform);

                    var p1Obj = Instantiate(player1Prefab, roomCenterWorld + Vector2.left * 2, Quaternion.identity);
                    p1Obj.name = "Player1";
                    p1Obj.transform.SetParent(dungeonContainer.transform);
                    _spawnedP1 = p1Obj.transform;

                    var p2Obj = Instantiate(player2Prefab, roomCenterWorld + Vector2.right * 2, Quaternion.identity);
                    p2Obj.name = "Player2";
                    p2Obj.transform.SetParent(dungeonContainer.transform);
                    _spawnedP2 = p2Obj.transform;

                    if (spaceshipPrefab != null)
                    {
                        spawnedObject = Instantiate(spaceshipPrefab, roomCenterWorld, Quaternion.identity, dungeonContainer.transform);
                        spaceshipManager = spawnedObject.GetComponent<SpaceShipInteraction>();
                        if (spaceshipManager != null)
                        {
                            spaceshipManager.partsVisualInstance = spawnedObject;
                            spaceshipManager.completeVisualInstance = spaceshipManager.completedSpaceshipVisual;
                            if (spaceshipManager.completeVisualInstance != null)
                                spaceshipManager.completeVisualInstance.SetActive(false);
                        }
                    }
                    enemiesToSpawn = 0;
                    break;

                case RoomType.Boss:
                    spawnedObject = Instantiate(accessCardPrefab, roomCenterWorld, Quaternion.identity);
                    break;

                case RoomType.Hidden:
                    if (hiddenPartIndex < spaceshipPartPrefabs.Length)
                        spawnedObject = Instantiate(spaceshipPartPrefabs[hiddenPartIndex], roomCenterWorld, Quaternion.identity);
                    else
                        spawnedObject = Instantiate(spaceshipPartPrefabs[spaceshipPartPrefabs.Length - 1], roomCenterWorld, Quaternion.identity);

                    hiddenPartIndex++;
                    break;

                case RoomType.Normal:
                    if (dedicatedFuelRooms.Contains(room))
                    {
                        spawnedObject = Instantiate(fuelItemPrefab, roomCenterWorld, Quaternion.identity);
                    }
                    break;
            }

            if (spawnedObject != null)
                spawnedObject.transform.SetParent(dungeonContainer.transform);

            // --- MODIFIED: Validation Check for Array ---
            if (enemiesToSpawn > 0 && enemyPrefabs != null && enemyPrefabs.Length > 0)
            {
                SpawnEnemiesInRoom(room, enemiesToSpawn, roomCenterWorld);
            }
        }

        if (_spawnedP1 != null && _spawnedP2 != null)
            OnPlayersSpawned?.Invoke(_spawnedP1, _spawnedP2);

        if (_spawnedP1 != null && InventoryUI.Instance != null)
        {
            var holder = _spawnedP1.GetComponent<PlayerInventoryHolder>();
            if (holder != null)
            {
                InventoryUI.Instance.playerInventory = holder.inventory;
                InventoryUI.Instance.RefreshAllSlots();
            }
        }
    }

    private void SpawnEnemiesInRoom(Room room, int count, Vector2 centerToAvoid)
    {
        List<Vector2Int> floorPositions = room.FloorTiles.ToList();
        floorPositions = floorPositions.OrderBy(x => Random.value).ToList();

        int spawnedCount = 0;
        foreach (var pos in floorPositions)
        {
            if (spawnedCount >= count) break;
            Vector2 spawnPos = new Vector2(pos.x + 0.5f, pos.y + 0.5f);
            if (Vector2.Distance(spawnPos, centerToAvoid) < 1.0f) continue;

            // --- MODIFIED: Pick Random Enemy from the Array ---
            GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            var enemy = Instantiate(randomEnemyPrefab, spawnPos, Quaternion.identity);
            enemy.transform.SetParent(dungeonContainer.transform);
            spawnedCount++;
        }
    }

    // ... [Rest of the file: RandomWalk, Room Creation, etc. remains unchanged] ...
    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var roomBounds in roomsList)
        {
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            HashSet<Vector2Int> validFloor = new HashSet<Vector2Int>();
            foreach (var pos in roomFloor)
            {
                if (pos.x >= roomBounds.xMin + offset && pos.x <= roomBounds.xMax - offset &&
                    pos.y >= roomBounds.yMin + offset && pos.y <= roomBounds.yMax - offset)
                    validFloor.Add(pos);
            }
            cachedRooms.Add(new Room
            {
                RoomCenterPos = roomCenter,
                FloorTiles = validFloor,
                Bounds = roomBounds,
                Type = RoomType.Normal
            });
            floor.UnionWith(validFloor);
        }
        return floor;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            HashSet<Vector2Int> currentRoomFloor = new HashSet<Vector2Int>();
            for (int x = offset; x < room.size.x - offset; x++)
                for (int y = offset; y < room.size.y - offset; y++)
                    currentRoomFloor.Add((Vector2Int)room.min + new Vector2Int(x, y));

            cachedRooms.Add(new Room
            {
                RoomCenterPos = new Vector2Int(Mathf.RoundToInt(room.center.x), Mathf.RoundToInt(room.center.y)),
                FloorTiles = currentRoomFloor,
                Bounds = room,
                Type = RoomType.Normal
            });

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
            corridors.UnionWith(CreateCorridor(currentRoomCenter, closest));
            currentRoomCenter = closest;
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int start, Vector2Int end)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        Vector2Int pos = start;
        PaintCorridorPoint(corridor, pos);
        while (pos.y != end.y)
        {
            pos += (end.y > pos.y) ? Vector2Int.up : Vector2Int.down;
            PaintCorridorPoint(corridor, pos);
        }
        while (pos.x != end.x)
        {
            pos += (end.x > pos.x) ? Vector2Int.right : Vector2Int.left;
            PaintCorridorPoint(corridor, pos);
        }
        return corridor;
    }

    private void PaintCorridorPoint(HashSet<Vector2Int> corridor, Vector2Int pos)
    {
        for (int x = -corridorWidth / 2; x < (corridorWidth + 1) / 2; x++)
            for (int y = -corridorWidth / 2; y < (corridorWidth + 1) / 2; y++)
                corridor.Add(pos + new Vector2Int(x, y));
    }

    private Vector2Int FindClosestPointTo(Vector2Int current, List<Vector2Int> points)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var p in points)
        {
            float d = Vector2.Distance(p, current);
            if (d < distance)
            {
                distance = d;
                closest = p;
            }
        }
        return closest;
    }

    private void OnDrawGizmos()
    {
        if (showCorridorsGizmo && corridorPositions != null)
        {
            Gizmos.color = Color.magenta;
            foreach (var pos in corridorPositions)
                Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0) + Vector3.one * 0.5f, Vector3.one);
        }

        if (showRoomGizmo && cachedRooms != null)
        {
            foreach (var room in cachedRooms)
            {
                switch (room.Type)
                {
                    case RoomType.Spawn: Gizmos.color = Color.green; break;
                    case RoomType.Boss: Gizmos.color = Color.red; break;
                    case RoomType.Hidden: Gizmos.color = Color.blue; break;
                    case RoomType.Normal: Gizmos.color = Color.white; break;
                }
                foreach (var pos in room.FloorTiles)
                    Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0) + Vector3.one * 0.5f, Vector3.one);
                Gizmos.DrawSphere(new Vector3(room.RoomCenterPos.x, room.RoomCenterPos.y, 0) + Vector3.one * 0.5f, 1f);
            }
        }
    }

    public (Transform p1, Transform p2) GetSpawnedPlayers() => (_spawnedP1, _spawnedP2);
}
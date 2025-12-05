using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPlacementHelper
{
    Dictionary<PlacementType, HashSet<Vector2Int>> tileByType = new Dictionary<PlacementType, HashSet<Vector2Int>>();

    HashSet<Vector2Int> roomFloorNoCorridor;

    public ItemPlacementHelper(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridor)
    {
        // Ensure you have the 'Graph' class defined elsewhere in your project!
        Graph graph = new Graph(roomFloor);
        this.roomFloorNoCorridor = roomFloorNoCorridor;

        foreach (var position in roomFloorNoCorridor)
        {
            int neighboursCount8Dir = graph.GetNeighbours8Directions(position).Count;

            // FIX 1: Completed the ternary operator
            PlacementType type = neighboursCount8Dir < 8 ? PlacementType.NearWall : PlacementType.OpenSpace;

            if (tileByType.ContainsKey(type) == false)
                tileByType[type] = new HashSet<Vector2Int>();

            // FIX 2: Completed the condition
            // If it is 'NearWall' but has 4 cardinal neighbors, it means the missing neighbor 
            // is diagonal. We usually treat this as open space or skip it to avoid weird corners.
            if (type == PlacementType.NearWall && graph.GetNeighbours4Directions(position).Count == 4)
                continue;

            tileByType[type].Add(position);
        }
    }

    // FIX 3: Added name for 'size' parameter and added missing 'bool addOffset' parameter
    public Vector2? GetItemPlacementPosition(PlacementType placementType, int iterationsMax, Vector2Int size, bool addOffset)
    {
        int itemArea = size.x * size.y;

        // Safety check if key exists
        if (tileByType.ContainsKey(placementType) == false || tileByType[placementType].Count < itemArea)
            return null;

        int iteration = 0;
        while (iteration < iterationsMax)
        {
            iteration++;
            int index = UnityEngine.Random.Range(0, tileByType[placementType].Count);
            Vector2Int position = tileByType[placementType].ElementAt(index);

            if (itemArea > 1 || addOffset)
            {
                // Now 'addOffset' exists in this scope
                var (result, placementPositions) = PlaceBigItem(position, size, addOffset);

                if (result == false)
                    continue;

                tileByType[placementType].ExceptWith(placementPositions);

                // Safety check before accessing NearWall
                if (tileByType.ContainsKey(PlacementType.NearWall))
                    tileByType[PlacementType.NearWall].ExceptWith(placementPositions);
            }
            else
            {
                tileByType[placementType].Remove(position);
            }

            return position;
        }
        return null;
    }

    private (bool, List<Vector2Int>) PlaceBigItem(Vector2Int originPosition, Vector2Int size, bool addOffset)
    {
        List<Vector2Int> positions = new List<Vector2Int>() { originPosition };

        // FIX 4: Loop Logic 
        // We use '<' in the loop, so we set Max to size.x (no offset) or size.x + 1 (with offset)
        int maxX = addOffset ? size.x + 1 : size.x;
        int maxY = addOffset ? size.y + 1 : size.y;
        int minX = addOffset ? -1 : 0;
        int minY = addOffset ? -1 : 0;

        // FIX 5: Changed loop from '<=' to '<'
        // If size is 1 and no offset: min 0, max 1. 
        // Loop 'row < 1' runs once (index 0). Correct.
        // Loop 'row <= 1' runs twice (index 0, 1). Incorrect.
        for (int row = minX; row < maxX; row++)
        {
            for (int col = minY; col < maxY; col++)
            {
                if (col == 0 && row == 0)
                    continue;

                Vector2Int newPosToCheck = new Vector2Int(originPosition.x + row, originPosition.y + col);

                if (roomFloorNoCorridor.Contains(newPosToCheck) == false)
                    return (false, positions);

                positions.Add(newPosToCheck);
            }
        }
        return (true, positions);
    }
}

public enum PlacementType
{
    OpenSpace,
    NearWall
}
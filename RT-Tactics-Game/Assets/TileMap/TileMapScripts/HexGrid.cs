using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public static HexGrid Instance { get; private set; }
    private Dictionary<Vector3Int, Hex> hexTileDict = new Dictionary<Vector3Int, Hex>();
    private Dictionary<Vector3Int, List<Vector3Int>> hexTileNeighboursDict = new Dictionary<Vector3Int, List<Vector3Int>>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartHexGrid()
    {
        foreach (Hex hex in FindObjectsOfType<Hex>())
        {
            hexTileDict[hex.HexCoords] = hex;
        }
    }

    public Hex GetTileAt(Vector3Int hexCoordinates)
    {
        hexTileDict.TryGetValue(hexCoordinates, out Hex result);
        return result;
    }

    public List<Vector3Int> GetNeighboursFor(Vector3Int hexCoordinates)
    {
        if (!hexTileDict.ContainsKey(hexCoordinates))
            return new List<Vector3Int>();

        if (hexTileNeighboursDict.ContainsKey(hexCoordinates))
            return hexTileNeighboursDict[hexCoordinates];

        List<Vector3Int> neighbors = new List<Vector3Int>();
        foreach (Vector3Int direction in Direction.GetDirectionList(hexCoordinates.z))
        {
            Vector3Int neighborCoord = hexCoordinates + direction;
            if (hexTileDict.ContainsKey(neighborCoord) && hexTileDict[neighborCoord].IsPassable())
            {
                neighbors.Add(neighborCoord);
                Debug.Log($"Neighbor found: {neighborCoord} is passable.");
            }
            else
            {
                Debug.Log($"Neighbor skipped: {neighborCoord} is either not present or impassable.");
            }
        }

        hexTileNeighboursDict[hexCoordinates] = neighbors;
        return neighbors;
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, int maxSteps)
    {
        Debug.Log($"Starting pathfinding from {start} to {goal} with max steps {maxSteps}.");

        Queue<Vector3Int> frontier = new Queue<Vector3Int>();
        frontier.Enqueue(start);

        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        cameFrom[start] = start;

        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();
            if (current == goal)
            {
                Debug.Log("Goal reached.");
                break;
            }

            foreach (Vector3Int next in GetNeighboursFor(current))
            {
                int newCost = costSoFar[current] + 1;

                if (!cameFrom.ContainsKey(next) && newCost <= maxSteps)
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                    costSoFar[next] = newCost;
                    Debug.Log($"Added {next} to path with cost {newCost}.");
                }
                else
                {
                    Debug.Log($"{next} is already in path or exceeds max steps.");
                }
            }
        }

        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int currentStep = goal;

        while (currentStep != start && cameFrom.ContainsKey(currentStep))
        {
            path.Add(currentStep);
            currentStep = cameFrom[currentStep];
        }

        path.Reverse();

        Debug.Log("Final Path:");
        foreach (var step in path)
        {
            Debug.Log(step);
        }

        return path.Count > 0 && path.Count <= maxSteps ? path : null;
    }

    public int CalculateHexDistance(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        int dz = Mathf.Abs(a.z - b.z);
        return Mathf.Max(dx, dy, dz);
    }


public static class Direction
    {
        public static List<Vector3Int> directionsOffsetOdd = new List<Vector3Int>
    {
        new Vector3Int(-1, 0, 1),  // N1
        new Vector3Int(0, 0, 1),   // N2
        new Vector3Int(1, 0, 0),   // E
        new Vector3Int(0, 0, -1),  // S2
        new Vector3Int(-1, 0, -1), // S1
        new Vector3Int(-1, 0, 0)   // W
    };

        public static List<Vector3Int> directionsOffsetEven = new List<Vector3Int>
    {
        new Vector3Int(0, 0, 1),   // N1
        new Vector3Int(1, 0, 1),   // N2
        new Vector3Int(1, 0, 0),   // E
        new Vector3Int(1, 0, -1),  // S2
        new Vector3Int(0, 0, -1),  // S1
        new Vector3Int(-1, 0, 0)   // W
    };

        public static List<Vector3Int> GetDirectionList(int z)
            => z % 2 == 0 ? directionsOffsetEven : directionsOffsetOdd;
    }
}

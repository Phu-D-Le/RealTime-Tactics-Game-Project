using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Complicated class but idea is BFS is used to find valid paths. Reused for movement and attack. ZO

public class GraphSearch
{
    public static BFSResult BFSGetRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints)
    {
        Dictionary<Vector3Int, Vector3Int?> visitedNodes = new Dictionary<Vector3Int, Vector3Int?>(); // Stores parents of visited nodes. Finds referenced path. ZO
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>(); // Stores full cost of BFS tiles. ZO
        Queue<Vector3Int> nodesToVisitQueue = new Queue <Vector3Int>(); // Queues each neighbour for future checking. Valid movement points? ZO
    
        nodesToVisitQueue.Enqueue(startPoint);
        costSoFar.Add(startPoint, 0);
        visitedNodes.Add(startPoint, null); // Start from nothing. ZO

        while (nodesToVisitQueue.Count > 0) // start with neighbours surrounding current tile. ZO
        {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue(); // Look through all neighbours. ZO
            foreach (Vector3Int neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
                if (hexGrid.GetTileAt(neighbourPosition).IsHazard()) // Skip Hazard tiles (skippable tiles). ZO
                    continue;

                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar[currentNode]; // Current tile cost is gotten and neighbour is retrieved and newCost checks if valid. ZO
                int newCost = currentCost + nodeCost;
            
                if (newCost <= movementPoints)
                {
                    if (!visitedNodes.ContainsKey(neighbourPosition)) // If we have not traveled here, simply update. ZO
                    {
                        visitedNodes[neighbourPosition] = currentNode;
                        costSoFar[neighbourPosition] = newCost;
                        nodesToVisitQueue.Enqueue(neighbourPosition); // Find new neighbours to continue traversing. ZO
                    }
                    else if (costSoFar[neighbourPosition] > newCost) // Updates parent if cheaper route is found. ZO
                    {
                        costSoFar[neighbourPosition] = newCost;
                        visitedNodes[neighbourPosition] = currentNode;
                    }
                }
            }
        }
        return new BFSResult { visitedNodesDict = visitedNodes};
    }
    public static List<Vector3Int> GeneratePathBFS(Vector3Int current, Dictionary<Vector3Int, Vector3Int?> visitedNodesDict)
    { // This method confuses me. I think it adds the clicked tile as the endpoint and if its valid then continuously update
    // the current tile in the path according to the BFS until the parent is visited. ZO
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(current);
        while (visitedNodesDict[current] != null)
        {
            path.Add(visitedNodesDict[current].Value);
            current = visitedNodesDict[current].Value;
        }
        path.Reverse();
        return path.Skip(1).ToList();
    }
}
public struct BFSResult // Checks if the tile clicked contains a valid path. ZO
{
    public Dictionary<Vector3Int, Vector3Int?> visitedNodesDict;

    public List<Vector3Int> GetPathTo(Vector3Int destination)
    {
        if (visitedNodesDict.ContainsKey(destination) == false)
            return new List<Vector3Int>();
        return GraphSearch.GeneratePathBFS(destination, visitedNodesDict);
    }
    public bool IsHexPositionInRange(Vector3Int position)
    {
        return visitedNodesDict.ContainsKey(position);
    }
    public IEnumerable<Vector3Int> GetRangePositions()
        => visitedNodesDict.Keys;
}
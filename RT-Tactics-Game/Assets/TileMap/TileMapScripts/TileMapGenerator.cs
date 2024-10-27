using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour
{
    public GameObject freeTile;
    public GameObject[] TileTypes;
    public int[] TileCounts;
    public int gridX = 10;
    public int gridY = 10;
    public float tileRadius = 1f;
    public float hSpacing = 1f;
    public float vSpacing = 1f;
    private float tileHeight;
    private float tileWidth;
    private float verticalSpacing;
    private float horizontalSpacing;
    private List<Vector2Int> availablePositions;

    void Start()
    {
        CalculateHexDimensions();
        GenerateTileMap();
    }

    void CalculateHexDimensions()
    {
        tileHeight = Mathf.Sqrt(3) * tileRadius;
        tileWidth = 2 * tileRadius;
        verticalSpacing = tileWidth * 0.75f * vSpacing;
        horizontalSpacing = tileHeight * hSpacing;
    }

    void GenerateTileMap()
    {
        availablePositions = new List<Vector2Int>();

        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                availablePositions.Add(new Vector2Int(x, y));
            }
        }

        for (int i = 0; i < TileTypes.Length; i++)
        {
            if (TileTypes[i] != null && i < TileCounts.Length)
            {
                for (int j = 0; j < TileCounts[i]; j++)
                {
                    if (availablePositions.Count == 0)
                    {
                        Debug.LogWarning("no space for tiles left");
                        return;
                    }

                    int randomIndex = Random.Range(0, availablePositions.Count);
                    Vector2Int position = availablePositions[randomIndex];
                    availablePositions.RemoveAt(randomIndex);

                    Vector3 worldPosition = CalculateWorldPosition(position.x, position.y);
                    Instantiate(TileTypes[i], worldPosition, Quaternion.identity, transform);
                }
            }
        }

        foreach (Vector2Int pos in availablePositions)
        {
            Vector3 worldPosition = CalculateWorldPosition(pos.x, pos.y);
            Instantiate(freeTile, worldPosition, Quaternion.identity, transform);
        }
    }

    Vector3 CalculateWorldPosition(int x, int y)
    {
        float xOffset = x * horizontalSpacing;
        float yOffset = y * verticalSpacing;

        if (y % 2 == 1)
        {
            xOffset += horizontalSpacing * 0.5f;
        }

        return new Vector3(xOffset, 0, yOffset);
    }
}

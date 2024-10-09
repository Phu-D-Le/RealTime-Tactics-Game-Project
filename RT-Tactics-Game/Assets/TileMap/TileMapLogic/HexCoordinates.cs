using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoordinates : MonoBehaviour
{
    public static float xOffset = 1.8186f, yOffset = 3.023109f, zOffset = 1.575f;

    internal Vector3Int GetHexCoords()
        => offsetCoordinates;

    [Header("Offset coordinates")]
    [SerializeField]
    private Vector3Int offsetCoordinates;

    private void Awake()
    {
        offsetCoordinates = ConvertPositionOffset(transform.position);
    }
    private Vector3Int ConvertPositionOffset(Vector3 position)
    {
        int x = Mathf.CeilToInt(position.z / xOffset); // Use z-axis for x in grid
        int y = Mathf.FloorToInt(position.y / yOffset);
        int z = Mathf.CeilToInt(position.x / zOffset);

        // Debugging output
     // Debug.Log($"World position: {position} -> Hex coordinates: ({x}, {y}, {z})");

        return new Vector3Int(x, y, z);
    }
}

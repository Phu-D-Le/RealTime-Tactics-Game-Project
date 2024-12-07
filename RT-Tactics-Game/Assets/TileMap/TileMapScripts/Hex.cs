using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// The Hex class defines the behavior and properties of each hexagonal tile on the map.
[SelectionBase]
public class Hex : MonoBehaviour
{
    [SerializeField]
    private GlowHighlight highlight; // For tile highlighting
    private HexCoordinates hexCoordinates; // Hex coordinates of this tile

    [SerializeField]
    private HexType hexType; // The type of this tile (e.g., Free, Hazard, Spawner)

    private Pawn occupyingPawn; // Tracks the pawn occupying this tile

    public Vector3Int HexCoords => hexCoordinates.GetHexCoords();

    public int GetCost()
        => hexType switch
        {
            HexType.Interactable => 1,
            HexType.Free => 1,
            HexType.Spawner => 1,
            HexType.Hazard => 1,
            _ => throw new Exception($"Hex of type {hexType} not supported")
        };

    public bool IsHazard()
    {
        return this.hexType == HexType.Hazard;
    }

    // Checks if the tile is occupied
    public bool IsOccupied()
    {
        return occupyingPawn != null;
    }

    // Gets the pawn occupying this tile
    public Pawn GetOccupyingPawn()
    {
        return occupyingPawn;
    }

    // Sets the pawn occupying this tile
    public void OccupyTile(Pawn pawn)
    {
        occupyingPawn = pawn;
    }

    // Clears the occupying pawn
    public void VacateTile()
    {
        occupyingPawn = null;
    }

    private void Awake()
    {
        hexCoordinates = GetComponent<HexCoordinates>();
        highlight = GetComponent<GlowHighlight>();
    }

    public void EnableHighlight(Color color)
    {
        highlight.ToggleGlow(true);
        this.GetComponent<Renderer>().material.color = color;
    }

    public void DisableHighlight()
    {
        highlight.ToggleGlow(false);
    }
}

public enum HexType
{
    None,
    Interactable,
    Free,
    Hazard,
    Spawner,
    Rough
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Serves to set behaviours and values depending on the hex type. The cost of travel can be set for each tile (so
// slowing tiles like water/slime can be supported) and environmental hazard tiles can be added that the pawn must
// travel around (instead of having gaps in map we can have tiles like lava that player cannot interact with). The current
// hazard tiles are skippable. no traps yet. ZO

[SelectionBase]
public class Hex : MonoBehaviour
{
    [SerializeField]
    private GlowHighlight highlight;
    private HexCoordinates hexCoordinates;

    [SerializeField]
    private HexType hexType;

    public Vector3Int HexCoords => hexCoordinates.GetHexCoords();

    public int GetCost()
        => hexType switch
    {
        HexType.Interactable => 1,
        HexType.Free => 1,
        // HexType.Hazard => 2,
        HexType.Spawner => 1,
        HexType.Hazard => 1,
        _=> throw new Exception($"Hex of type {hexType} not supported")
    };
    public bool IsHazard()
    {
        return this.hexType == HexType.Hazard;
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
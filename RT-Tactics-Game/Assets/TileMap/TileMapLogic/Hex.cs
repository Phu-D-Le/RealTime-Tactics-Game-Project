using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public void EnableHighlight()
    {
        highlight.ToggleGlow(true);
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
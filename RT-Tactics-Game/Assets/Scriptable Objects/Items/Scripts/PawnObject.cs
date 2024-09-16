using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pawn Object", menuName = "Inventory System/Items/Pawns")]
public class PawnObject : ItemObject
{
    public int Health;
    public int Speed;
    public PawnType pawnType;
    //public GameObject uiPrefab; // For menu/UI display
    public GameObject gamePrefab; // For in-game representation

    public InventoryObject pawnInventory; // To store attacks or other items

    public enum PawnType
    {
        Small,
        Medium,
        Heavy
    }

    public void Awake()
    {
        type = ItemType.Pawn;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pawn Object", menuName = "Inventory System/Items/Pawns")]
public class PawnObject : ItemObject
{
    public int Health;
    public int Speed;
    public PawnType pawnType;
    public List<AttackObject> learnedAttacks; // List of attacks this pawn knows

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
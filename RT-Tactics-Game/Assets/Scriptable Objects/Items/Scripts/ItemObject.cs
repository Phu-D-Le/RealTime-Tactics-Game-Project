using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Attack,
    Pawn,
    Default
}
public abstract class ItemObject : ScriptableObject
{
    public GameObject imagePrefab;
    public ItemType type;
    [TextArea(15,20)]
    public string description;
}

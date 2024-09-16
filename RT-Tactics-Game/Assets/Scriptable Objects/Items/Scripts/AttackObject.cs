using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Object", menuName = "Inventory System/Items/Attacks")]
public class AttackObject : ItemObject
{
    public int Damage;
    public void Awake()
    {
        type = ItemType.Attack;
    }
}

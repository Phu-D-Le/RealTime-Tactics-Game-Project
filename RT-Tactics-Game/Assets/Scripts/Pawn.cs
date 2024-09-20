using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Pawn : MonoBehaviour
{
    public string pawnName;
    public Sprite pawnSprite;
    public Attack defaultAttack;
    public int pawnSpeed;

    public int maxHP;
    public int currentHP;

    public void Attack()
    {
        Debug.Log($"{pawnName} will now {defaultAttack.attackName} dealing {defaultAttack.damage} damage.");
    }
}

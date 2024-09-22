using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Pawn : MonoBehaviour
{
    public string pawnName;
    public Sprite pawnSprite;
    public List<Attack> attacks;
    public int pawnSpeed;
    public AttackHUD attackHUD;

    public int maxHP;
    public int currentHP;

    public void Attack()
    {
        // Loop through the list of attacks and log each one
        for (int i = 0; i < attacks.Count; i++)
        {
            Attack currentAttack = attacks[i];
            Debug.Log($"{pawnName} will now use {currentAttack.attackName}, dealing {currentAttack.damage} damage.");
        }
    }
}
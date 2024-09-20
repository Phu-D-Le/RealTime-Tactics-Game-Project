using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Slider healthBar;

    public void SetHUD(Pawn pawn)
    {
        healthBar.maxValue = pawn.maxHP;
        healthBar.value = pawn.currentHP;
    }
    public void SetHP(int hp)
    {
        healthBar.value = hp;
    }
}
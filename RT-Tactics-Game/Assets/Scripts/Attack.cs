using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public string attackName;
    public int damage;

    public Attack(string name, int dmg)
    {
        attackName = name;
        damage = dmg;
    }

    public void DealDamage(PlayerType target)
    {
        target.TakeDamage(damage);
    }
}
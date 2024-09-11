using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack")]
public class Attack : ScriptableObject
{
    public string attackName;
    public int damage;

    public void Initialize(string name, int dmg)
    {
        attackName = name;
        damage = dmg;
    }
}

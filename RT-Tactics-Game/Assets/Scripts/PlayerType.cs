using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerType : MonoBehaviour
{
    public int Health { get; private set; }
    public int Speed { get; private set; } //Public get but can only be changed here
    public Size type; // Public field to set the type in the Inspector

    public List<Attack> attacks = new List<Attack>();

    void Start()
    {
        InitializePlayerType(type);
    }
    public enum Size //Declare sizes
    {
        Small,
        Medium,
        Heavy
    }
    private void InitializePlayerType(Size type)
    {
        switch (type)
        {
            case Size.Small:
                Health = 2;
                Speed = 3;
                AddDefaultAttack("Short Swing", 1); // Default attack for Small pawn
                break;
            case Size.Medium:
                Health = 3;
                Speed = 2;
                AddDefaultAttack("Ranged Attack", 1); // Default attack for Medium pawn
                break;
            case Size.Heavy:
                Health = 4;
                Speed = 1;
                AddDefaultAttack("Wide Swing", 1); // Default attack for Heavy pawn
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0; // Prevent negative damage
    }
    public void LearnNewAttack(Attack newAttack)
    {
        if (attacks.Count < 3) // Limit to 3 attacks
        {
            attacks.Add(newAttack);
        }
        else
        {
            Debug.Log("No space for new attacks.");
        }
    }
    private void AddDefaultAttack(string attackName, int damage)
    {
        Attack newAttack = new Attack(attackName, damage);
        attacks.Add(newAttack);
    }
}

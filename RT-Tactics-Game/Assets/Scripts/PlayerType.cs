using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerType : MonoBehaviour
{
    public int Health { get; private set; }
    public int Speed { get; private set; } //Public get but can only be changed here

    public List<Attack> learnedAttacks = new List<Attack>(); // Attacks the pawn has learned
    public List<Attack> availableAttacks = new List<Attack>(); // Attacks the pawn can learn

    public enum Size //Declare sizes
    {
        Small,
        Medium,
        Heavy
    }
    public void PawnStats(Size type)
    {
        switch (type)
        {
            case Size.Small:
                Health = 2;
                Speed = 3;
                AddDefaultAttack("Short Swing", 3);
                break;
            case Size.Medium:
                Health = 3;
                Speed = 2;
                AddDefaultAttack("Ranged Attack", 2);
                break;
            case Size.Heavy:
                Health = 4;
                Speed = 1;
                AddDefaultAttack("Wide Swing", 1);
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
        // Set available attacks for the pawn
        availableAttacks = Attack.GetAvailableAttacks(type, gameObject);
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0; // Prevent negative damage
    }
    private void AddDefaultAttack(string attackName, int damage)
    {
        Attack newAttack = gameObject.AddComponent<Attack>();
        newAttack.Initialize(attackName, damage);
        learnedAttacks.Add(newAttack); // Add the default attack to learned attacks
    }

    public void LearnAttack(Attack newAttack)
    {
        if (learnedAttacks.Count < 3)
        {
            learnedAttacks.Add(newAttack);
        }
        else
        {
            Debug.Log("This pawn has already learned the maximum number of attacks.");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public string attackName;
    public int damage;

    // Use this method to initialize the attack
    public void Initialize(string name, int dmg)
    {
        attackName = name;
        damage = dmg;
    }

    public void DealDamage(PlayerType target)
    {
        target.TakeDamage(damage);
    }
    // Method to assign possible attack options based on pawn type
    public static List<Attack> GetAvailableAttacks(PlayerType.Size type, GameObject pawn)
    {
        List<Attack> availableAttacks = new List<Attack>();

        switch (type)
        {
            case PlayerType.Size.Small:
                availableAttacks.Add(CreateAttack(pawn, "Jump", 2)); // Additional attack
                availableAttacks.Add(CreateAttack(pawn, "Run", 0)); // Additional attack
                break;

            case PlayerType.Size.Medium:
                availableAttacks.Add(CreateAttack(pawn, "Throw", 3)); // Additional attack
                availableAttacks.Add(CreateAttack(pawn, "Dash", 1)); // Additional attack
                break;

            case PlayerType.Size.Heavy:
                availableAttacks.Add(CreateAttack(pawn, "Stomp", 4)); // Additional attack
                availableAttacks.Add(CreateAttack(pawn, "Shield", 0)); // Additional attack
                break;

            default:
                Debug.LogError("Invalid pawn type.");
                break;
        }

        return availableAttacks;
    }

    // Helper method to create and initialize an attack
    private static Attack CreateAttack(GameObject pawn, string name, int dmg)
    {
        Attack newAttack = pawn.AddComponent<Attack>(); // Add Attack component to the pawn GameObject
        newAttack.Initialize(name, dmg); // Initialize the attack
        return newAttack;
    }
}
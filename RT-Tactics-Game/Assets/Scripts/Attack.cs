using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public string attackName;
    public int damage;

    private static Dictionary<GameObject, List<Attack>> pawnAttackDictionary = new Dictionary<GameObject, List<Attack>>();

    public void Initialize(string name, int dmg)
    {
        attackName = name;
        damage = dmg;
    }

    // Assign default attack
    public static void AssignDefaultAttack(string name, int damage, GameObject pawn)
    {
        Attack defaultAttack = pawn.AddComponent<Attack>();
        defaultAttack.Initialize(name, damage);
        
        if (!pawnAttackDictionary.ContainsKey(pawn))
        {
            pawnAttackDictionary[pawn] = new List<Attack> { defaultAttack };
        }
        else
        {
            pawnAttackDictionary[pawn].Add(defaultAttack);
        }
    }

    // Method to assign possible attack options based on pawn type
    public static void AssignAvailableAttacks(PlayerType.Size type, GameObject pawn)
    {
        List<Attack> availableAttacks = new List<Attack>();

        switch (type)
        {
            case PlayerType.Size.Small:
                availableAttacks.Add(CreateAttack(pawn, "Jump", 2));
                availableAttacks.Add(CreateAttack(pawn, "Run", 0));
                break;

            case PlayerType.Size.Medium:
                availableAttacks.Add(CreateAttack(pawn, "Throw", 3));
                availableAttacks.Add(CreateAttack(pawn, "Dash", 1));
                break;

            case PlayerType.Size.Heavy:
                availableAttacks.Add(CreateAttack(pawn, "Stomp", 4));
                availableAttacks.Add(CreateAttack(pawn, "Shield", 0));
                break;
        }

        // Store available attacks for the pawn
        if (!pawnAttackDictionary.ContainsKey(pawn))
        {
            pawnAttackDictionary[pawn] = availableAttacks;
        }
        else
        {
            pawnAttackDictionary[pawn].AddRange(availableAttacks);
        }
    }

    // Method for learning a new attack
    public static void LearnAttack(GameObject pawn, Attack newAttack)
    {
        if (pawnAttackDictionary.ContainsKey(pawn))
        {
            List<Attack> learnedAttacks = pawnAttackDictionary[pawn];

            if (learnedAttacks.Count < 3)
            {
                learnedAttacks.Add(newAttack);
                Debug.Log($"{newAttack.attackName} was learned!");
            }
            else
            {
                Debug.Log("This pawn has already learned the maximum number of attacks.");
            }
        }
    }

    public static List<Attack> GetPawnAttacks(GameObject pawn)
    {
        if (pawnAttackDictionary.ContainsKey(pawn))
        {
            return pawnAttackDictionary[pawn];
        }
        return new List<Attack>();
    }

    private static Attack CreateAttack(GameObject pawn, string name, int dmg)
    {
        Attack newAttack = pawn.AddComponent<Attack>();
        newAttack.Initialize(name, dmg);
        return newAttack;
    }
}

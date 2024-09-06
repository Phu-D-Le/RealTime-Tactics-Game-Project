using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerType.Size pawnType;
    private PlayerType pawnAttributes;

    private int turnsSinceLastLearn = 0; // Tracks turns since the last attack was learned
    private const int turnsRequiredToLearn = 3; // Number of turns required before learning a new attack

    void Start()
    {
        pawnAttributes = GetComponent<PlayerType>();

        if (pawnAttributes != null)
        {
            pawnAttributes.PawnStats(pawnType);
            Debug.Log($"Pawn Type: {pawnType}, Health: {pawnAttributes.Health}, Speed: {pawnAttributes.Speed}");
            DisplayAttacks();
        }
        else
        {
            Debug.LogError("PlayerType component not found on this GameObject.");
        }
    }

    void Update()
    {
        // Example turn progression
        if (Input.GetKeyDown(KeyCode.Space)) // Assume space bar advances the turn
        {
            EndTurn();
        }
    }

    void EndTurn()
    {
        turnsSinceLastLearn++;
        Debug.Log($"Turn {turnsSinceLastLearn} ended.");

        if (turnsSinceLastLearn >= turnsRequiredToLearn)
        {
            Debug.Log("This pawn can now learn a new attack!");
            turnsSinceLastLearn = 0; // Reset the turn counter after learning
            // Allow the player to choose a new attack here
            ChooseAttack();
        }
    }

    void ChooseAttack()
    {
        // For simplicity, this example auto-chooses the first available attack.
        // Replace this with actual player selection logic.

        if (pawnAttributes.availableAttacks.Count > 0)
        {
            Debug.Log("Available attacks:");
            for (int i = 0; i < pawnAttributes.availableAttacks.Count; i++)
            {
                Attack attack = pawnAttributes.availableAttacks[i];
                Debug.Log($"{i + 1}. {attack.attackName} ({attack.damage} damage)");
            }

            // Example: Assume player chooses the first attack (index 0)
            int selectedIndex = 0; // This should be replaced with actual player input

            if (selectedIndex >= 0 && selectedIndex < pawnAttributes.availableAttacks.Count)
            {
                Attack chosenAttack = pawnAttributes.availableAttacks[selectedIndex];
                pawnAttributes.LearnAttack(chosenAttack);
                pawnAttributes.availableAttacks.RemoveAt(selectedIndex); // Remove the chosen attack from available list
                Debug.Log($"{chosenAttack.attackName} was learned!");
                DisplayAttacks();
            }
            else
            {
                Debug.Log("Invalid attack selection.");
            }
        }
        else
        {
            Debug.Log("No attacks available to learn.");
        }
    }

    void DisplayAttacks()
    {
        Debug.Log("Learned Attacks:");
        foreach (var attack in pawnAttributes.learnedAttacks)
        {
            Debug.Log($"- {attack.attackName} ({attack.damage} damage)");
        }
    }
}

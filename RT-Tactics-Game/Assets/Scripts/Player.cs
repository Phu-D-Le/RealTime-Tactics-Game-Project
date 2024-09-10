using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerType.Size pawnType;
    private PlayerType pawnAttributes;
    public bool HasEndedTurn { get; private set; }
    public GameManager turn;

    public void Start()
    {
        pawnAttributes = GetComponent<PlayerType>();
        pawnAttributes.PawnStats(pawnType);

        // Assign available attacks to this pawn based on its type
        Attack.AssignAvailableAttacks(pawnType, this.gameObject);

        DisplayAttacks();
    }
    // Add the StartTurn method here
    public void StartTurn()
    {
        HasEndedTurn = false; // Reset the turn state
        Debug.Log("Player's turn started.");
    }
    public void EndTurn()
    {
        HasEndedTurn = true;
        ChooseAttack();
    }

    public void ChooseAttack()
    {
        List<Attack> availableAttacks = Attack.GetPawnAttacks(this.gameObject);

        if (availableAttacks.Count > 0)
        {
            int selectedIndex = 0; // Placeholder for player input

            if (selectedIndex >= 0 && selectedIndex < availableAttacks.Count)
            {
                Attack chosenAttack = availableAttacks[selectedIndex];
                Attack.LearnAttack(this.gameObject, chosenAttack);
                availableAttacks.RemoveAt(selectedIndex); // Remove from available attacks
                DisplayAttacks();
            }
        }
    }

    void DisplayAttacks()
    {
        List<Attack> learnedAttacks = Attack.GetPawnAttacks(this.gameObject);
        foreach (var attack in learnedAttacks)
        {
            Debug.Log($"{attack.attackName} ({attack.damage} damage)");
        }
    }
}

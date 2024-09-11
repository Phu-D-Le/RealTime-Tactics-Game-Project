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
        Debug.Log("Health: " + pawnAttributes.Health);

        AttackManager.InitializeDefaultAttack(gameObject, pawnType); // Initialize default attacks
        DisplayAttacks();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }
    }

    public void StartTurn(int currentTurn)
    {
        HasEndedTurn = false; // Reset the turn state
    }

    public void EndTurn()
    {
        HasEndedTurn = true;
        //turn.NotifyTurnEnd(this);
    }

    void DisplayAttacks()
    {
        List<Attack> learnedAttacks = AttackManager.GetLearnedAttacks(gameObject);
        foreach (var attack in learnedAttacks)
        {
            Debug.Log($"{attack.attackName} ({attack.damage} damage)");
        }
    }
}

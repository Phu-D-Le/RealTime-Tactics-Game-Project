using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PlayerTurn,
        EnemyTurn,
        GameOver
    }

    public GameState currentState;

    public Player firstPlayer; 
    public Player enemyPlayer; 
    private int playerTurns = 0; // Tracks total turns
    private int enemyTurns = 0;
    public AttackMenu attackMenu; // Reference to the AttackMenu script

    private void Start()
    {
        StartPlayerTurn();
    }

    private void Update()
    {
        if (currentState == GameState.GameOver) return;

        // Automatically switch between player and enemy based on turn end
        if (currentState == GameState.PlayerTurn && firstPlayer.HasEndedTurn)
        {
            StartEnemyTurn();
        }
        else if (currentState == GameState.EnemyTurn && enemyPlayer.HasEndedTurn)
        {
            StartPlayerTurn();
        }
    }

    public void NotifyTurnEnd(Player player)
    {
        if (player == firstPlayer)
        {
            if (playerTurns == 3 || playerTurns == 6)
            {
                attackMenu.UpdateMenu(firstPlayer, playerTurns); // Update menu based on turn
            }
        }

        if (player == enemyPlayer)
        {
            if (enemyTurns == 3 || enemyTurns == 6)
            {
                attackMenu.UpdateMenu(enemyPlayer, enemyTurns);
            }
        }
    }

    private void StartPlayerTurn()
    {
        currentState = GameState.PlayerTurn;
        playerTurns++;
        firstPlayer.StartTurn(playerTurns); // Pass the current turn count to the player
        enemyPlayer.EndTurn();
        attackMenu.UpdateMenu(firstPlayer, playerTurns);
        Debug.Log("Player's turn started.");
    }

    private void StartEnemyTurn()
    {
        currentState = GameState.EnemyTurn;
        enemyTurns++;
        enemyPlayer.StartTurn(enemyTurns); // Pass the current turn count to the enemy
        firstPlayer.EndTurn();
        attackMenu.UpdateMenu(enemyPlayer, enemyTurns);
        Debug.Log("Enemy's turn started.");
    }
}

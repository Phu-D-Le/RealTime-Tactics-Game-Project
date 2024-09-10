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
    private int turnsRequiredToLearn = 3; // Set when a pawn can learn a new attack

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
        if (player == firstPlayer && playerTurns >= turnsRequiredToLearn)
        {
            firstPlayer.ChooseAttack();
        }
        if (player == enemyPlayer && playerTurns >= turnsRequiredToLearn)
        {
            firstPlayer.ChooseAttack();
        }
    }

    private void StartPlayerTurn()
    {
        currentState = GameState.PlayerTurn;
        playerTurns++;
        firstPlayer.StartTurn();
        enemyPlayer.EndTurn();
        Debug.Log("Player's turn started.");
    }

    private void StartEnemyTurn()
    {
        currentState = GameState.EnemyTurn;
        enemyPlayer.StartTurn();
        firstPlayer.EndTurn();
        Debug.Log("Enemy's turn started.");
    }
}

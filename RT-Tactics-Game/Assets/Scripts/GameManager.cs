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
    public Player secondPlayer; 
    private Player currentPlayer;
    public DisplayInventory playerInventoryDisplay; // Reference to the player's inventory display
    public DisplayInventory attackMenuDisplay; // Reference to the attack menu display
    private int playerTurns = 0;
    private int enemyTurns = 0;
    private PawnObject selectedPawn; // Store the currently selected pawn

    private void Start()
    {
        StartEnemyTurn();
        ClearAttackMenu(); // Ensure attack menu is empty at the start
    }

    private void Update()
    {
        if (currentState == GameState.GameOver) return;

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            EndCurrentTurn();
            ClearAttackMenu();
        }
    }

    private void EndCurrentTurn()
    {
        if (currentState == GameState.PlayerTurn)
        {
            StartEnemyTurn();
        }
        else if (currentState == GameState.EnemyTurn)
        {
            StartPlayerTurn();
        }
    }

    private void StartPlayerTurn()
    {
        currentState = GameState.PlayerTurn;
        currentPlayer = firstPlayer; // Switch to the actual current player
        playerTurns++;
        Debug.Log($"Player's Turn {playerTurns} Started");


        // Update the UI for the current player's inventory
        if (currentPlayer != null)
        {
            // Update the player inventory display
            if (playerInventoryDisplay != null)
            {
                playerInventoryDisplay.UpdateMenu(currentPlayer.playerInventory);
            }
            currentPlayer.DisplayPawnsAndAttacks();
        }
    }

    private void StartEnemyTurn()
    {
        currentState = GameState.EnemyTurn;
        currentPlayer = secondPlayer; // Switch to the actual current player
        enemyTurns++;
        Debug.Log($"Enemy's Turn {enemyTurns} Started");

        // Update the UI for the current player's inventory
        if (currentPlayer != null)
        {
            // Update the player inventory display
            if (playerInventoryDisplay != null)
            {
                playerInventoryDisplay.UpdateMenu(currentPlayer.playerInventory);
            }
            currentPlayer.DisplayPawnsAndAttacks();
        }
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    private void ClearAttackMenu()
    {
        if (attackMenuDisplay != null)
        {
            attackMenuDisplay.ClearDisplay();
        }
    }
    public PawnObject GetSelectedPawn()
    {
        return selectedPawn;
    }
    public void SetSelectedPawn(PawnObject pawn)
    {
        selectedPawn = pawn;
        Debug.Log($"{pawn.name} is now the selected pawn.");
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject Player;
    public GameObject AI;
    private Player firstPlayer;
    private Player aiPlayer;
    public PawnHUD pawnHUD;
    public AttackHUD attackHUD;
    public TextMeshProUGUI turnDialogueText;
    public BattleState state;
    public TileMapManager tileMapManager;

    void Start()
    {
        state = BattleState.START;

        Player = GameObject.FindWithTag("Player");
        AI = GameObject.FindWithTag("AI");

        tileMapManager.GenerateTileMap();

        TileMapSpawner spawner = FindObjectOfType<TileMapSpawner>();
        spawner.InitializeSpawner();

        firstPlayer = Player.GetComponent<Player>();
        firstPlayer.playerName = "Player";
        firstPlayer.SetList();
        foreach (var pawn in firstPlayer.pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            currentPawn.AwakenPawn();
            currentPawn.isAIControlled = false;
        }

        aiPlayer = AI.GetComponent<Player>();
        aiPlayer.playerName = "AI";
        aiPlayer.SetList();
        foreach (var pawn in aiPlayer.pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            currentPawn.AwakenPawn();
            currentPawn.isAIControlled = true;
        }

        firstPlayer.SpawnPawnsOnMap(spawner);
        aiPlayer.SpawnPawnsOnMap(spawner);

        attackHUD.gameObject.SetActive(false);

        SetUpBattle();
    }

    void SetUpBattle()
    {
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        turnDialogueText.text = "Player's Turn!";
        pawnHUD.SetPlayerCanvas(firstPlayer);
        foreach (var pawn in firstPlayer.pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            currentPawn.ResetStatus();
        }
    }

    void EndPlayerTurn()
    {
        state = BattleState.ENEMYTURN;
        StartCoroutine(ExecuteEnemyTurn());
    }

    IEnumerator ExecuteEnemyTurn()
    {
        turnDialogueText.text = "AI's Turn!";
        pawnHUD.HidePlayerCanvas();

        foreach (var pawn in aiPlayer.pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            currentPawn.ResetStatus();

            if (IsPlayerInRange(currentPawn))
            {
                currentPawn.AttackNearestPlayer();
            }
            else
            {
                currentPawn.MoveTowardsPlayer(firstPlayer);
            }

            yield return new WaitForSeconds(1.0f);
        }

        EndEnemyTurn();
    }

    void EndEnemyTurn()
    {
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    public void Win(Player winner)
    {
        turnDialogueText.text = $"{winner.playerName} wins!";
        if (winner.playerName == "Player")
        {
            SceneManager.LoadScene("Gamedemo");
        }
        else
        {
            Destroy(Player);
            Destroy(AI);
            SceneManager.LoadScene("Test Menu");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && state == BattleState.PLAYERTURN)
        {
            attackHUD.gameObject.SetActive(false);
            EndPlayerTurn();
        }
    }

    private bool IsPlayerInRange(Pawn aiPawn)
    {
        foreach (var playerPawn in firstPlayer.pawns)
        {
            if (Vector3.Distance(aiPawn.transform.position, playerPawn.transform.position) <= aiPawn.attackRange)
            {
                return true;
            }
        }
        return false;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Start must be here as this controls game logic. Other objects should be on Awake(). This
// code serves as the game manager and switches from Player to Enemy (with space bar input) during the game
// and will also need more methods to handle future changes such as win/lose criteria.
// Logic here follows the states and is updated by the space bar. Further updates is within Pawn and HUD buttons. ZO

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}
public class BattleSystem : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;
    private Player firstPlayer;
    private Player enemyPlayer;
    public PawnHUD pawnHUD;
    public AttackHUD attackHUD;
    public TextMeshProUGUI turnDialogueText; // Set as Turn Display in PlayerUI. ZO
    public BattleState state;
    public TileMapManager tileMapManager;
    public SelectManager selectManager;

    void Start()
    {
        state = BattleState.START;

        Player = GameObject.FindWithTag("Player");
        Enemy = GameObject.FindWithTag("Enemy");

        tileMapManager.GenerateTileMap();

        TileMapSpawner spawner = FindObjectOfType<TileMapSpawner>();
        spawner.InitializeSpawner();

        firstPlayer = Player.GetComponent<Player>();
        firstPlayer.playerName = "Player";
        firstPlayer.SetList();
        foreach (var pawn in firstPlayer.pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            currentPawn.gameObject.tag = "PlayerPawn";
            currentPawn.AwakenPawn();
        }

        enemyPlayer = Enemy.GetComponent<Player>();
        enemyPlayer.playerName = "Enemy";
        enemyPlayer.SetList();
        foreach (var pawn in enemyPlayer.pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            currentPawn.gameObject.tag = "EnemyPawn";
            currentPawn.AwakenPawn();
        }

        firstPlayer.SpawnPawnsOnMap(spawner); // Spawner tag tiles must be in order within map. ZO
        enemyPlayer.SpawnPawnsOnMap(spawner);

        attackHUD.gameObject.SetActive(false);
        selectManager = FindObjectOfType<SelectManager>();

        SetUpBattle();
    }
    void SetUpBattle()
    {
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }
    void PlayerTurn()
    {
        turnDialogueText.text = "Player's Turn!";
        pawnHUD.SetPlayerCanvas(firstPlayer);
    }
    void PlayerAttack()
    {
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }
    void EnemyTurn()
    {
        turnDialogueText.text = "Enemy's Turn!";
        pawnHUD.SetPlayerCanvas(enemyPlayer);
    }
    void EnemyAttack()
    {
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
    public void Win(Player winner)
    {
        turnDialogueText.text = ($"{winner.playerName} wins!");
        if (winner.playerName == "Player")
        {
            SceneManager.LoadScene("Gamedemo");
        }
        else
        {
            Destroy(Player);
            Destroy(Enemy);
            SceneManager.LoadScene("Test Menu");
        }
    }
    // Space bar is turn ultimatum. Selection logic in SelectManager. ZO
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && selectManager.ready)
        {
            attackHUD.gameObject.SetActive(false);

            if (state == BattleState.PLAYERTURN)
            {
                PlayerAttack();
            }
            else if (state == BattleState.ENEMYTURN)
            {
                EnemyAttack();
            }
        }
    }
}


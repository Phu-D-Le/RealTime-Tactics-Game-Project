using TMPro;
using UnityEngine;

// Once we have references to the tiles, instantiate Pawn GameObjects either
// here or within Pawn. Just has to be a Start method where we can assign location. This
// code serves as the game manager and switches from Player to Enemy (with space) during the game
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

    //public Transform firstBattleStation; possible start of instantiation logic (junk). ZO
    //public Transform secondBattleStation;
    void Start()
    {
        state = BattleState.START;

        tileMapManager.GenerateTileMap();

        TileMapSpawner spawner = FindObjectOfType<TileMapSpawner>();
        spawner.InitializeSpawner();

        firstPlayer = Player.GetComponent<Player>();
        enemyPlayer = Enemy.GetComponent<Player>();

        firstPlayer.SpawnPawnsOnMap(spawner);
        // enemyPlayer.SpawnPawnsOnMap(spawner);

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
            currentPawn.ResetStatus();  // Reset flag for all pawns so they can attack again. ZO
        }
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
        foreach (var pawn in enemyPlayer.pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            currentPawn.ResetStatus();
        }
    }
    void EnemyAttack()
    {
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
    // Space bar is turn ultimatum. Otherwise turns are fairly binary. Need to implement
    // selection logic from the attack menu to select enemies and perhaps move automatically
    // when all pawns are done. ZO
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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


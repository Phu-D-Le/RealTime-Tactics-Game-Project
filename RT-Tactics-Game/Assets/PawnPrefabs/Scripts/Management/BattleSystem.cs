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
    public ActionHUD actionHUD;
    public TextMeshProUGUI turnDialogueText; // Set as Turn Display in PlayerUI. ZO
    public BattleState state;
    public TileMapManager tileMapManager;
    public SelectManager selectManager;

    //private bool playerHadTurn;
    //private bool enemyHadTurn;
    void Start()
    {
        state = BattleState.START;

        Player = GameObject.FindWithTag("Player");
        Enemy = GameObject.FindWithTag("Enemy");

        tileMapManager.GenerateTileMap();

        //playerHadTurn = false;
        //enemyHadTurn = false;

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
            currentPawn.team = 1;
        }

        enemyPlayer = Enemy.GetComponent<Player>();
        enemyPlayer.playerName = "Enemy";
        enemyPlayer.SetList();
        foreach (var pawn in enemyPlayer.pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            currentPawn.gameObject.tag = "EnemyPawn";
            currentPawn.AwakenPawn();
            currentPawn.team = 0;
        }

        firstPlayer.SpawnPawnsOnMap(spawner); // Spawner tag tiles must be in order within map. ZO
        enemyPlayer.SpawnPawnsOnMap(spawner);

        attackHUD.gameObject.SetActive(false);

        selectManager = FindObjectOfType<SelectManager>();

        actionHUD.gameObject.SetActive(false);


        SetUpBattle();
    }
    void SetUpBattle()
    {
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }
    void PlayerTurn()
    {
        pawnHUD.gameObject.SetActive(true);
        turnDialogueText.text = "Player 1's Turn!";
        pawnHUD.SetPlayerCanvas(firstPlayer);
    }
    void PlayerAttack()
    {
        state = BattleState.ENEMYTURN;
        //playerHadTurn = true;
        EnemyTurn();
    }
    void EnemyTurn()
    {
        pawnHUD.gameObject.SetActive(true);
        turnDialogueText.text = "Player 2's Turn!";
        pawnHUD.SetPlayerCanvas(enemyPlayer);
    }
    void EnemyAttack()
    {
        state = BattleState.PLAYERTURN;
        //enemyHadTurn = true;
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
    public void UpdateHUD()
    {
        attackHUD.gameObject.SetActive(false);
        actionHUD.gameObject.SetActive(false);

        if (state == BattleState.PLAYERTURN)
        {
            PlayerAttack();
        }
        else if (state == BattleState.ENEMYTURN)
        {
            EnemyAttack();
        }
    }
    public void ActionsPlaying()
    {
        turnDialogueText.text = "Actions Commencing...";
        pawnHUD.gameObject.SetActive(false);
        attackHUD.gameObject.SetActive(false);
        actionHUD.gameObject.SetActive(false);
    }
    //public void UpdateHUD2()
    //{

        //attackHUD.gameObject.SetActive(false);

        //if (state == BattleState.PLAYERTURN)
        //{
        //    PlayerAttack();
        //}
        //else if (state == BattleState.ENEMYTURN)
        //{
        //    EnemyAttack();
        //}
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
            //attackHUD.gameObject.SetActive(false);
            
            //actionHUD.gameObject.SetActive(false);
            
                
            //pawnHUD.gameObject.SetActive(false);

            //if (state == BattleState.PLAYERTURN)
            //{
                //PlayerAttack();
              //  playerHadTurn = true;
            //}
           // else if (state == BattleState.ENEMYTURN)
           // {
            //    EnemyAttack();
          //      enemyHadTurn = true;
        //    }

      //  }
    //}
}


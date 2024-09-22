using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}
public class BattleSystem : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;

    private Player firstPlayer;
    private Player enemyPlayer;

    //public Transform firstBattleStation;
    //public Transform secondBattleStation;
    public BattleState state;
    public TextMeshProUGUI turnDialogueText;
    public PawnHUD playerHUD;
    public PawnHUD enemyHUD;
    public GameObject attackHUD;

    //public TextMeshProUGUI enemyDialogueText;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;

        firstPlayer = Player.GetComponent<Player>();
        enemyPlayer = Enemy.GetComponent<Player>();

        attackHUD.SetActive(false);

        SetUpBattle();
    }
    void SetUpBattle()
    {
        state = BattleState.PLAYERTURN;
        playerHUD.SetPlayerCanvas(firstPlayer);
        enemyHUD.SetPlayerCanvas(enemyPlayer);
        PlayerTurn();
    }

    void PlayerTurn()
    {
        turnDialogueText.text = "Player's Turn!";
        playerHUD.gameObject.SetActive(true);
        enemyHUD.gameObject.SetActive(false);
    }
    void PlayerAttack()
    {
        firstPlayer.TakeTurn();
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }
    void EnemyTurn()
    {
        turnDialogueText.text = "Enemy's Turn!";
        playerHUD.gameObject.SetActive(false);
        enemyHUD.gameObject.SetActive(true);
    }
    void EnemyAttack()
    {
        enemyPlayer.TakeTurn();
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            attackHUD.SetActive(false);

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


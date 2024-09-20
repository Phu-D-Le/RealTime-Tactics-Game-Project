using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    //public TextMeshProUGUI enemyDialogueText;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        SetUpBattle();
    }
    void SetUpBattle()
    {
        firstPlayer = Player.GetComponent<Player>();
        enemyPlayer = Enemy.GetComponent<Player>();

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        turnDialogueText.text = "Player's Turn!";
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


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class OldPlayer : MonoBehaviour
//{
//    public PlayerType.Size pawnType;
//    private PlayerType pawnAttributes;
//    public bool HasEndedTurn { get; private set; }
//    public GameManager turn;

 //   public int health;
  //  public void Start()
    //{
      //  pawnAttributes = GetComponent<PlayerType>();
        //pawnAttributes.PawnStats(pawnType);

        //health = pawnAttributes.Health; //added public health variable to Player, may need to be private

        //AttackManager.InitializeDefaultAttack(gameObject, pawnType); // Initialize default attacks
        //DisplayAttacks();
        //GetComponent<Health>().init();
    //}
    
    //void Update()
    //{
      //  if (Input.GetKeyDown(KeyCode.Space))
        //{
          //  EndTurn();
        //}
    //}

    //public void StartTurn(int currentTurn)
    //{
      //  HasEndedTurn = false; // Reset the turn state
    //}

    //public void EndTurn()
    //{
      //  HasEndedTurn = true;
        //turn.NotifyTurnEnd(this);

        //If pawn is dead, destroy pawn
        //if(CheckIsDead())
        //{
          //  Destroy(gameObject);
        //}
    //}

    //void DisplayAttacks()
    //{
      //  List<Attack> learnedAttacks = AttackManager.GetLearnedAttacks(gameObject);
        //foreach (var attack in learnedAttacks)
        //{
          //  Debug.Log($"{attack.attackName} ({attack.damage} damage)");
        //}
    //}

    //When called, display pawn's current health
    //void DisplayCurrentHealth()
    //{
      //  Debug.Log("Current HP: " + health);
    //}

    //Checks if pawn is eligable to continue
    //bool CheckIsDead()
    //{
      //  return health <= 0;
    //}
//}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallOfFire : MonoBehaviour
{
    //******************************************************************************************
    //
    // A lot of methods in this class needs to be called every frame, really needs a different approach
    //
    //******************************************************************************************

    private int damage = 1;
    public List<Pawn> hitPawns = new List<Pawn>();
    
    //private GlobalVariables variables;
    int duration = 3;
    int turn;
    public int startTurn;
    
    public int playerOrEnemy; // 1 is player, 0 is enemy

    public void AssignTeam(int team)
    {
        playerOrEnemy = team;
        startTurn = GlobalVariables.turns;
        turn = GlobalVariables.turns;
    }

    
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("lol");
        Pawn currentPawn = other.gameObject.GetComponentInParent<Pawn>();
        //Debug.Log($"{currentPawn.pawnName} hit by fire");
        if (currentPawn != null && !hitPawns.Contains(currentPawn))   //prevents pawns from getting hit multiple times in 1 turn and makes sure only pawns get hit
        {
            currentPawn.TakeDamage(damage);
            hitPawns.Add(currentPawn);
            Debug.Log($"{currentPawn.pawnName} hit by fire");
        }
    }
    
    public void Update()
    {

        if (startTurn + duration == GlobalVariables.turns)
        {
            Destroy(gameObject);
        }
        if(turn < GlobalVariables.turns)
        {
            turn = GlobalVariables.turns;
            EmptyStack();
        }
    }
    public void EmptyStack()
    {
        hitPawns.Clear();
    }
    
}

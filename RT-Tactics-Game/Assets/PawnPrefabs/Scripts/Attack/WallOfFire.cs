using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfFire : MonoBehaviour
{
    private int damage = 1;
    public List<Pawn> hitPawns = new List<Pawn>();
    //private GlobalVariables variables;
    int duration = 2;
    int turn;
    
    public int playerOrEnemy; // 1 is player, 0 is enemy

    public void AssignTeam(int team)
    {
        playerOrEnemy = team;
        turn = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        Pawn currentPawn = other.gameObject.GetComponentInParent<Pawn>();
        if (currentPawn != null && !hitPawns.Contains(currentPawn))   //prevents pawns from getting hit multiple times in 1 turn and makes sure only pawns get hit
        {
            currentPawn.TakeDamage(damage);
            hitPawns.Add(currentPawn);
            Debug.Log($"{currentPawn.pawnName} hit by fire");
        }
    }

    public void UpdateTurn()
    {
        turn++;
        if(turn >= duration)
        {
            Destroy(gameObject);
        }
    }
    public void EmptyStack()
    {
        hitPawns.Clear();
    }
    
}

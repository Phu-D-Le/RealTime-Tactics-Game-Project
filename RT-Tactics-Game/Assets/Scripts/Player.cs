using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Add BattleState for each pawn within the Player children so foreach pawn can index each pawn available and then we run through valid game states? Handle death later.
// Also need to figure out a way to populate a PawnPanel with the respective sprites. Maybe GameManager should handle logic and call list from Player

public class Player : MonoBehaviour
{
    public List<GameObject> pawns = new List<GameObject>();

    void Start()
    {
        // Assuming pawns are child objects of this player object
        for (int i = 0; i < transform.childCount; i++)
        {
            pawns.Add(transform.GetChild(i).gameObject);
        }

    }

    public void TakeTurn()
    {
        foreach (var pawn in pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            if (currentPawn != null)
            {
                currentPawn.Attack(); // Or other action logic
            }
        }
    }
}
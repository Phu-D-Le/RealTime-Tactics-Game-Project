using System.Collections.Generic;
using UnityEngine;

// Player finds all of its children and assigns them to the pawns list so long as
// they are active. TakeTurn is called in GameManager and is a filler method for turn recognition. ZO

public class Player : MonoBehaviour
{
    public List<GameObject> pawns;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (child.activeSelf)
            {
                pawns.Add(child);  // Add only active pawns to player pawns list. ZO
            }
        }
    }
    public void SpawnPawnsOnMap(TileMapSpawner spawner)
    {
        foreach (GameObject pawn in pawns)
        {
            spawner.SpawnPawn(pawn);
        }
    }
    public void RemovePawn(Pawn pawn) // Handle Death. ZO
    {
        GameObject pawnObject = pawn.gameObject;
        if (pawns.Contains(pawnObject))
        {
            pawns.Remove(pawnObject);
            Destroy(pawnObject);  // Optional destroy. Maybe revive feature later? ZO
            Debug.Log($"{pawn.pawnName} has been removed from the player's list.");
        }
    }

    public void TakeTurn() // Used to use this in BattleSystem. Null now that buttons serve almost all functions. ZO
    {
        foreach (var pawn in pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            if (currentPawn != null)
            {
                currentPawn.ResetStatus();
            }
        }
    }
}
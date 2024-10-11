using System.Collections.Generic;
using UnityEngine;

// Player finds all of its children and assigns them to the pawns list so long as
// they are active. Call Spawner to initialize all player's pawns location. GameObject pawns
// as their components are not gotten/manipulated yet. ZO

public class Player : MonoBehaviour
{
    public List<GameObject> pawns;
    public string playerName { get; set; }

    void Awake()
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
}
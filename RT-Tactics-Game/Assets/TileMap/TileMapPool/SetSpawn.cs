using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawn pawns and declare location. I left original code that I saw when I began coding here except I made 
// all the Spawner tiles to be found and added to a list. It is crucial the Spawner tiles are found in an 
// order since player will immediately assign first 3 spawners it finds in list. Set pawn's current tile for 
// SelectManager reference later. ZO
public class TileMapSpawner : MonoBehaviour
{
    //public GameObject pawnPrefab;
    public float heightOffset = 1.0f;
    public string spawnTag = "Spawner";
    private List<GameObject> availableSpawnTiles;

    public void InitializeSpawner()
    {
        availableSpawnTiles = new List<GameObject>(GameObject.FindGameObjectsWithTag(spawnTag));

        if (availableSpawnTiles.Count == 0)
        {
            Debug.LogError("No spawn tiles found! Check if tiles are tagged with 'Spawner'.");
        }
    }

    public void SpawnPawn(GameObject pawn)
    {
        if (availableSpawnTiles == null || availableSpawnTiles.Count == 0)
        {
            Debug.LogWarning("No available spawn tiles left.");
            return;
        }
        // GameObject[] spawnTiles = GameObject.FindGameObjectsWithTag(spawnTag);

        // Pick the first available tile, remove it from the list
        GameObject tile = availableSpawnTiles[0];
        availableSpawnTiles.RemoveAt(0);

        // Spawn pawn on the tile
        Vector3 movePosition = tile.transform.position + new Vector3(0, 1, 0);
        pawn.transform.position = movePosition;

        //foreach (GameObject tile in spawnTiles)
        //{
            // spawn pawn on top of the tile
           // Vector3 spawnPosition = tile.transform.position + new Vector3(0, heightOffset, 0);
            //Instantiate(pawn, spawnPosition, Quaternion.identity);
        //}
        Pawn pawnComponent = pawn.GetComponent<Pawn>();
        if (pawnComponent != null)
        {
            pawnComponent.CurrentTile = tile;
        }
    }
}
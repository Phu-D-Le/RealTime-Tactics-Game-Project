using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Vector3 movePosition = tile.transform.position + new Vector3(0, heightOffset, 0);
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
            pawnComponent.CurrentTile = tile;  // Assuming the Pawn class has a CurrentTile property
        }
    }
}
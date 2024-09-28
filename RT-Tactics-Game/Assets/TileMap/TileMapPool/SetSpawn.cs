using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapSpawner : MonoBehaviour
{
    public GameObject pawnPrefab;
    public float heightOffset = 1.0f;
    public string spawnTag = "Spawner";

    // Start is called before the first frame update
    void Start()
    {
        SpawnPawn();
    }

    void SpawnPawn()
    {
        GameObject[] spawnTiles = GameObject.FindGameObjectsWithTag(spawnTag);

        foreach (GameObject tile in spawnTiles)
        {
            // spawn pawn on top of the tile
            Vector3 spawnPosition = tile.transform.position + new Vector3(0, heightOffset, 0);
            Instantiate(pawnPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
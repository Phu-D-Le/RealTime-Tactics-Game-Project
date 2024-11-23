using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapManager : MonoBehaviour
{
    public GameObject[] TileMaps;
    
    void Start()
    {
        GenerateTileMap();
    }

    void GenerateTileMap()
    {
        Instantiate(TileMaps[Random.Range(0, TileMaps.Length)], transform.position, Quaternion.identity);
    }

    
}

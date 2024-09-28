using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapManager : MonoBehaviour
{
    public GameObject[] TileMaps; 
    public Camera mainCamera;

    void Start()
    {
        GenerateTileMap();
    }

    void GenerateTileMap()
    {
        //gen tile map
        GameObject tileMap = Instantiate(TileMaps[Random.Range(0, TileMaps.Length)]);
        Debug.Log("Tile Map: " + tileMap.name);
        CenterTileMap(tileMap);
    }

    void CenterTileMap(GameObject tileMap)
    {
        Bounds tileMapBounds = CalculateBounds(tileMap);
        Vector3 cameraCenter = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        cameraCenter.z = 0;
        Vector3 offset = cameraCenter - tileMapBounds.center;
        tileMap.transform.position += offset;
    }

    Bounds CalculateBounds(GameObject obj)
    {

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(obj.transform.position, Vector3.zero);
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }
}
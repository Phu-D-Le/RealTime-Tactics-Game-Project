using System.Collections.Generic;
using UnityEngine;

public class TileMapManager : MonoBehaviour
{
    public GameObject[] TileMaps;
    public Camera mainCamera;
    private HexGrid hexGrid;
    public SelectManager selectManager;

    // GenerateTileMap is called in BattleSystem. ZO
    // Updated tile maps to use correct tile prefabs, adjusted spawn points for pawns. JP

    public void GenerateTileMap()
    {
        // Generate a random tile map from the available prefabs
        GameObject tileMap = Instantiate(TileMaps[Random.Range(0, TileMaps.Length)]); // Random map pool
        Debug.Log("Tile Map: " + tileMap.name);
        CenterTileMap(tileMap);

        // Initialize the HexGrid
        hexGrid = GetComponent<HexGrid>();
        if (hexGrid == null)
        {
            Debug.LogError("HexGrid component not found on TileMapManager!");
        }
        else
        {
            hexGrid.StartHexGrid();
        }

        // Initialize the SelectManager
        selectManager = GetComponent<SelectManager>();
        if (selectManager == null)
        {
            Debug.LogError("SelectManager component not found on TileMapManager!");
        }
        else
        {
            selectManager.InitializeSelectManager(mainCamera, hexGrid);
        }
    }

    // Expose the HexGrid to other scripts
    public HexGrid GetHexGrid()
    {
        return hexGrid;
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

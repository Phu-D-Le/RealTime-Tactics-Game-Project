using System.Collections.Generic;
using UnityEngine;

public class TileMapManager : MonoBehaviour
{
    public static TileMapManager Instance { get; private set; }
    public GameObject[] TileMaps;
    public Camera mainCamera;
    private HexGrid hexGrid;
    public SelectManager selectManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void GenerateTileMap()
    {
        GameObject tileMap = Instantiate(TileMaps[Random.Range(0, TileMaps.Length)]);
        Debug.Log("Tile Map: " + tileMap.name);
        CenterTileMap(tileMap);

        hexGrid = GetComponent<HexGrid>();
        hexGrid.StartHexGrid();

        selectManager = GetComponent<SelectManager>();
        selectManager.InitializeSelectManager(mainCamera, hexGrid);
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

    public HexGrid GetHexGrid()
    {
        return hexGrid;
    }
}

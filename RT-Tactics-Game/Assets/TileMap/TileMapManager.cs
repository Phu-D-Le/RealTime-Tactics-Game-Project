using System.Collections.Generic;
using UnityEngine;

public class TileMapManager : MonoBehaviour
{
    public GameObject[] TileMaps; 
    public Camera mainCamera;
    private HexGrid hexGrid;
    List<Vector3Int> neighbours = new List<Vector3Int>();

    void Start()
    {
        // GenerateTileMap(); (Uncomment this line if you want to generate a tile map on start)
    }

    public void GenerateTileMap()
    {
        //gen tile map
        // GameObject tileMap = Instantiate(TileMaps[Random.Range(0, TileMaps.Length)]);
        GameObject tileMap = Instantiate(TileMaps[0]);
        Debug.Log("Tile Map: " + tileMap.name);
        CenterTileMap(tileMap);

        hexGrid = GetComponent<HexGrid>();
        hexGrid.StartHexGrid();
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

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // Check if ray hits any object
            {
                GameObject clickedTile = hit.collider.gameObject;
                Debug.Log($"Hit object: {clickedTile.name} with tag: {clickedTile.tag}");

                // Check if the clicked object is a tile
                if (clickedTile.CompareTag("Hazard") || 
                    clickedTile.CompareTag("Free") || 
                    clickedTile.CompareTag("Interactable") || 
                    clickedTile.CompareTag("Spawner"))
                {
                    Debug.Log($"Clicked on tile with tag: {clickedTile.tag}");

                    Hex selectedHex = clickedTile.GetComponent<Hex>();
                    selectedHex.DisableHighlight();

                    foreach(Vector3Int neighbour in neighbours)
                    {
                        hexGrid.GetTileAt(neighbour).DisableHighlight();
                    }

                    // Fetch the current pawn through PawnHUD
                    PawnHUD pawnHUD = FindObjectOfType<PawnHUD>();
                    Pawn currentPawn = pawnHUD.selectedPawn;

                    if (currentPawn != null && !currentPawn.hasMoved)
                    {
                        // neighbours = hexGrid.GetNeighboursFor(selectedHex.HexCoords);
                        BFSResult bfsResult = GraphSearch.BFSGetRange(hexGrid, selectedHex.HexCoords, currentPawn.pawnSpeed);
                        neighbours = new List<Vector3Int>(bfsResult.GetRangePositions());

                        foreach(Vector3Int neighbour in neighbours)
                        {
                            hexGrid.GetTileAt(neighbour).EnableHighlight();
                        }
                        Debug.Log($"Neighbours for {selectedHex.HexCoords} are:");
    
                        foreach (Vector3Int neighbourPos in neighbours)
                        {
                            Debug.Log(neighbourPos);
                        }

                        HandleTileClick(clickedTile);
                    }
                    else
                    {
                        Debug.Log("No pawn selected");
                    }
                }
                else
                {
                    Debug.Log("Clicked on a non-tile object.");
                }
            }
        }
    }

    private void HandleTileClick(GameObject clickedTile)
    {
        PawnHUD pawnHUD = FindObjectOfType<PawnHUD>();
        Pawn currentPawn = pawnHUD.selectedPawn;

        if (currentPawn != null && !currentPawn.hasMoved)
        {
            MovePawnToTile(currentPawn, clickedTile);
            currentPawn.Move();
        }
    }

    private void MovePawnToTile(Pawn pawn, GameObject targetTile)
    {
        // Move the pawn
        Vector3 newPosition = targetTile.transform.position + new Vector3(0, 2.0f, 0); // Adjust height if needed
        pawn.transform.position = newPosition;

        // Update the current tile reference
        pawn.CurrentTile = targetTile;
    }
}

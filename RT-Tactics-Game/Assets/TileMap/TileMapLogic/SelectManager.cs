using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    private Camera mainCamera;
    private HexGrid hexGrid;
    private List<Vector3Int> neighbours = new List<Vector3Int>();  // Store valid movement tiles

    private Pawn selectedPawn;  // Store the currently selected pawn
    private Vector3Int selectedTile; // Store the selected tile for movement
    private Pawn selectedTargetPawn; // Store the selected target pawn for attack

    public bool IsMoving { get; set; }
    public bool IsAttacking { get; set; }

    public void InitializeSelectManager(Camera camera, HexGrid grid)
    {
        mainCamera = camera;
        hexGrid = grid;
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // Check if ray hits any object
            {
                GameObject clickedTile = hit.collider.gameObject;
                Debug.Log($"Hit object: {clickedTile.name} with tag: {clickedTile.tag}");

                if (IsTile(clickedTile)) // Check if the clicked object is a tile
                {
                    HandleTileClick(clickedTile);
                }
                else
                {
                    Debug.Log("Clicked on a non-tile object.");
                }
            }
        }
    }

    private bool IsTile(GameObject clickedTile)
    {
        return clickedTile.CompareTag("Hazard") ||
               clickedTile.CompareTag("Free") ||
               clickedTile.CompareTag("Interactable") ||
               clickedTile.CompareTag("Spawner");
    }

    private void HandleTileClick(GameObject clickedTile)
{
    Hex hexComponent = clickedTile.GetComponent<Hex>();
    if (hexComponent != null)
    {
        // If the selected action is to move
        if (IsMoving)
        {
            // Check if the clicked tile is within the highlighted (valid) movement range
            if (IsTileInRange(hexComponent.HexCoords))
            {
                // Store the selected tile for movement
                selectedTile = hexComponent.HexCoords; 
                Debug.Log($"Selected tile for moving: {selectedTile}");

                // Retrieve the currently selected pawn from the PawnHUD
                PawnHUD pawnHUD = FindObjectOfType<PawnHUD>();
                Pawn currentPawn = pawnHUD.selectedPawn;

                // Check if the selected pawn exists and has not moved
                if (currentPawn != null && !currentPawn.hasMoved)
                {
                    // Move the pawn to the selected tile
                    MovePawnToTile(currentPawn, clickedTile);
                    currentPawn.Move(); // Call the Move method on the pawn (if needed)
                }
                else
                {
                    Debug.Log("No pawn selected to move or pawn has already moved.");
                }
            }
            else
            {
                Debug.Log("Clicked tile is not within movement range.");
            }
        }
        // If the selected action is to attack
        else if (IsAttacking)
        {
            // Check if the clicked tile contains an enemy pawn
            if (IsTileInRange(hexComponent.HexCoords))
            {
                // Check if there's an enemy pawn on this tile
                Pawn targetPawn = FindPawnOnTile(clickedTile); // Pass the clicked tile (GameObject)
                if (targetPawn != null)
                {
                    // Store the selected target pawn for the attack
                    selectedTargetPawn = targetPawn; 
                    Debug.Log($"Selected target pawn for attack: {selectedTargetPawn.pawnName}");
                }
                else
                {
                    Debug.Log("No enemy pawn on this tile.");
                }
            }
            else
            {
                Debug.Log("Clicked tile is not within attack range.");
            }
        }
    }
}


    private bool IsTileInRange(Vector3Int tileCoords)
    {
        // Check if the clicked tile's coordinates are in the list of valid (highlighted) tiles
        return neighbours.Contains(tileCoords);
    }

    private void MovePawnToTile(Pawn pawn, GameObject targetTile)
    {
        // Adjust height if needed, moving pawn to the new tile
        Vector3 newPosition = targetTile.transform.position + new Vector3(0, 2.0f, 0); 
        pawn.transform.position = newPosition;
        pawn.CurrentTile = targetTile;
        pawn.Move(); // Call pawn's Move function to finalize movement
    }

    public void HighlightTilesForPawn(Pawn pawn)
{
    // Clear any previous highlights
    DisableAllHighlights();

    selectedPawn = pawn;  // Set the selected pawn

    if (selectedPawn != null && !selectedPawn.hasMoved)
    {
        // Assume the pawn's current position is on a tile
        Hex currentHex = selectedPawn.CurrentTile.GetComponent<Hex>();

        if (currentHex != null)
        {
            // Highlight tiles in range of the selected pawn's movement
            BFSResult bfsResult = GraphSearch.BFSGetRange(hexGrid, currentHex.HexCoords, selectedPawn.pawnSpeed);
            neighbours = new List<Vector3Int>(bfsResult.GetRangePositions());

            foreach (Vector3Int neighbour in neighbours)
            {
                Hex tileHex = hexGrid.GetTileAt(neighbour);
                if (tileHex != null) // Ensure that the tile exists
                {
                    tileHex.EnableHighlight();
                }
            }
        }
    }
}
    private void DisableAllHighlights()
    {
        foreach (Vector3Int neighbour in neighbours)
        {
            hexGrid.GetTileAt(neighbour).DisableHighlight();
        }
        neighbours.Clear();  // Clear the neighbours list after disabling highlights
    }
    public void HighlightTilesForAttack(Pawn pawn, Attack attack)
{
    DisableAllHighlights(); // Clear previous highlights
    Hex currentHex = pawn.CurrentTile.GetComponent<Hex>();

    if (currentHex != null)
    {
        // Calculate the range based on the attack
        BFSResult bfsResult = GraphSearch.BFSGetRange(hexGrid, currentHex.HexCoords, attack.range);
        neighbours = new List<Vector3Int>(bfsResult.GetRangePositions());

        foreach (Vector3Int neighbour in neighbours)
        {
            Hex tileHex = hexGrid.GetTileAt(neighbour);
            if (tileHex != null) // Ensure that the tile exists
            {
                tileHex.EnableHighlight();
            }
        }
    }
}
    private Pawn FindPawnOnTile(GameObject tile)
    {
        Hex clickedHex = tile.GetComponent<Hex>();
        if (clickedHex == null) return null;

        // Get the coordinates of the clicked hex
        Vector3Int hexCoords = clickedHex.HexCoords;

        // Find all pawns in the game
        Pawn[] allPawns = FindObjectsOfType<Pawn>();

        // Check each pawn to see if it is on the clicked tile
        foreach (Pawn pawn in allPawns)
        {
            if (pawn.CurrentTile.GetComponent<Hex>().HexCoords == hexCoords)
            {
                return pawn; // Return the pawn found on the clicked tile
            }
        }
        
        return null; // No pawn found on this tile
    }
}
using System.Collections.Generic;
using UnityEngine;

// Very beefy and very likely to be ineffecient class. Handles clicking the map and all behaviours associated with activating
// the BFS. ZO
public class SelectManager : MonoBehaviour
{
    private Camera mainCamera;
    private HexGrid hexGrid;
    private List<Vector3Int> neighbours = new List<Vector3Int>();
    
    private Pawn selectedPawn;  
    private Vector3Int selectedTile; 
    private Pawn selectedTargetPawn; 
    public Attack selectedAttack;

    public bool IsMoving { get; set; }
    public bool IsAttacking { get; set; }

    public void InitializeSelectManager(Camera camera, HexGrid grid)
    {
        mainCamera = camera;
        hexGrid = grid;
    }
    void Update()
    {
        HandleInput();
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            DisableAllHighlights(); // Cheating way of not having to reference this in BattleSystem. ZO 
        }
    }

    private void HandleMouseClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // Mesh collider necessary to find tile. ZO
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clickedTile = hit.collider.gameObject;
            // Debug.Log($"Hit object: {clickedTile.name} with tag: {clickedTile.tag}");

            if (IsTile(clickedTile))
            {
                HandleTileClick(clickedTile);
            }
            else
            {
                Debug.Log("Clicked on a non-tile object."); // Like a player object. Objects on tile will have to be child? ZO
            }
        }
    }

    private bool IsTile(GameObject clickedTile) // Ensure tile has valid tag and cost within Hex for BFS to work. ZO
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
            if (IsMoving)
            {
                TryMovePawn(hexComponent, clickedTile);
            }
            else if (IsAttacking)
            {
                TryAttackPawn(hexComponent, clickedTile);
            }
        }
    }

    private void TryMovePawn(Hex hexComponent, GameObject clickedTile)
    {
        if (IsTileInRange(hexComponent.HexCoords) && FindPawnOnTile(clickedTile) == null) // Highlighted and no pawn is on tile. ZO
        {
            Pawn currentPawn = GetCurrentPawn();

            if (currentPawn != null && !currentPawn.hasMoved)
            {
                MovePawnToTile(currentPawn, clickedTile);
                currentPawn.Move();
                DisableAllHighlights();
            }
            else
            {
                Debug.Log("No pawn selected to move or pawn has already moved.");
            }
        }
        else
        {
            Debug.Log(FindPawnOnTile(clickedTile) != null ? "Cannot move to a tile that is occupied by another pawn." : "Clicked tile is not within movement range.");
        }
    }

    private void TryAttackPawn(Hex hexComponent, GameObject clickedTile)
    {
        if (IsTileInRange(hexComponent.HexCoords))
        {
            Pawn targetPawn = FindPawnOnTile(clickedTile);
            if (targetPawn != null)
            {
                Pawn currentPawn = GetCurrentPawn();
                if (currentPawn != null && !currentPawn.hasAttacked)
                {
                    currentPawn.DealAttack(selectedAttack, targetPawn);
                    currentPawn.Attack();
                    Debug.Log($"{currentPawn.pawnName} has attacked {targetPawn.pawnName}.");
                    DisableAllHighlights();

                    Player targetPlayer = targetPawn.GetComponentInParent<Player>(); // Win condition in BattleSystem once all pawns dead. ZO
                    if (targetPlayer.pawns.Count == 0)
                    {
                        Player currentPlayer = currentPawn.GetComponentInParent<Player>();
                        BattleSystem battleSystem = (BattleSystem)FindObjectOfType(typeof(BattleSystem));
                        if (battleSystem != null)
                        {
                            battleSystem.Win(currentPlayer);
                        }
                    }
                }
                else
                {
                    Debug.Log("No pawn selected to attack or pawn has already attacked.");
                }
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

    private Pawn GetCurrentPawn() // PawnHUD button selected pawn. ZO
    {
        PawnHUD pawnHUD = FindObjectOfType<PawnHUD>();
        return pawnHUD?.selectedPawn;
    }

    private bool IsTileInRange(Vector3Int tileCoords) // does BFS find a valid coordinate? ZO
    {
        return neighbours.Contains(tileCoords);
    }

    private void MovePawnToTile(Pawn pawn, GameObject targetTile) // Simple transform. ZO
    {
        Vector3 newPosition = targetTile.transform.position + new Vector3(0, 2.0f, 0); 
        pawn.transform.position = newPosition;
        pawn.CurrentTile = targetTile;
        pawn.Move();
    }

    public void HighlightTilesForPawn(Pawn pawn) // Shader highlights tiles, BFS finds valid neighbours. ZO
    {
        DisableAllHighlights();
        selectedPawn = pawn;  

        if (selectedPawn != null && !selectedPawn.hasMoved)
        {
            Hex currentHex = selectedPawn.CurrentTile.GetComponent<Hex>();

            if (currentHex != null)
            {
                BFSResult bfsResult = GraphSearch.BFSGetRange(hexGrid, currentHex.HexCoords, selectedPawn.pawnSpeed);
                neighbours = new List<Vector3Int>(bfsResult.GetRangePositions());
                neighbours.Remove(currentHex.HexCoords); // Exclude the current tile. ZO

                HighlightValidTiles();
            }
        }
    }

    public void HighlightTilesForAttack(Pawn pawn, Attack attack) // Ensures all tiles are highlighted. including if a pawn is on it. ZO
    {
        DisableAllHighlights(); 
        Hex currentHex = pawn.CurrentTile.GetComponent<Hex>();

        if (currentHex != null)
        {
            BFSResult bfsResult = GraphSearch.BFSGetRange(hexGrid, currentHex.HexCoords, attack.range);
            neighbours = new List<Vector3Int>(bfsResult.GetRangePositions());
            neighbours.Remove(currentHex.HexCoords);

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

    private void HighlightValidTiles()
    {
        foreach (Vector3Int neighbour in neighbours)
        {
            Hex tileHex = hexGrid.GetTileAt(neighbour);
            if (tileHex != null && FindPawnOnTile(tileHex.gameObject) == null) // Ensure tile exists and is unoccupied
            {
                tileHex.EnableHighlight();
            }
        }
    }

    private void DisableAllHighlights() // Reset. ZO
    {
        foreach (Vector3Int neighbour in neighbours)
        {
            hexGrid.GetTileAt(neighbour)?.DisableHighlight();
        }
        neighbours.Clear();  
    }

    private Pawn FindPawnOnTile(GameObject tile)
    {
        Hex clickedHex = tile.GetComponent<Hex>();
        if (clickedHex == null) return null;

        Vector3Int hexCoords = clickedHex.HexCoords;
        Pawn[] allPawns = FindObjectsOfType<Pawn>();

        foreach (Pawn pawn in allPawns)
        {
            if (pawn.CurrentTile.GetComponent<Hex>().HexCoords == hexCoords)
            {
                return pawn; 
            }
        }
        return null; 
    }
}

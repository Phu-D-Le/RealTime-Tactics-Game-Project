using System.Collections.Generic; 
using UnityEngine;
using System.Collections;
using System.Linq;

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

    public bool isMoving { get; set; }
    public bool isAttacking { get; set; }
    public bool ready {get; set; }

    private List<Action> actionQueue = new List<Action>();
    private HashSet<Vector3Int> occupiedTiles = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> plannedTiles = new HashSet<Vector3Int>();

    void Start()
    {
        ready = true;
    }
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
        if (Input.GetMouseButtonDown(0) && ready == true)
        {
            HandleMouseClick();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            DisableAllHighlights(); // These are failsafes so player cannot select or move on enemy turn. ZO
            ExecuteActions();
        }
    }
    private void HandleMouseClick() // Check if mouse hits tile. ZO
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
    private void HandleTileClick(GameObject clickedTile) // Now tile has been clicked. ismoving/isattacking
    // is set by the pawnHUD/attackHUD to tell selectmanager what route to take in terms of the menu. ZO
    {
        Hex hexComponent = clickedTile.GetComponent<Hex>();
        if (hexComponent != null)
        {
            if (isMoving)
            {
                TryMovePawn(hexComponent, clickedTile);
            }
            else if (isAttacking)
            {
                TryAttackPawn(hexComponent, clickedTile);
            }
        }
    }
    public void SetMovementMode(bool enabled) // Called by pawnHUD. ZO
    {
        isMoving = enabled;
        isAttacking = !enabled;
        DisableAllHighlights();
    }
    public void SetAttackMode(bool enabled)
    {
        isAttacking = enabled;
        isMoving = !enabled;
        DisableAllHighlights();
    }
    private void TryMovePawn(Hex hexComponent, GameObject clickedTile) // Handles the pawn selected from pawnHUD, 
    // pawnHUD selects pawn and already commits BFS and highlighting valid tiles, now we queue up selected movement
    // and a plannedTile that the pawn wants to move to. ZO
    {
        Vector3Int targetCoords = hexComponent.HexCoords;

        if (IsTileInRange(hexComponent.HexCoords) && FindPawnOnTile(clickedTile) == null) // Highlighted and no pawn is on tile. ZO
        {
            Pawn currentPawn = GetCurrentPawn();

            if (currentPawn != null && !currentPawn.hasMoved)
            {
                occupiedTiles.Remove(currentPawn.CurrentTile.GetComponent<Hex>().HexCoords); // Remove current tile. ZO
                actionQueue.Add(new Action(ActionType.Move, currentPawn, targetCoords)); // Queue up movement. ZO
                plannedTiles.Add(targetCoords);
                currentPawn.Move(); // set hasMoved to true so pawn cant move again. ZO
                occupiedTiles.Add(targetCoords); // do not let other pawns move here on same turn. ZO
                DisableAllHighlights(); // move done. ZO
                Debug.Log($"{currentPawn.pawnName} queued to move.");
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
    private void TryAttackPawn(Hex hexComponent, GameObject clickedTile) // AttackHUD has done our BFS and highlighting so now
    // we add our attacks to a list and associate an attack to each pawn (could use another list but eh). ZO
    {
        if (IsTileInRange(hexComponent.HexCoords))
        {
            Pawn targetPawn = FindPawnOnTile(clickedTile);

            if (targetPawn != null) // Pawn must be on tile for an attack to work. Perhaps set no friendly fire here? ZO && !targetpawn.hasMoved
            { // Keep friendly fire but pawn cannot attack a tile where a pawn has moved from. ZO

                Pawn currentPawn = GetCurrentPawn();
                if (currentPawn != null && !currentPawn.hasAttacked)
                {
                    if (selectedAttack != null) // Ensure the selected attack is assigned before queuing the attack. ZO
                    {
                        actionQueue.Add(new Action(ActionType.Attack, currentPawn, targetPawn, selectedAttack));
                        currentPawn.Attack(); // hasAttacked = true. ZO
                        DisableAllHighlights();
                        Debug.Log($"{currentPawn.pawnName} queued to attack {targetPawn.pawnName} for {selectedAttack} dealing {selectedAttack.damage} damage.");
                    }
                    else
                    {
                        Debug.LogError("No attack selected for the current pawn.");
                    }
                }
                else
                {
                    Debug.Log("No pawn selected to attack or pawn has already attacked.");
                }
            }
            else if(targetPawn.hasMoved && plannedTiles.Contains(hexComponent.HexCoords)) // Pawn can attack a tile where a pawn moves to. ZO
            {
                Pawn currentPawn = GetCurrentPawn();
                if (currentPawn != null && !currentPawn.hasAttacked)
                {
                    // Ensure the selected attack is assigned before queuing the attack
                    if (selectedAttack != null)
                    {
                        actionQueue.Add(new Action(ActionType.Attack, currentPawn, targetPawn, selectedAttack));
                        currentPawn.Attack(); // hasAttacked = true. ZO
                        DisableAllHighlights();
                    }
                    else
                    {
                        Debug.LogError("No attack selected for the current pawn.");
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
    private IEnumerator MovePawnToTile(Pawn pawn, GameObject targetTile) // Physical movement called once dequeueing.
    // essentially redo the BFS but since tile is valid we can find our path here.
    {
        Hex targetHex = targetTile.GetComponent<Hex>();
        Hex currentHex = pawn.CurrentTile.GetComponent<Hex>();

        if (targetHex != null && currentHex != null)
        {
            BFSResult bfsResult = GraphSearch.BFSGetRange(hexGrid, currentHex.HexCoords, pawn.pawnSpeed);

            if (bfsResult.IsHexPositionInRange(targetHex.HexCoords))
            {
                List<Vector3Int> path = GraphSearch.GetExactPathToDestination(targetHex.HexCoords, bfsResult.visitedNodesDict);
                List<GameObject> pathTiles = path.Select(hexCoords => hexGrid.GetTileAt(hexCoords).gameObject).ToList();
                // reuse MoveAlongPath but now each coordinate is a gameObject put into a list that the path can be
                // traversed. ZO
                yield return StartCoroutine(MoveAlongPath(pawn, pathTiles)); // Ensure path movement happens step by step. ZO
            }
            else
            {
                Debug.Log("Target tile is not within movement range.");
            }
        }
        else
        {
            Debug.Log("Current or target tile is invalid.");
        }
    }
    private IEnumerator MoveAlongPath(Pawn pawn, List<GameObject> path) // Now the pawn can actually move. ZO
    {
        foreach (GameObject tile in path)
        {
            // Calculate how pawn will rotate. y is zero so it wont face down/up.  ZO
            Vector3 direction = (tile.transform.position - pawn.transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Have the pawn face where it goes to each tile. ZO
            while (Quaternion.Angle(pawn.transform.rotation, lookRotation) > 0.1f)
            {
                pawn.transform.rotation = Quaternion.Slerp(pawn.transform.rotation, lookRotation, Time.deltaTime * 5f);
                yield return null;
            }

            // Speed of movement is here, can increase/decrease as necessary. ZO
            Vector3 targetPosition = tile.transform.position + new Vector3(0, 2.0f, 0);
            while (Vector3.Distance(pawn.transform.position, targetPosition) > 0.1f)
            {
                pawn.transform.position = Vector3.Lerp(pawn.transform.position, targetPosition, Time.deltaTime * 3f);
                yield return null;
            }

            pawn.transform.position = targetPosition;
        }

        pawn.CurrentTile = path.Last(); // Pawn has moved. we would have to move this if we want all actions at once. ZO
        pawn.Move();
        DisableAllHighlights();
        //occupiedTiles.Add(pawn.CurrentTile.GetComponent<Hex>().HexCoords);
    }
    public void HighlightTilesForPawn(Pawn pawn) // Shader highlights tiles, BFS finds valid neighbours. 
    // called by pawnHUD once pawn is selected and current phase is MovePhase. ZO
    {
        selectedPawn = pawn;  

        if (selectedPawn != null && !selectedPawn.hasMoved)
        {
            Hex currentHex = selectedPawn.CurrentTile.GetComponent<Hex>();

            if (currentHex != null)
            {
                BFSResult bfsResult = GraphSearch.BFSGetRange(hexGrid, currentHex.HexCoords, selectedPawn.pawnSpeed);
                neighbours = new List<Vector3Int>(bfsResult.GetRangePositions());
                neighbours.Remove(currentHex.HexCoords); // Exclude the current tile. ZO

                foreach (Vector3Int neighbour in neighbours) // Tiles not occupied by another from either queue or if a pawn is there. ZO
                {
                    if (!occupiedTiles.Contains(neighbour) && !plannedTiles.Contains(neighbour))
                    {
                        Hex tileHex = hexGrid.GetTileAt(neighbour);
                        if (tileHex != null && FindPawnOnTile(tileHex.gameObject) == null)
                        {
                            tileHex.EnableHighlight(Color.red);
                        }
                    }
                } 
            }
        }
    }
    public void HighlightTilesForAttack(Pawn pawn, Attack attack) // Ensures all tiles are highlighted. including if a pawn is on it. 
    // do not include self for highlighting for valid. ZO
    {
        Vector3Int currentTileCoords;

        // If pawn has a move planned, use the planned tile for attack calculations. ZO. ZO
        Action moveAction = actionQueue.Find(action => action.pawn == pawn && action.actionType == ActionType.Move);
        if (moveAction != null)
        {
            currentTileCoords = moveAction.targetTile;  // Use the planned tile. ZO
        }
        else
        {
            currentTileCoords = pawn.CurrentTile.GetComponent<Hex>().HexCoords; // Use the current tile. ZO
        }
        BFSResult bfsResult = GraphSearch.BFSGetRange(hexGrid, currentTileCoords, attack.range);
        neighbours = new List<Vector3Int>(bfsResult.GetRangePositions());
        neighbours.Remove(currentTileCoords);

        foreach (Vector3Int neighbour in neighbours)
        {
            Hex tileHex = hexGrid.GetTileAt(neighbour);
            if (tileHex != null)
            {
                tileHex.EnableHighlight(Color.red); // Available tiles in red. ZO
            }
        }
        foreach (Vector3Int plannedTile in plannedTiles)
        {
            if (plannedTile != currentTileCoords) // Skip the current tile. ZO
            {
                Hex plannedTileHex = hexGrid.GetTileAt(plannedTile);
                if (plannedTileHex != null)
                {
                    plannedTileHex.EnableHighlight(Color.blue); // Allies highlighted in blue. ZO
                }
            }
            else
            {
                Hex plannedTileHex = hexGrid.GetTileAt(plannedTile);
                if (plannedTileHex != null)
                {
                    plannedTileHex.EnableHighlight(Color.white); // self highlighted in white. ZO
                }
            }
        }
    }
    private void DisableAllHighlights() // Reset highlight and bfs but not planned tiles (only at end do we reset). ZO
    {
        foreach (Vector3Int neighbour in neighbours)
        {
            if (!plannedTiles.Contains(neighbour))
            {
                hexGrid.GetTileAt(neighbour)?.DisableHighlight();
            }
        }
        neighbours.Clear();  
        HighlightPlannedTiles();
    }
    private Pawn FindPawnOnTile(GameObject tile) // simply find if a pawn is there or not. ZO
    {
        Hex clickedHex = tile.GetComponent<Hex>();
        if (clickedHex == null) return null;

        Vector3Int hexCoords = clickedHex.HexCoords;

        // First check planned tiles. ZO
        foreach (Action action in actionQueue)
        {
            if (action.actionType == ActionType.Move && action.targetTile == hexCoords)
            {
                return action.pawn;  // Treat the pawn as if it's on the planned tile. ZO
            }
        }
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
    private void ExecuteActions()
    {
        if (ready)
        {
            ready = false;
            StartCoroutine(ExecuteActionsSequentially());
        }
    }
    private IEnumerator ExecuteActionsSequentially() // Change to IEnumerator to handle coroutines sequentially. ZO
    {
        foreach (Action action in actionQueue)
        {
            if (action.actionType == ActionType.Move)
            {
                // Start the move coroutine and wait until it finishes. ZO
                yield return StartCoroutine(MovePawnToTile(action.pawn, hexGrid.GetTileAt(action.targetTile).gameObject));
            }
            else if (action.actionType == ActionType.Attack)
            {
                if (action.selectedAttack != null)
                {
                    action.pawn.DealAttack(action.selectedAttack, action.targetPawn);
                    Debug.Log($"{action.pawn.pawnName} attacks {action.targetPawn.pawnName}.");
                }
                else
                {
                    Debug.LogError("attack is null during attack execution.");
                }
            }
        }

        // Clear the queue after executing all actions
        actionQueue.Clear();
        occupiedTiles.Clear();
        DisablePlannedTileHighlights();
        ready = true;
    }
    private void HighlightPlannedTiles() // Continuously highlight planned tiles in green.
    {
        foreach (Vector3Int plannedTile in plannedTiles)
        {
            Hex tileHex = hexGrid.GetTileAt(plannedTile);
            if (tileHex != null)
            {
                tileHex.EnableHighlight(Color.green); // Highlight planned tiles in green
            }
        }
    }
    private void DisablePlannedTileHighlights()
    {
        foreach (Vector3Int plannedTile in plannedTiles)
        {
            hexGrid.GetTileAt(plannedTile)?.DisableHighlight(); // Disable highlight on each planned tile
        }
        plannedTiles.Clear(); // Clear the plannedTiles after disabling
    }
}
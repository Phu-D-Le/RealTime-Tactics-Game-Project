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
    private HashSet<Vector3Int> plannedTiles = new HashSet<Vector3Int>();

    private bool isSecondPlayerTurn;
    private Hex highlightedTile;
    public float movementSpeed = 7f; // Change the pawns movement speed here. ZO
    public float rotationSpeed = 7f; // Change the pawns rotation speed here. ZO

    void Start()
    {
        ready = true;
        isSecondPlayerTurn = false;
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
            if (isSecondPlayerTurn)
            {
                isSecondPlayerTurn = false;
                DisableAllHighlights(); // These are failsafes so player cannot select or move on enemy turn. ZO
                ExecuteActions();
            }
            else
            {
                isSecondPlayerTurn = true;
                DisableAllHighlights();
            }
        }
    }
    private void HandleMouseClick() // Check if mouse hits tile. ZO
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // Mesh collider necessary to find tile. ZO
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clickedTile = hit.collider.gameObject;

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
    public void SetAttackMode(bool enabled) // Called by attackHUD. ZO
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
        Pawn currentPawn = GetCurrentPawn();

        if (IsTileInRange(hexComponent.HexCoords)) // Highlighted and no pawn is on tile. ZO
        {
            if (plannedTiles.Contains(targetCoords))
            {
                Debug.Log("Cannot move to a planned tile.");
                return; // Movement is not successful, exit the method
            }
            Pawn pawnOnTile = FindPawnOnTile(clickedTile);
            if (pawnOnTile == null)
            {
                // Allow movement if the current pawn hasn't moved yet
                if (currentPawn != null && !currentPawn.hasMoved)
                {
                    actionQueue.Add(new Action(ActionType.Move, currentPawn, targetCoords));
                    plannedTiles.Add(targetCoords); // Add the planned tile
                    currentPawn.Move(); // Move the pawn
                    DisableAllHighlights(); // Move done
                    Debug.Log($"{currentPawn.pawnName} queued to move.");
                }
                else
                {
                    Debug.Log("No pawn selected to move or pawn has already moved.");
                }
            }
            else // If there is a pawn on the clicked tile
            {
                // Check if the pawn on the tile has moved
                if (pawnOnTile.hasMoved)
                {
                    // Allow movement if the other pawn has already moved
                    actionQueue.Add(new Action(ActionType.Move, currentPawn, targetCoords));
                    plannedTiles.Add(targetCoords);
                    currentPawn.Move(); // Move the pawn
                    DisableAllHighlights(); // Move done
                    Debug.Log($"{currentPawn.pawnName} queued to move to a tile occupied by a pawn that has already moved.");
                }
                else
                {
                    // Cannot move to a tile occupied by another pawn that hasn't moved
                    Debug.Log("Cannot move to a tile that is occupied by another pawn that has not moved.");
                }
            }
        }
        else
        {
            Debug.Log("Clicked tile is not within movement range.");
        }
    }
    private void TryAttackPawn(Hex hexComponent, GameObject clickedTile) // AttackHUD has done our BFS and highlighting so now
    // we add our attacks to a list and associate an attack to each pawn (could use another list but eh). ZO
    {
        if (IsTileInRange(hexComponent.HexCoords))
        {
            Pawn targetPawn = FindPawnOnTile(clickedTile);

            if (targetPawn != null && !targetPawn.hasMoved) // Pawn must be on tile for an attack to work. Perhaps set no friendly fire here? ZO && !targetpawn.hasMoved
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
            else if(targetPawn != null && targetPawn.hasMoved && plannedTiles.Contains(hexComponent.HexCoords)) // Pawn can attack a tile where a pawn moves to. ZO && plannedTiles.Contains(hexComponent.HexCoords
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
                pawn.transform.rotation = Quaternion.Slerp(pawn.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                yield return null;
            }

            // Speed of movement is here, can increase/decrease as necessary. ZO
            Vector3 targetPosition = tile.transform.position + new Vector3(0, 2.0f, 0);
            while (Vector3.Distance(pawn.transform.position, targetPosition) > 0.1f)
            {
                pawn.transform.position = Vector3.Lerp(pawn.transform.position, targetPosition, Time.deltaTime * movementSpeed);
                yield return null;
            }

            pawn.transform.position = targetPosition;
        }

        pawn.CurrentTile = path.Last(); // Pawn has moved. we would have to move this if we want all actions at once. ZO
        pawn.Move();
        DisableAllHighlights();
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
                    if (!plannedTiles.Contains(neighbour))
                    {
                        Hex tileHex = hexGrid.GetTileAt(neighbour);
                        if (tileHex != null && FindPawnOnTile(tileHex.gameObject) == null) 
                        {
                            tileHex.EnableHighlight(Color.green);
                        }
                        else if(tileHex != null)
                        {
                            Pawn emptyPawn = FindPawnOnTile(tileHex.gameObject);
                            if (emptyPawn.hasMoved)
                            {
                                tileHex.EnableHighlight(Color.green);
                            }
                        }
                    }
                }
                highlightedTile = currentHex; // Store the current tile being highlighted in white
                highlightedTile.EnableHighlight(Color.white); 
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

        // Highlight the current tile in white.
        Hex currentTileHex = hexGrid.GetTileAt(currentTileCoords);
        if (currentTileHex != null)
        {
            highlightedTile = currentTileHex;
            currentTileHex.EnableHighlight(Color.white); // Highlight current tile in white.
        } 

        foreach (Vector3Int neighbour in neighbours)
        {
            Hex tileHex = hexGrid.GetTileAt(neighbour);
            if (tileHex != null)
            {
                Pawn pawnOnTile = FindPawnOnTile(tileHex.gameObject);
                if (pawnOnTile != null)
                {
                    if (pawnOnTile.hasMoved && !plannedTiles.Contains(tileHex.HexCoords))
                    {
                        continue;
                    }
                    // Check the parent GameObject tag of the pawn on the tile
                    string parentTag = pawnOnTile.transform.parent.tag;
                    string currentTag = pawn.transform.parent.tag;
                
                    // Apply different highlight colors based on the parent's tag
                    if (currentTag != parentTag)
                    {
                        tileHex.EnableHighlight(Color.red);  // Enemy tile in red
                    }
                    else if (parentTag == currentTag)
                    {
                        tileHex.EnableHighlight(Color.blue);  // Ally tile in blue
                    }
                }
                else
                {
                    tileHex.EnableHighlight(Color.green);  // blank tiles are green. show range
                }
            }
        }
    }
    private void DisableAllHighlights() // Reset highlight and bfs but not planned tiles (only at end do we reset). ZO
    {
        // Clear highlight on the current tile if it exists
        if (highlightedTile != null)
        {
            highlightedTile.DisableHighlight(); // Clear white highlight
            highlightedTile = null; // Reset for next use
        }
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
        StartNewTurn();
    }
    private void HighlightPlannedTiles() // Continuously highlight planned tiles in yellow.
    {
        foreach (Vector3Int plannedTile in plannedTiles)
        {
            Hex tileHex = hexGrid.GetTileAt(plannedTile);
            if (tileHex != null)
            {
                tileHex.EnableHighlight(Color.yellow); // Highlight planned tiles in yellow
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
    private void ResetTurnFlags()
    {
        foreach (Pawn pawn in FindObjectsOfType<Pawn>())
        {
            pawn.ResetStatus();
        }
    }
    private void StartNewTurn()
    {
        // Clear the queue after executing all actions
        actionQueue.Clear();
        DisablePlannedTileHighlights();
        ready = true;
        ResetTurnFlags();
    }
}
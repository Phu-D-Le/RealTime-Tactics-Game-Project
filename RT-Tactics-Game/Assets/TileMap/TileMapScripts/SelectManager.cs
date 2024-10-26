using System.Collections.Generic; 
using UnityEngine;
using System.Collections;
using System.Linq;

// Very beefy and very likely to be ineffecient class. Handles clicking the map and all behaviours associated with activating
// the BFS. ZO

public enum GamePhase
{
    MovePhase,
    AttackPhase
}

public class SelectManager : MonoBehaviour
{
    public GamePhase currentPhase;
    private Camera mainCamera;
    private HexGrid hexGrid;
    private List<Vector3Int> neighbours = new List<Vector3Int>();
    
    private Pawn selectedPawn;  
    private Vector3Int selectedTile; 
    private Pawn selectedTargetPawn; 
    public Attack selectedAttack;

    public bool IsMoving { get; set; }
    public bool IsAttacking { get; set; }

    private List<Pawn> movementQueue = new List<Pawn>();
    private List<Vector3Int> targetTiles = new List<Vector3Int>();
    private HashSet<Vector3Int> occupiedTiles = new HashSet<Vector3Int>();

    private List<Pawn> attackQueue = new List<Pawn>();
    private List<Pawn> attackTargets = new List<Pawn>();

    void Start()
    {
        currentPhase = GamePhase.MovePhase;
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
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
        else if (Input.GetKeyDown(KeyCode.M) && currentPhase == GamePhase.MovePhase) // Current action buttion is M.
        // we could keep it as space bar but have it check for the game phase but BattleSystem also uses space bar
        // as turn ultimatum. idk M is placeholder action button. ZO
        {
            MoveAllPawns();
            currentPhase = GamePhase.AttackPhase;
            Debug.Log("Move phase complete, now enter attack phase.");
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            currentPhase = GamePhase.MovePhase;

            movementQueue.Clear();
            targetTiles.Clear();
            DisableAllHighlights(); // These are failsafes so player cannot select or move on enemy turn. ZO

            ExecuteAllAttacks();
            Debug.Log("Attack phase complete, now enter move phase.");
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
    private void HandleTileClick(GameObject clickedTile) // Now tile has been clicked. check if what phase we 
    // are in and handle action based on current. ZO
    {
        Hex hexComponent = clickedTile.GetComponent<Hex>();
        if (hexComponent != null)
        {
            if (currentPhase == GamePhase.MovePhase)
            {
                TryMovePawn(hexComponent, clickedTile);
            }
            else if (currentPhase == GamePhase.AttackPhase)
            {
                TryAttackPawn(hexComponent, clickedTile);
            }
        }
    }
    private void TryMovePawn(Hex hexComponent, GameObject clickedTile) // Handles the pawn selected from pawnHUD, 
    // pawnHUD selects pawn and already commits BFS and highlighting valid tiles, now we queue up selected movement. ZO
    {
        Vector3Int targetCoords = hexComponent.HexCoords;

        if (IsTileInRange(hexComponent.HexCoords) && FindPawnOnTile(clickedTile) == null) // Highlighted and no pawn is on tile. ZO
        {
            Pawn currentPawn = GetCurrentPawn();

            if (currentPawn != null && !currentPawn.hasMoved)
            {
                movementQueue.Add(currentPawn);
                targetTiles.Add(hexComponent.HexCoords);

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
    private void MoveAllPawns() // Running through a list is honestly similar to a queue but basically dequeue movement here. ZO
    {
        for (int i = 0; i < movementQueue.Count; i++)
        {
            Pawn pawn = movementQueue[i];
            Vector3Int targetTile = targetTiles[i];

            MovePawnToTile(pawn, hexGrid.GetTileAt(targetTile).gameObject);
        }
        movementQueue.Clear();
        targetTiles.Clear();
        occupiedTiles.Clear(); // M is pressed so these work. if m is not pressed, we do not call these. ZO
    }
    private void TryAttackPawn(Hex hexComponent, GameObject clickedTile) // AttackHUD has done our BFS and highlighting so now
    // we add our attacks to a list and associate an attack to each pawn (could use another list but eh). ZO
    {
        if (IsTileInRange(hexComponent.HexCoords))
        {
            Pawn targetPawn = FindPawnOnTile(clickedTile);
            if (targetPawn != null) // Pawn must be on tile for an attack to work. Perhaps set no friendly fire here? ZO
            {
                Pawn currentPawn = GetCurrentPawn();
                if (currentPawn != null && !currentPawn.hasAttacked)
                {
                    if (selectedAttack != null)
                    {
                        attackQueue.Add(currentPawn);
                        attackTargets.Add(targetPawn);

                        Debug.Log($"{currentPawn.pawnName} queued {selectedAttack.attackName} to attack {targetPawn.pawnName}.");
                    
                        currentPawn.selectedAttack = selectedAttack; // just assign the attack to pawn because I am lazy. ZO
                    }
                    else
                    {
                        Debug.Log("No attack selected.");
                    }
                    currentPawn.Attack(); // hasAttacked = true. ZO
                    DisableAllHighlights();
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
    private void ExecuteAllAttacks()
    { // Now we move through the attack queue and attack targets based on a pawns associated selected attack. ZO
        for (int i = 0; i < attackQueue.Count; i++)
        {
            Pawn attackingPawn = attackQueue[i];
            Pawn targetPawn = attackTargets[i];

            if (attackingPawn.selectedAttack != null)
            {
                attackingPawn.DealAttack(attackingPawn.selectedAttack, targetPawn);
                Debug.Log($"{attackingPawn.pawnName} used {attackingPawn.selectedAttack.attackName} to attack {targetPawn.pawnName}.");
                attackingPawn.selectedAttack = null;
            }
            else
            {
                Debug.LogWarning("No attack selected for this pawn.");
            }
        }
        attackQueue.Clear();
        attackTargets.Clear();
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
    private void MovePawnToTile(Pawn pawn, GameObject targetTile) // Physical movement called once dequeueing.
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
                StartCoroutine(MoveAlongPath(pawn, pathTiles));
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
    }
    public void HighlightTilesForPawn(Pawn pawn) // Shader highlights tiles, BFS finds valid neighbours. 
    // called by pawnHUD once pawn is selected and current phase is MovePhase. ZO
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

                foreach (Vector3Int neighbour in neighbours) // Tiles not occupied by another from either queue or if a pawn is there. ZO
                {
                    if (!occupiedTiles.Contains(neighbour))
                    {
                        Hex tileHex = hexGrid.GetTileAt(neighbour);
                        if (tileHex != null && FindPawnOnTile(tileHex.gameObject) == null)
                        {
                            tileHex.EnableHighlight();
                        }
                    }
                } 
            }
        }
    }
    public void HighlightTilesForAttack(Pawn pawn, Attack attack) // Ensures all tiles are highlighted. including if a pawn is on it. 
    // do not include self for highlighting for valid. ZO
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
                if (tileHex != null)
                {
                    tileHex.EnableHighlight();
                }
            }
        }
    }
    private void DisableAllHighlights() // Reset highlight and bfs. ZO
    {
        foreach (Vector3Int neighbour in neighbours)
        {
            hexGrid.GetTileAt(neighbour)?.DisableHighlight();
        }
        neighbours.Clear();  
    }
    private Pawn FindPawnOnTile(GameObject tile) // simply find if a pawn is there or not. ZO
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
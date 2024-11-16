using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum BattleState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    WON,
    LOST
}

public class BattleSystem : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;
    private Player firstPlayer;
    private Player enemyPlayer;
    public PawnHUD pawnHUD;
    public AttackHUD attackHUD;
    public TextMeshProUGUI turnDialogueText;
    public BattleState state;
    public TileMapManager tileMapManager;
    private TileMapSpawner spawner;

    void Start()
    {
        state = BattleState.START;

        Player = GameObject.FindWithTag("Player");
        Enemy = GameObject.FindWithTag("Enemy");

        tileMapManager.GenerateTileMap();

        spawner = FindObjectOfType<TileMapSpawner>();
        if (spawner == null)
        {
            Debug.LogError("TileMapSpawner not found in the scene. Make sure it is attached to the TileMapManager.");
            return;
        }

        spawner.InitializeSpawner();

        firstPlayer = Player.GetComponent<Player>();
        firstPlayer.playerName = "Player";
        firstPlayer.SetList();
        InitializePawns(firstPlayer, "PlayerPawn");

        enemyPlayer = Enemy.GetComponent<Player>();
        enemyPlayer.playerName = "Enemy";
        enemyPlayer.SetList();
        InitializePawns(enemyPlayer, "EnemyPawn");

        firstPlayer.SpawnPawnsOnMap(spawner);
        enemyPlayer.SpawnPawnsOnMap(spawner);

        attackHUD.gameObject.SetActive(false);

        SetUpBattle();
    }

    private void InitializePawns(Player player, string tag)
    {
        foreach (var pawn in player.pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            currentPawn.gameObject.tag = tag;
            currentPawn.AwakenPawn();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (state == BattleState.PLAYERTURN)
            {
                EndPlayerTurn();
            }
        }
    }

    void SetUpBattle()
    {
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        turnDialogueText.text = "Player's Turn!";
        pawnHUD.SetPlayerCanvas(firstPlayer);
    }

    void EndPlayerTurn()
    {
        state = BattleState.ENEMYTURN;
        StartCoroutine(ExecuteEnemyTurn());
    }

    private IEnumerator ExecuteEnemyTurn()
    {
        turnDialogueText.text = "Enemy's Turn!";

        // Track occupied tiles to prevent stacking
        HashSet<Vector3Int> occupiedTiles = GetOccupiedTiles();

        foreach (var pawn in enemyPlayer.pawns)
        {
            Pawn enemyPawn = pawn.GetComponent<Pawn>();
            if (!enemyPawn.hasMoved && !enemyPawn.hasAttacked)
            {
                // Try attacking first
                if (TryEnemyAttack(enemyPawn))
                {
                    yield return new WaitForSeconds(1f); // Simulate attack delay
                }
                else
                {
                    // Move toward the nearest player pawn
                    TryMoveTowardPlayer(enemyPawn, occupiedTiles);
                    yield return new WaitForSeconds(1f); // Simulate move delay
                }
            }
        }

        // End the enemy turn and switch back to the player
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    private bool TryEnemyAttack(Pawn enemyPawn)
    {
        foreach (var attack in enemyPawn.attacks)
        {
            List<Pawn> playerPawnsInRange = FindPawnsInRange(enemyPawn, attack.range, "PlayerPawn");
            if (playerPawnsInRange.Count > 0)
            {
                // Target the first player pawn in range
                Pawn targetPawn = playerPawnsInRange[0];
                enemyPawn.DealAttack(attack, targetPawn);
                enemyPawn.Attack(); // Mark as attacked
                Debug.Log($"{enemyPawn.pawnName} attacked {targetPawn.pawnName}");
                return true;
            }
        }
        return false;
    }

    private void TryMoveTowardPlayer(Pawn enemyPawn, HashSet<Vector3Int> occupiedTiles)
    {
        // Find the nearest player pawn
        Pawn nearestPlayerPawn = FindNearestPawn(enemyPawn, "PlayerPawn");

        if (nearestPlayerPawn != null)
        {
            // Find tiles in range of the enemy pawn
            List<Vector3Int> tilesInRange = FindAvailableTiles(enemyPawn);

            // Exclude occupied tiles
            tilesInRange = tilesInRange.Where(tile => !occupiedTiles.Contains(tile)).ToList();

            if (tilesInRange.Count > 0)
            {
                // Sort tiles by distance to the nearest player pawn and move closer
                Vector3Int targetTileCoords = tilesInRange
                    .OrderBy(tile => Vector3Int.Distance(tile, nearestPlayerPawn.CurrentTile.GetComponent<Hex>().HexCoords))
                    .FirstOrDefault();

                GameObject targetTile = tileMapManager.GetComponent<HexGrid>().GetTileAt(targetTileCoords)?.gameObject;
                if (targetTile != null)
                {
                    StartCoroutine(MovePawnToTile(enemyPawn, targetTile));
                    enemyPawn.Move(); // Mark as moved
                    occupiedTiles.Add(targetTileCoords); // Mark the tile as occupied
                }
            }
            else
            {
                Debug.LogWarning($"{enemyPawn.pawnName} could not find an available tile to move to.");
            }
        }
        else
        {
            Debug.LogWarning($"{enemyPawn.pawnName} could not find a valid target.");
        }
    }

    private HashSet<Vector3Int> GetOccupiedTiles()
    {
        HashSet<Vector3Int> occupiedTiles = new HashSet<Vector3Int>();

        // Include tiles occupied by all player and enemy pawns
        foreach (var pawn in FindObjectsOfType<Pawn>())
        {
            if (pawn.CurrentTile != null)
            {
                Hex hex = pawn.CurrentTile.GetComponent<Hex>();
                if (hex != null)
                {
                    occupiedTiles.Add(hex.HexCoords);
                }
            }
        }

        return occupiedTiles;
    }

    private Pawn FindNearestPawn(Pawn enemyPawn, string targetTag)
    {
        Pawn nearestPawn = null;
        float closestDistance = float.MaxValue;

        foreach (var pawn in FindObjectsOfType<Pawn>())
        {
            if (pawn.CompareTag(targetTag))
            {
                float distance = Vector3.Distance(enemyPawn.transform.position, pawn.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestPawn = pawn;
                }
            }
        }

        return nearestPawn;
    }

    private IEnumerator MovePawnToTile(Pawn pawn, GameObject targetTile)
    {
        Hex targetHex = targetTile.GetComponent<Hex>();
        Hex currentHex = pawn.CurrentTile.GetComponent<Hex>();

        if (targetHex != null && currentHex != null)
        {
            BFSResult bfsResult = GraphSearch.BFSGetRange(tileMapManager.GetComponent<HexGrid>(), currentHex.HexCoords, pawn.pawnSpeed);
            if (bfsResult.IsHexPositionInRange(targetHex.HexCoords))
            {
                List<Vector3Int> path = GraphSearch.GetExactPathToDestination(targetHex.HexCoords, bfsResult.visitedNodesDict);
                foreach (Vector3Int step in path)
                {
                    Hex stepHex = tileMapManager.GetComponent<HexGrid>().GetTileAt(step);
                    if (stepHex != null)
                    {
                        Vector3 targetPosition = stepHex.transform.position + new Vector3(0, 2f, 0); // Adjust Y if needed
                        while (Vector3.Distance(pawn.transform.position, targetPosition) > 0.1f)
                        {
                            pawn.transform.position = Vector3.MoveTowards(
                                pawn.transform.position,
                                targetPosition,
                                Time.deltaTime * pawn.pawnSpeed
                            );
                            yield return null;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError($"No valid path to target tile {targetHex.HexCoords}");
            }
        }
        else
        {
            Debug.LogError("Target or current hex is null.");
        }

        pawn.CurrentTile = targetTile;
        Debug.Log($"{pawn.pawnName} moved to {targetTile.name}");
    }

    private List<Pawn> FindPawnsInRange(Pawn enemyPawn, int range, string targetTag)
    {
        BFSResult bfsResult = GraphSearch.BFSGetRange(tileMapManager.GetComponent<HexGrid>(), enemyPawn.CurrentTile.GetComponent<Hex>().HexCoords, range);
        List<Vector3Int> rangePositions = new List<Vector3Int>(bfsResult.GetRangePositions());
        List<Pawn> pawnsInRange = new List<Pawn>();

        foreach (Vector3Int position in rangePositions)
        {
            GameObject tile = tileMapManager.GetComponent<HexGrid>().GetTileAt(position)?.gameObject;
            if (tile != null)
            {
                Pawn pawnOnTile = FindPawnOnTile(tile);
                if (pawnOnTile != null && pawnOnTile.CompareTag(targetTag))
                {
                    pawnsInRange.Add(pawnOnTile);
                }
            }
        }
        return pawnsInRange;
    }

    private List<Vector3Int> FindAvailableTiles(Pawn enemyPawn)
    {
        BFSResult bfsResult = GraphSearch.BFSGetRange(tileMapManager.GetComponent<HexGrid>(), enemyPawn.CurrentTile.GetComponent<Hex>().HexCoords, enemyPawn.pawnSpeed);
        return bfsResult.GetRangePositions().Where(pos => tileMapManager.GetComponent<HexGrid>().GetTileAt(pos) != null).ToList();
    }

    private Pawn FindPawnOnTile(GameObject tile)
    {
        Hex clickedHex = tile.GetComponent<Hex>();
        if (clickedHex == null) return null;

        Vector3Int hexCoords = clickedHex.HexCoords;

        foreach (Pawn pawn in FindObjectsOfType<Pawn>())
        {
            if (pawn.CurrentTile.GetComponent<Hex>().HexCoords == hexCoords)
            {
                return pawn;
            }
        }
        return null;
    }
}

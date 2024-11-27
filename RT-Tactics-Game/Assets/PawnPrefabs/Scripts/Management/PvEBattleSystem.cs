using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum PvEBattleState
{
    ENEMY_PLANNING,
    PLAYER_INPUT,
    RESOLVING_ACTIONS,
    GAME_OVER
}

public class PvEBattleSystem : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;
    private Player firstPlayer;
    private Player enemyPlayer;
    public PawnHUD pawnHUD;
    public AttackHUD attackHUD;
    public TextMeshProUGUI turnDialogueText;
    public PvEBattleState state;
    public TileMapManager tileMapManager;
    private TileMapSpawner spawner;

    private List<Action> playerActionsQueue = new List<Action>();
    private List<Action> enemyActionsQueue = new List<Action>();
    private Dictionary<Pawn, Vector3Int> aiPlannedTiles = new Dictionary<Pawn, Vector3Int>();
    private HashSet<Vector3Int> highlightedAITiles = new HashSet<Vector3Int>();

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        Enemy = GameObject.FindWithTag("Enemy");

        tileMapManager.GenerateTileMap();

        spawner = FindObjectOfType<TileMapSpawner>();
        if (spawner == null)
        {
            Debug.LogError("TileMapSpawner not found in the scene.");
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

        state = PvEBattleState.ENEMY_PLANNING;
        StartCoroutine(EnemyPlanningPhase());
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
        if (state == PvEBattleState.PLAYER_INPUT && Input.GetKeyDown(KeyCode.Space))
        {
            state = PvEBattleState.RESOLVING_ACTIONS;
            StartCoroutine(ResolveActions());
        }
    }

    private IEnumerator EnemyPlanningPhase()
    {
        turnDialogueText.text = "Enemy is Planning...";
        enemyActionsQueue.Clear();
        aiPlannedTiles.Clear();

        // Plan AI moves and highlight tiles
        foreach (var pawn in enemyPlayer.pawns)
        {
            if (pawn.GetComponent<Pawn>().currentHP > 0)
            {
                if (!TryEnemyAttack(pawn.GetComponent<Pawn>()))
                {
                    TryMoveTowardPlayer(pawn.GetComponent<Pawn>());
                }
            }
        }

        HighlightAITiles();

        yield return new WaitForSeconds(1f);

        // End AI planning phase
        state = PvEBattleState.PLAYER_INPUT;
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        turnDialogueText.text = "Player's Turn!";
        pawnHUD.SetPlayerCanvas(firstPlayer);

        playerActionsQueue.Clear();
    }

    public void QueuePlayerAction(Action action)
    {
        if (state == PvEBattleState.PLAYER_INPUT)
        {
            playerActionsQueue.Add(action);
        }
    }

    private IEnumerator ResolveActions()
    {
        turnDialogueText.text = "Resolving Actions...";

        // Combine player and AI actions into a single list
        List<Action> combinedActionsQueue = CombineActions(playerActionsQueue, enemyActionsQueue);

        // Execute all actions simultaneously
        yield return ExecuteSimultaneousActions(combinedActionsQueue);

        // Reset and proceed to the next turn
        state = PvEBattleState.ENEMY_PLANNING;
        StartCoroutine(EnemyPlanningPhase());
    }

    private void HighlightAITiles()
    {
        foreach (var tile in aiPlannedTiles.Values)
        {
            highlightedAITiles.Add(tile);
            var hex = tileMapManager.GetComponent<HexGrid>().GetTileAt(tile);
            if (hex != null)
            {
                hex.EnableHighlight(Color.yellow);
            }
        }
    }

    private void ClearAIHighlights()
    {
        foreach (var tile in highlightedAITiles)
        {
            var hex = tileMapManager.GetComponent<HexGrid>().GetTileAt(tile);
            if (hex != null)
            {
                hex.DisableHighlight();
            }
        }
        highlightedAITiles.Clear();
    }

    private bool TryEnemyAttack(Pawn enemyPawn)
    {
        foreach (var attack in enemyPawn.attacks)
        {
            List<Pawn> playerPawnsInRange = FindPawnsInRange(enemyPawn, attack.range, "PlayerPawn");
            if (playerPawnsInRange.Count > 0)
            {
                Action attackAction = new Action(ActionType.Attack, enemyPawn, playerPawnsInRange[0], attack);
                enemyActionsQueue.Add(attackAction);
                return true;
            }
        }
        return false;
    }

    private void TryMoveTowardPlayer(Pawn enemyPawn)
    {
        Pawn nearestPlayerPawn = FindNearestPawn(enemyPawn, "PlayerPawn");
        if (nearestPlayerPawn != null)
        {
            List<Vector3Int> tilesInRange = FindAvailableTiles(enemyPawn).ToList();

            Vector3Int targetTile = tilesInRange
                .OrderBy(tile => Vector3Int.Distance(tile, nearestPlayerPawn.CurrentTile.GetComponent<Hex>().HexCoords))
                .FirstOrDefault(tile => !IsTileOccupied(tile));

            if (targetTile != null)
            {
                Action moveAction = new Action(ActionType.Move, enemyPawn, targetTile);
                enemyActionsQueue.Add(moveAction);
                aiPlannedTiles[enemyPawn] = targetTile;
            }
        }
    }

    private IEnumerator ExecuteSimultaneousActions(List<Action> actions)
    {
        ClearAIHighlights();

        List<IEnumerator> actionCoroutines = new List<IEnumerator>();

        foreach (var action in actions)
        {
            if (action.pawn.currentHP > 0)
            {
                if (action.actionType == ActionType.Move)
                {
                    actionCoroutines.Add(MovePawn(action.pawn, action.targetTile));
                }
                else if (action.actionType == ActionType.Attack)
                {
                    actionCoroutines.Add(ExecuteAttack(action));
                }
            }
        }

        foreach (var coroutine in actionCoroutines)
        {
            StartCoroutine(coroutine);
        }

        yield return new WaitForSeconds(2f); // Allow time for all actions to resolve
    }

    private IEnumerator ExecuteAttack(Action action)
    {
        action.pawn.DealAttack(action.selectedAttack, action.targetPawn);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator MovePawn(Pawn pawn, Vector3Int targetTileCoords)
    {
        if (IsTileOccupied(targetTileCoords))
        {
            Debug.LogWarning($"Target tile {targetTileCoords} is already occupied. Finding nearest available tile.");
            targetTileCoords = FindNearestAvailableTile(targetTileCoords);
        }

        GameObject targetTile = tileMapManager.GetComponent<HexGrid>().GetTileAt(targetTileCoords)?.gameObject;
        if (targetTile != null)
        {
            Hex targetHex = targetTile.GetComponent<Hex>();
            if (targetHex != null)
            {
                Vector3 targetPosition = targetHex.transform.position + new Vector3(0, 2f, 0);
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

        pawn.CurrentTile = targetTile;
    }

    private bool IsTileOccupied(Vector3Int tileCoords)
    {
        foreach (var pawn in FindObjectsOfType<Pawn>())
        {
            if (pawn.CurrentTile != null && pawn.CurrentTile.GetComponent<Hex>().HexCoords == tileCoords)
            {
                return true;
            }
        }
        return false;
    }

    private Vector3Int FindNearestAvailableTile(Vector3Int startingTile)
    {
        BFSResult bfsResult = GraphSearch.BFSGetRange(tileMapManager.GetComponent<HexGrid>(), startingTile, 1);
        foreach (Vector3Int position in bfsResult.GetRangePositions())
        {
            if (!IsTileOccupied(position))
            {
                return position;
            }
        }
        return startingTile;
    }

    private List<Action> CombineActions(List<Action> playerActions, List<Action> enemyActions)
    {
        List<Action> combinedActions = new List<Action>();
        combinedActions.AddRange(playerActions);
        combinedActions.AddRange(enemyActions);
        return combinedActions;
    }

    private List<Pawn> FindPawnsInRange(Pawn pawn, int range, string targetTag)
    {
        BFSResult bfsResult = GraphSearch.BFSGetRange(tileMapManager.GetComponent<HexGrid>(), pawn.CurrentTile.GetComponent<Hex>().HexCoords, range);
        List<Pawn> pawnsInRange = new List<Pawn>();

        foreach (Vector3Int position in bfsResult.GetRangePositions())
        {
            GameObject tile = tileMapManager.GetComponent<HexGrid>().GetTileAt(position)?.gameObject;
            if (tile != null)
            {
                Pawn targetPawn = FindPawnOnTile(tile);
                if (targetPawn != null && targetPawn.CompareTag(targetTag))
                {
                    pawnsInRange.Add(targetPawn);
                }
            }
        }

        return pawnsInRange;
    }

    private Pawn FindNearestPawn(Pawn pawn, string targetTag)
    {
        Pawn nearestPawn = null;
        float closestDistance = float.MaxValue;

        foreach (var targetPawn in FindObjectsOfType<Pawn>())
        {
            if (targetPawn.CompareTag(targetTag))
            {
                float distance = Vector3.Distance(pawn.transform.position, targetPawn.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestPawn = targetPawn;
                }
            }
        }

        return nearestPawn;
    }

    private IEnumerable<Vector3Int> FindAvailableTiles(Pawn pawn)
    {
        BFSResult bfsResult = GraphSearch.BFSGetRange(tileMapManager.GetComponent<HexGrid>(), pawn.CurrentTile.GetComponent<Hex>().HexCoords, pawn.pawnSpeed);
        return bfsResult.GetRangePositions().Where(pos => !IsTileOccupied(pos));
    }

    private Pawn FindPawnOnTile(GameObject tile)
    {
        Hex hex = tile.GetComponent<Hex>();
        if (hex == null) return null;

        Vector3Int hexCoords = hex.HexCoords;
        foreach (Pawn pawn in FindObjectsOfType<Pawn>())
        {
            if (pawn.CurrentTile != null && pawn.CurrentTile.GetComponent<Hex>().HexCoords == hexCoords)
            {
                return pawn;
            }
        }

        return null;
    }
}

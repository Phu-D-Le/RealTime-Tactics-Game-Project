using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum PvEBattleState
{
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

    void Start()
    {
        // Initialize references
        Player = GameObject.FindWithTag("Player");
        Enemy = GameObject.FindWithTag("Enemy");

        if (Player == null || Enemy == null || tileMapManager == null || pawnHUD == null || attackHUD == null)
        {
            Debug.LogError("One or more references are not assigned in the Inspector.");
            return;
        }

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

        // Start with player's turn
        state = PvEBattleState.PLAYER_INPUT;
        PlayerTurn();
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
        if (state == PvEBattleState.PLAYER_INPUT)
        {
            HandlePlayerInput();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EndPlayerTurn();
            }
        }
    }

    private void PlayerTurn()
    {
        Debug.Log("Player's turn started");
        turnDialogueText.text = "Player's Turn!";
        pawnHUD.SetPlayerCanvas(firstPlayer);
        playerActionsQueue.Clear();
        ResetPawnsForNewTurn(firstPlayer);
        state = PvEBattleState.PLAYER_INPUT;
    }

    private void HandlePlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedTile = hit.collider.gameObject;
                Hex hex = clickedTile.GetComponent<Hex>();

                if (hex != null && !hex.IsOccupied())
                {
                    Vector3Int targetCoords = hex.HexCoords;
                    Pawn selectedPawn = pawnHUD.selectedPawn;

                    // Ensure the selected pawn is a player-controlled pawn
                    if (selectedPawn != null && selectedPawn.CompareTag("PlayerPawn") && IsTileInRange(selectedPawn, targetCoords))
                    {
                        Action moveAction = new Action(ActionType.Move, selectedPawn, targetCoords);
                        QueuePlayerAction(moveAction);

                        Debug.Log($"Queued move action for {selectedPawn.pawnName} to {targetCoords}");
                    }
                    else
                    {
                        Debug.Log("Target tile is out of range or no player pawn is selected.");
                    }
                }
                else
                {
                    Debug.Log("Clicked on an invalid or occupied tile.");
                }
            }
        }
    }

    private void QueuePlayerAction(Action action)
    {
        if (state == PvEBattleState.PLAYER_INPUT)
        {
            playerActionsQueue.Add(action);
            Debug.Log($"Queued action for {action.pawn.pawnName}");
        }
        else
        {
            Debug.LogWarning("Attempted to queue a player action outside of the player's turn.");
        }
    }

    private bool IsTileInRange(Pawn pawn, Vector3Int targetCoords)
    {
        BFSResult bfsResult = GraphSearch.BFSGetRange(
            tileMapManager.GetComponent<HexGrid>(),
            pawn.CurrentTile.GetComponent<Hex>().HexCoords,
            pawn.pawnSpeed
        );

        return bfsResult.GetRangePositions().Contains(targetCoords);
    }

    private void EndPlayerTurn()
    {
        Debug.Log("Player's turn ended");
        state = PvEBattleState.RESOLVING_ACTIONS;
        StartCoroutine(ResolveActions());
    }

    private IEnumerator ResolveActions()
    {
        turnDialogueText.text = "Resolving Actions...";
        GenerateEnemyActions();
        List<Action> combinedActionsQueue = CombineActions(playerActionsQueue, enemyActionsQueue);
        yield return ExecuteActionsSequentially(combinedActionsQueue);

        if (state != PvEBattleState.GAME_OVER)
        {
            PlayerTurn(); // Ensure control returns to player after resolving actions
        }
    }

    private void GenerateEnemyActions()
    {
        enemyActionsQueue.Clear();

        foreach (var pawn in enemyPlayer.pawns)
        {
            Pawn enemyPawn = pawn.GetComponent<Pawn>();

            // Ensure the AI only acts on AI-controlled pawns
            if (enemyPawn != null && enemyPawn.CompareTag("EnemyPawn") && enemyPawn.currentHP > 0)
            {
                Vector3Int targetTile = FindAvailableTiles(enemyPawn).FirstOrDefault();
                if (targetTile != null)
                {
                    Action moveAction = new Action(ActionType.Move, enemyPawn, targetTile);
                    enemyActionsQueue.Add(moveAction);
                }
            }
        }

        Debug.Log("Enemy actions generated.");
    }

    private List<Action> CombineActions(List<Action> playerActions, List<Action> enemyActions)
    {
        return playerActions.Concat(enemyActions).ToList();
    }

    private IEnumerator ExecuteActionsSequentially(List<Action> actions)
    {
        foreach (var action in actions)
        {
            if (action.actionType == ActionType.Move)
            {
                yield return MovePawn(action.pawn, action.targetTile);
            }
            else if (action.actionType == ActionType.Attack)
            {
                yield return ExecuteAttack(action);
            }
        }
    }

    private IEnumerator ExecuteAttack(Action action)
    {
        action.pawn.DealAttack(action.selectedAttack, action.targetPawn);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator MovePawn(Pawn pawn, Vector3Int targetCoords)
    {
        GameObject targetTileObject = tileMapManager.GetComponent<HexGrid>().GetTileAt(targetCoords)?.gameObject;
        if (targetTileObject == null) yield break;

        Hex targetHex = targetTileObject.GetComponent<Hex>();
        if (targetHex == null) yield break;

        if (pawn.CurrentTile != null)
        {
            pawn.CurrentTile.GetComponent<Hex>().VacateTile();
        }

        Vector3 targetPosition = targetTileObject.transform.position + new Vector3(0, 2f, 0);
        while (Vector3.Distance(pawn.transform.position, targetPosition) > 0.1f)
        {
            pawn.transform.position = Vector3.MoveTowards(pawn.transform.position, targetPosition, Time.deltaTime * pawn.pawnSpeed);
            yield return null;
        }

        pawn.CurrentTile = targetTileObject;
        targetHex.OccupyTile(pawn);
    }

    private IEnumerable<Vector3Int> FindAvailableTiles(Pawn pawn)
    {
        BFSResult bfsResult = GraphSearch.BFSGetRange(
            tileMapManager.GetComponent<HexGrid>(),
            pawn.CurrentTile.GetComponent<Hex>().HexCoords,
            pawn.pawnSpeed
        );

        return bfsResult.GetRangePositions().Where(pos => !IsTileOccupied(pos));
    }

    private bool IsTileOccupied(Vector3Int tileCoords)
    {
        foreach (var pawn in FindObjectsOfType<Pawn>())
        {
            if (pawn.CurrentTile.GetComponent<Hex>().HexCoords == tileCoords)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetPawnsForNewTurn(Player player)
    {
        foreach (var pawn in player.pawns)
        {
            Pawn currentPawn = pawn.GetComponent<Pawn>();
            if (currentPawn != null)
            {
                currentPawn.ResetStatus(); // Ensure all pawns are ready for the next turn
            }
        }
    }
}

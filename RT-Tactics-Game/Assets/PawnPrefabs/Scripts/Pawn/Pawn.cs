using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public string pawnName { get; private set; }
    public Sprite pawnSprite { get; private set; }
    public List<Attack> attacks { get; private set; }
    public int pawnSpeed { get; private set; }
    public int maxHP { get; private set; }
    public int currentHP { get; private set; }
    public bool hasAttacked { get; private set; }
    public bool hasMoved { get; private set; }
    public GameObject CurrentTile { get; set; }
    public Attack selectedAttack { get; set; }

    public PawnType pawnType;
    public HealthHUD healthHUD;
    private Player player;

    public bool isAIControlled { get; set; }
    public float attackRange = 1.5f;

    public void AwakenPawn()
    {
        player = GetComponentInParent<Player>();
        healthHUD = GetComponentInChildren<HealthHUD>();
        InitializeFromPawnType(pawnType);
        healthHUD.SetHealthHUD(this);
    }

    public void DealAttack(Attack attack, Pawn target)
    {
        target.TakeDamage(attack.damage);
        hasAttacked = true;
        Debug.Log($"{pawnName} has attacked {target.pawnName} with {attack.attackName}, dealing {attack.damage} damage.");
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        healthHUD.SetHP(currentHP);
        if (currentHP <= 0)
        {
            Debug.Log($"{pawnName} has died.");
            player.RemovePawn(this);
        }
        Debug.Log($"{pawnName} takes {amount} damage. Current HP: {currentHP}");
    }

    public void InitializeFromPawnType(PawnType type)
    {
        pawnName = type.pawnTypeName;
        pawnSprite = type.pawnTypeSprite;
        attacks = new List<Attack>(type.pawnTypeAttacks);
        pawnSpeed = type.pawnTypeSpeed;
        maxHP = type.pawnTypeMaxHP;
        currentHP = type.pawnTypeCurrentHP;
        hasAttacked = false;
        hasMoved = false;
    }

    public void ResetStatus()
    {
        hasAttacked = false;
        hasMoved = false;
    }

    public void MoveTowardsPlayer(Player player)
    {
        if (!isAIControlled || hasMoved) return;

        Pawn closestPlayerPawn = FindClosestPawn(player);
        if (closestPlayerPawn == null)
        {
            Debug.LogWarning($"{pawnName} found no closest player pawn.");
            return;
        }

        Vector3Int startTile = CurrentTile.GetComponent<HexCoordinates>().GetHexCoords();
        Vector3Int targetTile = closestPlayerPawn.CurrentTile.GetComponent<HexCoordinates>().GetHexCoords();

        Debug.Log($"{pawnName} attempting to move towards {closestPlayerPawn.pawnName} from {startTile} to {targetTile}");

        List<Vector3Int> path = HexGrid.Instance.FindPath(startTile, targetTile, pawnSpeed);

        if (path != null && path.Count > 0)
        {
            Debug.Log($"{pawnName} found a path with {path.Count} steps.");
            StartCoroutine(MoveAlongPath(path));
        }
        else
        {
            Debug.LogWarning($"{pawnName} could not find a valid path to move towards {closestPlayerPawn.pawnName}.");
        }
    }

    private IEnumerator MoveAlongPath(List<Vector3Int> path)
    {
        foreach (Vector3Int tileCoord in path)
        {
            Hex targetTile = HexGrid.Instance.GetTileAt(tileCoord);
            if (targetTile != null)
            {
                Vector3 targetPosition = targetTile.transform.position;
                targetPosition.y = transform.position.y;

                Debug.Log($"{pawnName} moving to {targetTile.name} at position {targetPosition}");

                while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, pawnSpeed * Time.deltaTime);
                    yield return null;
                }

                CurrentTile = targetTile.gameObject;
            }
        }

        hasMoved = true;
        Debug.Log($"{pawnName} has completed movement.");
    }

    public void AttackNearestPlayer()
    {
        if (!isAIControlled || hasAttacked) return;

        Pawn closestPlayerPawn = FindClosestPawn(player);
        if (closestPlayerPawn != null)
        {
            float distance = Vector3.Distance(transform.position, closestPlayerPawn.transform.position);

            if (distance <= attackRange)
            {
                selectedAttack = attacks[0];
                DealAttack(selectedAttack, closestPlayerPawn);
                Debug.Log($"{pawnName} attacks {closestPlayerPawn.pawnName} within range.");
            }
            else
            {
                Debug.Log($"{pawnName} is out of range to attack {closestPlayerPawn.pawnName} (distance: {distance})");
            }
        }
        else
        {
            Debug.LogWarning($"{pawnName} could not find a player pawn to attack.");
        }
    }

    private Pawn FindClosestPawn(Player player)
    {
        Pawn closestPawn = null;
        float shortestDistance = float.MaxValue;

        Vector3Int aiPawnCoords = CurrentTile.GetComponent<HexCoordinates>().GetHexCoords();

        foreach (var playerPawnObj in player.pawns)
        {
            Pawn playerPawn = playerPawnObj.GetComponent<Pawn>();
            if (playerPawn == null || playerPawn.CurrentTile == null) continue;

            Vector3Int playerPawnCoords = playerPawn.CurrentTile.GetComponent<HexCoordinates>().GetHexCoords();

            float hexDistance = HexGrid.Instance.CalculateHexDistance(aiPawnCoords, playerPawnCoords);

            Debug.Log($"{pawnName} checking distance to {playerPawn.pawnName}: Hex Distance = {hexDistance}");

            if (hexDistance < shortestDistance)
            {
                shortestDistance = hexDistance;
                closestPawn = playerPawn;
            }
        }

        if (closestPawn != null)
        {
            Debug.Log($"{pawnName} found closest player pawn: {closestPawn.pawnName} at hex distance {shortestDistance}");
        }
        else
        {
            Debug.LogWarning($"{pawnName} could not find any player pawns in range.");
        }

        return closestPawn;
    }

    public void Move()
    {
        hasMoved = true;
    }

    public void Attack()
    {
        hasAttacked = true;
    }
}

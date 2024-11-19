using UnityEngine;

public enum ActionType
{
    Move,
    Attack
}

public class Action
{
    public ActionType actionType { get; }
    public Pawn pawn { get; }
    public Vector3Int targetTile { get; }
    public Pawn targetPawn { get; }
    public Attack selectedAttack { get; }

    // Constructor for Move actions
    public Action(ActionType actionType, Pawn pawn, Vector3Int targetTile)
    {
        this.actionType = actionType;
        this.pawn = pawn;
        this.targetTile = targetTile;
    }

    // Constructor for Attack actions
    public Action(ActionType actionType, Pawn pawn, Pawn targetPawn, Attack selectedAttack)
    {
        this.actionType = actionType;
        this.pawn = pawn;
        this.targetPawn = targetPawn;
        this.selectedAttack = selectedAttack;
    }
}

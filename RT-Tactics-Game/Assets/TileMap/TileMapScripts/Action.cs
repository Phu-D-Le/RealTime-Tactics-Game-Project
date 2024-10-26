using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Move,
    Attack
}

public class Action
{
    public ActionType actionType;
    public Pawn pawn;
    public Vector3Int targetTile;
    public Pawn targetPawn;
    public Attack selectedAttack;

    public Action(ActionType type, Pawn pawn, Vector3Int targetTile)
    {
        this.actionType = type;
        this.pawn = pawn;
        this.targetTile = targetTile;
    }

    public Action(ActionType type, Pawn pawn, Pawn targetPawn, Attack selectedAttack)
    {
        this.actionType = type;
        this.pawn = pawn;
        this.targetPawn = targetPawn;
        this.selectedAttack = selectedAttack;
    }
}

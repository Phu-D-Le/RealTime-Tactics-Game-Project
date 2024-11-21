using System.Collections.Generic;
using UnityEngine;

// This is essentially the hub for setting and getting all the values from the PawnType
// scriptable object as well as some rudimentary Attack functions.
// There is also no controlling the situation for more than 3 pawns aside from only 3 buttons being available.
// Remember the private set, so values cannot be changed outside of here. ZO
public class Pawn : MonoBehaviour
{
    public string pawnName { get; private set; }
    public Sprite pawnSprite { get; private set; }
    public List<Attack> attacks { get; private set; }
    public List<SpecialAction> actions { get; set; }
    public int pawnSpeed { get; private set; }
    public int maxHP { get; private set; }
    public int currentHP { get; private set; }
    public bool hasAttacked { get; private set; }
    public bool hasActed { get; private set; }
    public bool hasMoved { get; private set; }
    public GameObject CurrentTile { get; set; }
    public Attack selectedAttack { get; set; }
    public SpecialAction selectedAction { get; set; }

    public bool specialDisable = false;

    public int startDuration;

    public int duration;

    public int team; // 1 is player, 0 is enemy


    public PawnType pawnType;
    public HealthHUD healthHUD;
    private Player player;

    public void AwakenPawn()
    {
        player = GetComponentInParent<Player>();
        healthHUD = GetComponentInChildren<HealthHUD>();
        InitializeFromPawnType(pawnType);
        healthHUD.SetHealthHUD(this);
    }

    public void Cursed(int duration)
    {
        specialDisable = true;
        this.duration = duration;
    }

    public void UpdateDuration()
    {
        duration--;
        if(duration == 0)
        {
            specialDisable = false;
        }
    }
    public void DealAttack(Attack attack, Pawn target) // Deal damage to another pawn. Called by SelectManager. ZO
    {
        target.TakeDamage(attack.damage);
        hasAttacked = true;
        Debug.Log($"{pawnName} has attacked {target.pawnName} with {attack.attackName}, dealing {attack.damage} damage.");
    }
    public void TakeDamage(int amount) // Current pawn will take certain damage and value of slider will renew
                                        // We could push in negative values to heal as well. ZO
    {
        currentHP -= amount;
        healthHUD.SetHP(currentHP);
        if (currentHP <= 0)
        {
            Debug.Log($"{pawnName} has died.");
            player.RemovePawn(this);  // Notify the Player to remove this pawn.
        } // Only reason PlayerHUD is not being notified immediately about death is because pawns should not
        // be attacking themselves. Need to add selection logic to further death logic...maybe. ZO
        Debug.Log($"{pawnName} takes {amount} damage. Current HP: {currentHP}");
    }
    public void InitializeFromPawnType(PawnType type)
    {
        pawnName = type.pawnTypeName;
        pawnSprite = type.pawnTypeSprite;
        attacks = new List<Attack>(type.pawnTypeAttacks);  // Create the list of attacks from everything in PawnType.
                                                           // Edit list when adding attacks between scenes. ZO

        actions = new List<SpecialAction>(type.pawnTypeActions);

        pawnSpeed = type.pawnTypeSpeed;
        maxHP = type.pawnTypeMaxHP;
        currentHP = type.pawnTypeCurrentHP;
        hasAttacked = false;
    }
    public void ResetStatus()
    {
        hasAttacked = false;
        hasMoved = false;
    }
    public void Move()
    {
        hasMoved = true;
    }
    public void Attack()
    {
        hasAttacked = true;
    }

    public void Act()
    {
        hasActed = true;
    }
}
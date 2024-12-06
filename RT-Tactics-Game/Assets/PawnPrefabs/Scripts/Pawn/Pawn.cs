using System.Collections;
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
    public bool isDead { get; private set; } = false;

    public PawnAnimation pawnAnimation;

    public AudioClip moveSound;
    public AudioClip deathSound;
    public AudioClip damageSound;

    public SpecialAction selectedAction { get; set; }

    public bool specialDisable = false;
    public bool lured = false;
    public Pawn luredSource = null;

    public int curseEnd;
    public int lureEnd;

    public int curseDuration;
    public int lureDuration;

    public int team; // 1 is player, 0 is enemy

    public PawnType pawnType;
    public HealthHUD healthHUD;
    private Player player;
    private AudioSource audioSource;

    public void AwakenPawn()
    {
        player = GetComponentInParent<Player>();
        healthHUD = GetComponentInChildren<HealthHUD>();
        InitializeFromPawnType(pawnType);
        healthHUD.SetHealthHUD(this);
        audioSource = GetComponent<AudioSource>();
        pawnAnimation = GetComponent<PawnAnimation>();
    }

    
    public void Cursed(int duration)
    {
        specialDisable = true;
        this.curseDuration = duration;
        curseEnd = GlobalVariables.turns + duration;
    }

    public void Lured(int duration, Pawn target)
    {
        lured = true;
        luredSource = target;
        lureEnd = GlobalVariables.turns + duration;
        this.lureDuration = duration;
    }

    private void Update()
    {
        if(specialDisable)
        {
            if(curseEnd == GlobalVariables.turns)
            {
                specialDisable = false;
            }
        }
        if(lured)
        {
            if (lureEnd == GlobalVariables.turns)
            {
                lured = false;
                luredSource = null;
            }
        }
    }
    public IEnumerator DealAttack(Attack attack, Pawn target)
    {
        if (this == null || isDead)
        {
            Debug.Log($"{pawnName} is dead and cannot attack.");
            yield break;
        }

        if (target == null || target.isDead)
        {
            Debug.Log($"{target.pawnName} is already dead and cannot be attacked.");
            yield break;
        }

        Hex playerHex = this.CurrentTile.GetComponent<Hex>();
        Hex enemyHex = target.CurrentTile.GetComponent<Hex>();
        playerHex.EnableHighlight(Color.white);
        enemyHex.EnableHighlight(Color.red);

        if (attack.attackSound != null)
        {
            PlayAttackSound(attack.attackSound);
            yield return new WaitForSeconds(attack.attackSound.length);
        }

        playerHex.DisableHighlight();
        enemyHex.DisableHighlight();

        // Deal damage to the target
        target.TakeDamage(attack.damage);
        hasAttacked = true;

        Debug.Log($"{pawnName} attacked {target.pawnName} with {attack.attackName}, dealing {attack.damage} damage.");
    }

    public void TakeDamage(int amount) // Current pawn will take certain damage and value of slider will renew
                                        // We could push in negative values to heal as well. ZO
    {
        if (this != null)
        {
            currentHP -= amount;
            healthHUD.SetHP(currentHP);
            if (currentHP <= 0)
            {
                Debug.Log($"{pawnName} has died.");
                StartCoroutine(HandleDeath());
                return;
            } // Only reason PlayerHUD is not being notified immediately about death is because pawns should not
            // be attacking themselves. Need to add selection logic to further death logic...maybe. ZO
            PlayDamageSound();
            Debug.Log($"{gameObject.tag} {pawnName} takes {amount} damage. Current HP: {currentHP}");
        }
        else
        {
            Debug.Log($"{this} is dead so they cannot be attacked");
        }
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
        damageSound = type.pawnTypeDamageSound;
        deathSound = type.pawnTypeDeathSound;
        moveSound = type.pawnTypeMoveSound;
        hasAttacked = false;
        hasActed = false;
    }
    public void ResetStatus()
    {
        hasAttacked = false;
        hasMoved = false;
        hasActed = false;
    }
    public void Move()
    {
        hasMoved = true;
    }
    public void Attack()
    {
        hasAttacked = true;
    }

    public void PlayMoveSound()
    {
        if (this != null)
        {
            PlaySound(moveSound);
        }
        else
        {
            Debug.Log($"{this} is dead so they cannot move.");
        }
    }
    public IEnumerator PlayDeathSound()
    {
        if (deathSound != null && this != null)
        {
            Debug.Log("Playing death sound...");
            audioSource.clip = deathSound;
            audioSource.Play();
            yield return new WaitForSeconds(deathSound.length);
        }
        else
        {
            Debug.Log($"{this} is dead so they cannot die again.");
        }
        player.RemovePawn(this); 
    }
    public void PlayDamageSound()
    {
        if (this != null)
        {
            PlaySound(damageSound);
            pawnAnimation.TriggerFlinch();
        }
        else
        {
            Debug.Log($"{this} is dead so they cannot be damaged.");
        }
    }
    public void PlayAttackSound(AudioClip clip)
    {
        PlaySound(clip);
        if (clip.name == "ShortSwing")
        {
            pawnAnimation.TriggerShortSwing();
        }
        else if (clip.name == "WideSwing")
        {
            pawnAnimation.TriggerWideSwing();
        }
        else if (clip.name == "RangedAttack")
        {
            pawnAnimation.TriggerRangeBow();
        }
        else if (clip.name == "WallOfFire")
        {
            pawnAnimation.TriggerWideSwing();
        }
        else if (clip.name == "Curse")
        {
            pawnAnimation.TriggerShortSwing();
        }
        else if (clip.name == "Necromancy")
        {
            pawnAnimation.TriggerFlinch();
        }
        else if (clip.name == "Lure")
        {
            pawnAnimation.TriggerFlinch();
        }
        else
        {
            pawnAnimation.TriggerFlinch();
        }
    }
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    public void Act()
    {
        hasActed = true;
    }
    private IEnumerator HandleDeath()
    {
        isDead = true;

        if (deathSound != null)
        {
            Debug.Log("Playing death sound...");
            audioSource.clip = deathSound;
            audioSource.Play();
            yield return new WaitForSeconds(deathSound.length);
        }

        Player player = GetComponentInParent<Player>();
        if (player != null)
        {
            player.RemovePawn(this);
        }

        Debug.Log($"{pawnName} has been removed from the game.");
    }
}
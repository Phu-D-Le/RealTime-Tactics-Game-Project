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
    public int pawnSpeed { get; private set; }
    public int maxHP { get; private set; }
    public int currentHP { get; private set; }
    public bool hasAttacked { get; private set; }
    public bool hasMoved { get; private set; }
    public GameObject CurrentTile { get; set; }
    public Attack selectedAttack { get; set; }
    public AudioClip moveSound;
    public AudioClip deathSound;
    public AudioClip damageSound;

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
    }
    public IEnumerator DealAttack(Attack attack, Pawn target) // Deal damage to another pawn. Called by SelectManager. ZO
    {
        if (this != null)
        {
            Hex playerHex = this.CurrentTile.GetComponent<Hex>();
            Hex enemyHex = target.CurrentTile.GetComponent<Hex>();
            playerHex.EnableHighlight(Color.white);
            enemyHex.EnableHighlight(Color.red);
            // Play the attack sound and wait for it to finish
            if (attack.attackSound != null && this != null)
            {
                PlayAttackSound(attack.attackSound);
                yield return new WaitForSeconds(attack.attackSound.length); // Wait for the sound to finish
            }
            playerHex.DisableHighlight();
            enemyHex.DisableHighlight();
            target.TakeDamage(attack.damage);
            hasAttacked = true;
            Debug.Log($"{gameObject.tag} {pawnName} has attacked {target.gameObject.tag} {target.pawnName} with {attack.attackName}, dealing {attack.damage} damage.");
        }
        else
        {
            Debug.Log($"{this} is dead so they cannot attack");
        }
    }
    public void TakeDamage(int amount) // Current pawn will take certain damage and value of slider will renew
                                        // We could push in negative values to heal as well. ZO
    {
        currentHP -= amount;
        healthHUD.SetHP(currentHP);
        if (currentHP <= 0)
        {
            Debug.Log($"{pawnName} has died.");
            StartCoroutine(PlayDeathSound());
            return;
        } // Only reason PlayerHUD is not being notified immediately about death is because pawns should not
        // be attacking themselves. Need to add selection logic to further death logic...maybe. ZO
        PlayDamageSound();
        Debug.Log($"{gameObject.tag} {pawnName} takes {amount} damage. Current HP: {currentHP}");
    }
    public void InitializeFromPawnType(PawnType type)
    {
        pawnName = type.pawnTypeName;
        pawnSprite = type.pawnTypeSprite;
        attacks = new List<Attack>(type.pawnTypeAttacks);  // Create the list of attacks from everything in PawnType.
                                                            // Edit list when adding attacks between scenes. ZO
        pawnSpeed = type.pawnTypeSpeed;
        maxHP = type.pawnTypeMaxHP;
        currentHP = type.pawnTypeCurrentHP;
        damageSound = type.pawnTypeDamageSound;
        deathSound = type.pawnTypeDeathSound;
        moveSound = type.pawnTypeMoveSound;
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
        }
        else
        {
            Debug.Log($"{this} is dead so they cannot be damaged.");
        }
    }
    public void PlayAttackSound(AudioClip clip)
    {
        PlaySound(clip);
    }
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

// This scriptable object holds all the attack information and more can be created in unity by
// clicking create > and at the top of the list is attack or pawn type.
[CreateAssetMenu]
public class PawnType : ScriptableObject
{
    public string pawnTypeName;
    public Sprite pawnTypeSprite;
    public List<Attack> pawnTypeAttacks;
    public int pawnTypeSpeed;
    public int pawnTypeMaxHP;
    public int pawnTypeCurrentHP;
    public AudioClip pawnTypeMoveSound;
    public AudioClip pawnTypeDeathSound;
    public AudioClip pawnTypeDamageSound;
}

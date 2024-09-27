using UnityEngine;

  // This scriptable object holds all the attack information and more can be created in unity by
  // clicking create > and at the top of the list is attack or pawn type.

[CreateAssetMenu]
public class Attack : ScriptableObject
{
    public string attackName;
    public int damage;
    public Sprite attackSprite;
}

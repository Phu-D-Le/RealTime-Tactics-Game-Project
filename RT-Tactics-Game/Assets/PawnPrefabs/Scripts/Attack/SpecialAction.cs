using UnityEngine;

// This scriptable object holds all the attack information and more can be created in unity by
// clicking create > and at the top of the list is attack or pawn type.

[CreateAssetMenu]
public class SpecialAction : ScriptableObject
{
    public string actionName;
    public int damage;
    public Sprite actionSprite;
    public int range;

    public void WallOfFire()
    {

    }

}

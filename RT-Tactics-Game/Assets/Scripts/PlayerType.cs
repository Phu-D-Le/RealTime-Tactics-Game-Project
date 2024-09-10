using System.Collections.Generic;
using UnityEngine;

public class PlayerType : MonoBehaviour
{
    public int Health { get; private set; }
    public int Speed { get; private set; }

    public enum Size
    {
        Small,
        Medium,
        Heavy
    }

    public void PawnStats(Size type)
    {
        switch (type)
        {
            case Size.Small:
                Health = 2;
                Speed = 3;
                // Pass 'this.gameObject' as the pawn to the AssignDefaultAttack method
                Attack.AssignDefaultAttack("Short Swing", 3, this.gameObject);
                break;
            case Size.Medium:
                Health = 3;
                Speed = 2;
                Attack.AssignDefaultAttack("Ranged Attack", 2, this.gameObject);
                break;
            case Size.Heavy:
                Health = 4;
                Speed = 1;
                Attack.AssignDefaultAttack("Wide Swing", 1, this.gameObject);
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
}

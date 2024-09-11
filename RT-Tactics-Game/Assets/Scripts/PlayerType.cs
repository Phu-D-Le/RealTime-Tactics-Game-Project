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
                break;
            case Size.Medium:
                Health = 3;
                Speed = 2;
                break;
            case Size.Heavy:
                Health = 4;
                Speed = 1;
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
}

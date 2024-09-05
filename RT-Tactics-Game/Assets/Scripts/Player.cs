using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerType.Size pawnType; // Declare the unit type
    private PlayerType pawnAttributes; // Find attributes linked to the type

    // Start is called before the first frame update
    void Start()
    {
        pawnAttributes = new PlayerType(pawnType); // Display
        Debug.Log($"Pawn Type: {pawnType}, Health: {pawnAttributes.Health}, Speed: {pawnAttributes.Speed}");
        Debug.Log($"Default Attack: {pawnAttributes.attacks[0].attackName}, Damage: {pawnAttributes.attacks[0].damage}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

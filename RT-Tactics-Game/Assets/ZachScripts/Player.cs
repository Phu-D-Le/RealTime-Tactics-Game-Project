using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject playerInventory; // Reference to the player's inventory
    public DisplayInventory attackMenuDisplay; // Reference to the DisplayInventory panel for attacks

    public void DisplayPawnsAndAttacks()
{
    if (playerInventory == null)
    {
        Debug.LogWarning("Player Inventory is not assigned!");
        return;
    }

    Debug.Log($"Displaying inventory for {name}");
    foreach (InventorySlot slot in playerInventory.Container)
    {
        PawnObject pawn = slot.item as PawnObject;
        if (pawn != null)
        {
            Debug.Log($"Pawn Type: {pawn.pawnType}, Health: {pawn.Health}, Speed: {pawn.Speed}");

            // Display learned attacks
            foreach (var attack in pawn.learnedAttacks)
            {
                Debug.Log($"Attack: {attack.name}, Damage: {attack.Damage}");
            }
        }
    }
}


    public void UpdateAttackMenu(PawnObject selectedPawn)
{
    if (attackMenuDisplay == null) 
    {
        Debug.LogWarning("AttackMenuDisplay is not assigned!");
        return;
    }

    if (selectedPawn == null) 
    {
        Debug.LogWarning("SelectedPawn is null!");
        return;
    }

    // Create a new inventory to hold the pawn's attacks
    InventoryObject attackInventory = ScriptableObject.CreateInstance<InventoryObject>();

    // Add the pawn's attacks to the new inventory
    foreach (var attack in selectedPawn.learnedAttacks)
    {
        attackInventory.AddItem(attack, 1);
    }

    // Update the DisplayInventory with the new inventory
    attackMenuDisplay.UpdateMenu(attackInventory);
}

}

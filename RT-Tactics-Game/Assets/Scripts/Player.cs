using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject playerInventory; // Player's overall inventory, including pawns
    public DisplayInventory playerInventoryDisplay; // UI component to display pawns
    public DisplayInventory attackMenuDisplay; // UI component to display attacks

    private Dictionary<PawnObject, GameObject> pawnInstances = new Dictionary<PawnObject, GameObject>();

    private void Start()
    {
        InitializePawnsFromScene();
        DisplayPawnsAndAttacks();
    }

    private void InitializePawnsFromScene()
    {
        // Clear the player's inventory first
        playerInventory.Container.Clear();

        foreach (Transform child in transform)
        {
            Pawn pawnComponent = child.GetComponent<Pawn>();
            if (pawnComponent != null)
            {
                PawnObject pawnObject = pawnComponent.pawnData;
                if (pawnObject != null)
                {
                    // Add the pawn to the player's inventory
                    playerInventory.AddItem(pawnObject, 1);
                    pawnInstances[pawnObject] = child.gameObject;
                }
            }
        }

        // Update the inventory UI
        if (playerInventoryDisplay != null)
        {
            playerInventoryDisplay.UpdateMenu(playerInventory);
        }
    }

    public void DisplayPawnsAndAttacks()
    {
        if (playerInventoryDisplay != null)
        {
            playerInventoryDisplay.UpdateMenu(playerInventory);
        }
    }

    public void OnPawnSelected(PawnObject selectedPawn)
    {
        if (selectedPawn == null)
        {
            Debug.LogWarning("Selected pawn is null.");
            return;
        }

        // Update the attack menu with the selected pawn's attacks
        UpdateAttackMenu(selectedPawn);
    }

    public void UpdateAttackMenu(PawnObject selectedPawn)
    {
        if (attackMenuDisplay == null || selectedPawn == null) return;

        // Display the selected pawn's attacks
        attackMenuDisplay.UpdateMenu(selectedPawn.pawnInventory);
    }
    public void DisplayPawnDetails(PawnObject selectedPawn)
    {
        if (selectedPawn == null)
        {
            Debug.LogWarning("Selected pawn is null.");
            return;
        }

        // Display detailed information about the pawn
        Debug.Log($"Pawn Details: Health: {selectedPawn.Health} Speed: {selectedPawn.Speed} Type: {selectedPawn.pawnType}");
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Player inventory for pawns and UI display components
    public InventoryObject playerInventory; // Player's overall inventory, including pawns
    public DisplayInventory playerInventoryDisplay; // UI component to display pawns
    public DisplayInventory attackMenuDisplay; // UI component to display attacks

    // Dictionary to track pawn instances
    private Dictionary<PawnObject, GameObject> pawnInstances = new Dictionary<PawnObject, GameObject>();

    private void Start()
    {
        // Initialize the pawns and display their attacks
        InitializePawnsFromScene();
        DisplayPawnsAndAttacks();
    }

    // Initialize pawns from the scene and assign data
    private void InitializePawnsFromScene()
    {
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

                    // Keep track of the pawn prefab instance
                    pawnInstances[pawnObject] = child.gameObject;

                    // Initialize health for the pawn using PawnObject data
                    Health pawnHealth = child.GetComponent<Health>();
                    if (pawnHealth != null)
                    {
                        Debug.Log($"Initializing health for {child.name} with base health: {pawnObject.Health}");
                        pawnHealth.Initialize(pawnObject); // Pass the whole PawnObject
                    }
                    else
                    {
                        Debug.LogWarning($"No Health component found on {child.name}.");
                    }

                    // Log pawn details for debugging
                    Debug.Log($"Pawn {child.name} initialized with Health: {pawnObject.Health} and Speed: {pawnObject.Speed}");
                }
            }
        }

        if (playerInventoryDisplay != null)
        {
            playerInventoryDisplay.UpdateMenu(playerInventory);
        }
    }

    // Display pawns and their corresponding attacks in the UI
    public void DisplayPawnsAndAttacks()
    {
        if (playerInventoryDisplay != null)
        {
            playerInventoryDisplay.UpdateMenu(playerInventory);
        }
    }

    // Called when a pawn is selected to display its attacks
    public void OnPawnSelected(PawnObject selectedPawn)
    {
        if (selectedPawn == null)
        {
            Debug.LogWarning("Selected pawn is null.");
            return;
        }

        UpdateAttackMenu(selectedPawn); // Correct data type: PawnObject
    }

    public void UpdateAttackMenu(PawnObject selectedPawn)
    {
        if (attackMenuDisplay == null || selectedPawn == null) return;

        attackMenuDisplay.UpdateMenu(selectedPawn.pawnInventory); // Updates based on the pawn's inventory
    }
}
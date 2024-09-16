using UnityEngine;
using UnityEngine.UI;

public class PawnButton : MonoBehaviour
{
    public PawnObject pawn; // Reference to the pawn ScriptableObject
    private GameManager gameManager;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        gameManager = FindObjectOfType<GameManager>(); // Find the GameManager in the scene
    }

    void OnButtonClick()
    {
        gameManager.SetSelectedPawn(pawn); // Now passing the PawnObject directly
        Player currentPlayer = gameManager.GetCurrentPlayer();
        if (currentPlayer != null)
        {
            currentPlayer.UpdateAttackMenu(pawn); // Update the attack menu with the selected pawn
        }
    }
}

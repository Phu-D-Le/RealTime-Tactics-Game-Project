using UnityEngine;
using UnityEngine.UI;

public class PawnButton : MonoBehaviour
{
    public PawnObject pawn; // Reference to the pawn
    private GameManager gameManager; // Reference to the GameManager

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        gameManager = FindObjectOfType<GameManager>(); // Find the GameManager in the scene
    }

    void OnButtonClick()
    {
        GameObject pawnGameObject = gameObject; // Reference to the clicked pawn's GameObject
        gameManager.SetSelectedPawn(pawnGameObject); // Set the selected pawn in the GameManager

        Player currentPlayer = gameManager.GetCurrentPlayer();
        if (currentPlayer != null)
        {
            currentPlayer.UpdateAttackMenu(pawn); // Update the attack menu with the selected pawn
        }
    }
}

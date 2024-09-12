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

        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnButtonClick()
    {
        Player currentPlayer = gameManager.GetCurrentPlayer();
        if (currentPlayer != null)
        {
            // Update the attack menu with the selected pawn
            currentPlayer.UpdateAttackMenu(pawn);
        }
    }
}

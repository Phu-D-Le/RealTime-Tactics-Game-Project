using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This does the same thing as AttackHUD except it selects from the player's list of pawns
// and populates the buttons within PawnHUD with the player's pawns. This allows for one PawnHUD to exist and it
// only needs to be assigned to BattleSystem within scene. ZO

public class PawnHUD : MonoBehaviour
{
    private List<Button> pawnButtons;
    private AttackHUD attackHUD;  // Reference AttackHUD once instead of on each pawn. ZO
    public Pawn selectedPawn;
    private SelectManager selectManager;

    void Awake()
    {
        pawnButtons = new List<Button>(GetComponentsInChildren<Button>());
        attackHUD = GameObject.FindObjectOfType<AttackHUD>();
        selectManager = FindObjectOfType<SelectManager>();
    }

    public void SetPlayerCanvas(Player player)
    {
        for (int i = 0; i < pawnButtons.Count; i++)
        {
            if (i < player.pawns.Count)
            {
                Pawn pawn = player.pawns[i].GetComponent<Pawn>(); 
                if (pawn != null)
                {
                    pawnButtons[i].GetComponent<Image>().sprite = pawn.pawnSprite;
                    TextMeshProUGUI buttonText = pawnButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.text = pawn.pawnName;
                    }
                    pawnButtons[i].onClick.RemoveAllListeners();
                    pawnButtons[i].onClick.AddListener(() => 
                    {
                        OpenAttackHUD(pawn);
                        OnPawnSelected(pawn);
                        OnMoveButtonClicked(); // Open AttackHUD and populate buttons according to pawn attack list. Then update
                        // SelectManager so it knows what pawn is being referenced from the menu. Finally update SelectManager
                        // so that it knows the pawn can move. ZO
                    });
                    pawnButtons[i].gameObject.SetActive(true);
                }
            }
            else
            {
                pawnButtons[i].gameObject.SetActive(false);  // Hide buttons if no matching pawn
            }
        }
    }

    private void OpenAttackHUD(Pawn pawn)
    {
        if (pawn.hasAttacked)
        {
            Debug.Log($"{pawn.pawnName} has already attacked and cannot attack again this turn.");
            return;
        }
        else if (attackHUD != null && !pawn.hasAttacked)
        {
            attackHUD.gameObject.SetActive(true);
            attackHUD.SetUpAttacks(pawn);  // Pass the pawn to the AttackHUD to display attacks. ZO
        }
    }
    public void OnPawnSelected(Pawn pawn)
    {
        selectedPawn = pawn;
        if (selectManager != null && selectedPawn != null && !selectedPawn.hasMoved)
        {
            selectManager.HighlightTilesForPawn(selectedPawn);
        }
        else
        {
            Debug.Log($"SelectManager not found or selectedPawn is null.");
        }
    }
    public void OnMoveButtonClicked()
    {
        selectManager.IsMoving = true;
        selectManager.IsAttacking = false;
    }
}

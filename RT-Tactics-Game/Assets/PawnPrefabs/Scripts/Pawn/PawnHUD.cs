using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PawnHUD : MonoBehaviour
{
    private List<Button> pawnButtons;
    private AttackHUD attackHUD;
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
                if (pawn != null && !pawn.isAIControlled)
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
                    });
                    pawnButtons[i].gameObject.SetActive(true);
                }
            }
            else
            {
                pawnButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void HidePlayerCanvas()
    {
        foreach (var button in pawnButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void OpenAttackHUD(Pawn pawn)
    {
        if (pawn.hasAttacked)
        {
            Debug.Log($"{pawn.pawnName} has already attacked and cannot attack again this turn.");
            return;
        }
        else if (attackHUD != null && !pawn.hasAttacked && selectManager.ready)
        {
            attackHUD.gameObject.SetActive(true);
            attackHUD.SetUpAttacks(pawn);
        }
    }

    public void OnPawnSelected(Pawn pawn)
    {
        selectedPawn = pawn;
        if (selectManager != null && selectedPawn != null && !selectedPawn.hasMoved && selectManager.ready)
        {
            selectManager.SetMovementMode(true);
            selectManager.HighlightTilesForPawn(selectedPawn);
        }
        else
        {
            Debug.Log($"SelectManager not found or selectedPawn is null.");
        }
    }
}

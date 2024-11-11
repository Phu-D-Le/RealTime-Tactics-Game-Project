using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionHUD : MonoBehaviour
{
    private List<Button> actionButtons;
    private SelectManager selectManager;

    void Awake()
    {
        // Dynamically get all buttons from the AttackHUD in scene. ZO
        actionButtons = new List<Button>(GetComponentsInChildren<Button>());
        selectManager = FindObjectOfType<SelectManager>();
    }
    public void SetUpActions(Pawn pawn)
    {
        for (int i = 0; i < actionButtons.Count; i++)
        {
            if (i < pawn.actions.Count)
            {
                SpecialAction currentAction = pawn.actions[i]; // Attack holder for each attack within this pawn's attack list. ZO

                actionButtons[i].GetComponent<Image>().sprite = currentAction.actionSprite;
                TextMeshProUGUI buttonText = actionButtons[i].GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    buttonText.text = currentAction.actionName;
                }

                actionButtons[i].onClick.RemoveAllListeners();
                actionButtons[i].onClick.AddListener(() =>
                {
                    HandleActionSelection(pawn, currentAction);
                    CloseActionHUD();

                }); // Adds three methods on each button dynamically. currently it handles attack tile selection in SelectManager,
                // closes the attackHUD so pawn can only attack once, and updates SelectManager to go into attacking mode. ZO

                actionButtons[i].gameObject.SetActive(true);
            }
            else
            {
                actionButtons[i].gameObject.SetActive(false);  // Failsafe. There is nothing stopping a pawn from having > 3 attacks. ZO
            }
        }
    }
    private void CloseActionHUD()
    {
        gameObject.SetActive(false);
    }
    private void HandleActionSelection(Pawn pawn, SpecialAction action)
    {
        if (selectManager.ready)
        {
            selectManager.SetAttackMode(true);
            selectManager.HighlightTilesForAction(pawn, action); //WIP, need to find a way to properly implement actions outside of damage and range, also need to figure out AOE
            selectManager.selectedAction = action;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

    // This method is beefy but the general idea is a pawn is clicked in the PawnHUD
    // and is sent here to iterate through each AttackHUD button and assign each one
    // with the pawn's (from the list of pawns) attacks. 
    // Only one AttackHUD is necessary as this HUD is recreated 
    // every time a pawn is clicked (with the RemoveAllListeners).
    // Assigned to BattleSystem. ZO
public class AttackHUD : MonoBehaviour
{
    private List<Button> attackButtons;
    private SelectManager selectManager;
    private ActionHUD actionHUD;

    void Awake()
    {
        // Dynamically get all buttons from the AttackHUD in scene. ZO
        attackButtons = new List<Button>(GetComponentsInChildren<Button>());
        selectManager = FindObjectOfType<SelectManager>();
        actionHUD = FindObjectOfType<ActionHUD>();
    }
    public void SetUpAttacks(Pawn pawn)
    {
        for (int i = 0; i < attackButtons.Count; i++)
        {
            if (i < pawn.attacks.Count)
            {
                Attack currentAttack = pawn.attacks[i]; // Attack holder for each attack within this pawn's attack list. ZO
                
                attackButtons[i].GetComponent<Image>().sprite = currentAttack.attackSprite;
                TextMeshProUGUI buttonText = attackButtons[i].GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    buttonText.text = currentAttack.attackName;
                }

                attackButtons[i].onClick.RemoveAllListeners();
                attackButtons[i].onClick.AddListener(() => 
                {
                    HandleAttackSelection(pawn, currentAttack);
                    actionHUD.CloseActionHUD();
                    CloseAttackHUD();

                }); // Adds three methods on each button dynamically. currently it handles attack tile selection in SelectManager,
                // closes the attackHUD so pawn can only attack once, and updates SelectManager to go into attacking mode. ZO

                attackButtons[i].gameObject.SetActive(true);
            }
            else
            {
                attackButtons[i].gameObject.SetActive(false);  // Failsafe. There is nothing stopping a pawn from having > 3 attacks. ZO
            }
        }
    }
    public void CloseAttackHUD()
    {
        gameObject.SetActive(false);
    }
    private void HandleAttackSelection(Pawn pawn, Attack attack)
    {
        if (selectManager.ready)
        {
            selectManager.SetAttackMode(true);
            selectManager.HighlightTilesForAttack(pawn, attack);
            selectManager.selectedAttack = attack;
        }
    }
}
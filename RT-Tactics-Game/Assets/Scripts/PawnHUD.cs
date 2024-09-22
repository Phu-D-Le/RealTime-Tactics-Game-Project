using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PawnHUD : MonoBehaviour
{
    public List<Button> pawnButtons;
    public AttackHUD attackHUD;
    //public Slider healthBar;

    //public void SetPlayerCanvas(Player player)
    //{
        //healthBar.maxValue = pawn.maxHP;
      //  healthBar.value = pawn.currentHP;
    //}
    public void SetPlayerCanvas(Player player)
    {
        for (int i = 0; i < pawnButtons.Count; i++)
        {
            if (i < player.pawns.Count)
            {
                Pawn pawn = player.pawns[i].GetComponent<Pawn>();
                if (pawn != null)
                {
                    pawnButtons[i].GetComponent<Image>().sprite = pawn.pawnSprite;  // Set the sprite for each button
                    TextMeshProUGUI buttonText = pawnButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.text = pawn.pawnName;  // Set the button's name text to the pawn's name
                    }
                    pawnButtons[i].gameObject.SetActive(true);  // Make sure the button is visible
                    pawnButtons[i].onClick.RemoveAllListeners();  // Clear old listeners
                    pawnButtons[i].onClick.AddListener(() => OpenAttackHUD(pawn));  // Assign new listener
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
        // Show the correct AttackHUD for the clicked pawn
        if (pawn.attackHUD != null)
        {
            pawn.attackHUD.gameObject.SetActive(true);
            pawn.attackHUD.SetUpAttacks(pawn);
        }
    }
    //public void SetHP(int hp)
    //{
      //  healthBar.value = hp;
    //}
}
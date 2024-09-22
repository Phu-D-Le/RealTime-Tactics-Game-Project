using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackHUD : MonoBehaviour
{
    public List<Button> attackButtons;  // Buttons for each attack option

    public void SetUpAttacks(Pawn pawn)
    {
        // Assuming the Pawn has multiple attacks stored in a list (e.g., List<Attack> attacks)
        for (int i = 0; i < attackButtons.Count; i++)
        {
            if (i < pawn.attacks.Count)  // Check if the pawn has an attack for this button
            {
                Attack currentAttack = pawn.attacks[i];  // Get the corresponding attack
                
                // Update button sprite
                attackButtons[i].GetComponent<Image>().sprite = currentAttack.attackSprite;

                // Update button text with the attack's name
                TextMeshProUGUI buttonText = attackButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = currentAttack.attackName;
                }

                // Set the onClick event for the button to trigger the attack
                attackButtons[i].onClick.RemoveAllListeners();
                attackButtons[i].onClick.AddListener(() => ExecuteAttack(currentAttack));

                attackButtons[i].gameObject.SetActive(true);  // Ensure the button is visible
            }
            else
            {
                attackButtons[i].gameObject.SetActive(false);  // Hide buttons if no matching attack
            }
        }
    }

    private void ExecuteAttack(Attack attack)
    {
        Debug.Log($"Executing {attack.attackName} which deals {attack.damage} damage.");
        // Logic for executing the attack
    }
}


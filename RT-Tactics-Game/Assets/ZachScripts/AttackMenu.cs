using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackMenu : MonoBehaviour
{
    public GameObject buttonPrefab; // Reference to a button prefab
    public Transform buttonParent; // Parent of the buttons in the canvas
    public int initialButtonCount = 10; // Initial number of buttons to pool

    private List<GameObject> buttonPool = new List<GameObject>();

    private void Start()
    {
        // Initialize the button pool
        for (int i = 0; i < initialButtonCount; i++)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonParent);
            buttonObj.SetActive(false);
            buttonPool.Add(buttonObj);
            Debug.Log("Button instantiated and added to pool: " + buttonObj.name);
        }
    }

    public void UpdateMenu(Player player, int turn)
    {
        Debug.Log("Updating menu for player: " + player.name + " on turn: " + turn);

        // Deactivate all buttons
        foreach (var buttonObj in buttonPool)
        {
            buttonObj.SetActive(false);
        }

        // Get the player's current attacks
        List<Attack> playerAttacks = AttackManager.GetLearnedAttacks(player.gameObject);

        Debug.Log("Player currently has " + playerAttacks.Count + " attacks:");
        foreach (var attack in playerAttacks)
        {
            Debug.Log(" - " + attack.attackName);
        }

        // No need to fetch new attacks if player already has 3
        if (playerAttacks.Count >= 3)
        {
            Debug.LogWarning("Player has the maximum number of attacks.");
            return;
        }

        // Get available additional attacks for the current turn
        List<Attack> availableAttacks = AttackManager.GetAvailableAttacksForTurn(player.pawnType, turn);
        Debug.Log("Found " + availableAttacks.Count + " available attacks for turn " + turn);

        // Combine playerAttacks and availableAttacks
        List<Attack> combinedAttacks = new List<Attack>(playerAttacks);
        combinedAttacks.AddRange(availableAttacks);
        Debug.Log("Combined attacks:");
        foreach (var attack in combinedAttacks)
        {
            Debug.Log(" - " + attack.attackName);
        }

        if (combinedAttacks.Count == 0)
        {
            Debug.LogWarning("No available attacks for this turn.");
            return;
        }

        int buttonIndex = 0;

        foreach (var attack in combinedAttacks)
        {
            if (buttonIndex >= buttonPool.Count)
            {
                Debug.LogError("Not enough buttons in the pool.");
                break;
            }

            GameObject buttonObj = buttonPool[buttonIndex];
            buttonObj.SetActive(true);

            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            if (button == null || buttonText == null)
            {
                Debug.LogError("Button or Text component is missing from the button prefab.");
                continue;
            }

            buttonText.text = attack.attackName;
            button.onClick.RemoveAllListeners(); // Remove old listeners
            button.onClick.AddListener(() => OnAttackButtonClicked(player.gameObject, attack));

            buttonIndex++;
        }
    }

    private void OnAttackButtonClicked(GameObject player, Attack attack)
    {
    Debug.Log($"Button clicked for {attack.attackName}");
    AttackManager.LearnAttack(player, attack);
    }

}
